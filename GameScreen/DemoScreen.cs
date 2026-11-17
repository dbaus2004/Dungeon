using System.Collections.Generic;
using System.IO;
using Dungeon.Collisions;
using Dungeon.DiceClasses;
using Dungeon.Input;
using Dungeon.Menu;
using Dungeon.Objects;
using Dungeon.ParticleSystem;
using Dungeon.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Dungeon;

public class DemoScreen : GameScreen
{
    private readonly Game _game;
    private readonly SpriteBatch _spriteBatch;
    private readonly GraphicsDevice _graphicsDevice;
    private readonly IDiceRoller _diceRoller;
    private readonly CombatResolver _combatResolver;
    private readonly IEntityFactory _entityFactory;
    private readonly Tilemap _tilemap;
    private AttackLog _attackLog;
    private SpriteFont _attackLogFont;
    private EntityDescription _entityDescription;
    private string _latestAttackLog = "Right click for info, left click to shoot";
    private SoundEffect _fireballShoot;
    private SoundEffect _fireballHit;
    private SoundEffect _deathSound;
    private Song _backgroundMusic;
    private readonly IInputState _inputState;
    private readonly InputAction _shootAction = new InputAction(
        null,
        null,
        true,
        leftMouseButton: true
    );
    private readonly InputAction _inspectAction = new InputAction(
        null,
        null,
        true,
        rightMouseButton: true
    );
    private readonly List<FireballSprite> _fireballs = new List<FireballSprite>();
    private ContentManager _content;
    private FireballExplosion _fireballExplosion;
    private GameTime _gameTime;

    private readonly List<Entity> _entities = new();
    private readonly List<Entity> _mobs = new();
    public Entity Player { get; private set; }

    private const string DefaultMapFilename = "DemoDungeon.tmj";
    private const string DefaultTilesetAssetName =
        "2D Pixel Dungeon Asset Pack/character and tileset/Dungeon_Tileset.png";

    private static readonly AttackDefinition FireballAttack = new(
        "Fireball",
        10,
        ENUM.Damage.Heat,
        ENUM.DamageType.Magic,
        ENUM.DiceType.d10,
        5
    );

    public DemoScreen(
        IInputState inputState,
        SpriteBatch spriteBatch,
        GraphicsDevice graphicsDevice,
        Game game,
        IDiceRoller diceRoller = null,
        IEntityFactory entityFactory = null,
        string mapFilename = DefaultMapFilename,
        string tilesetAssetName = DefaultTilesetAssetName
    )
    {
        _game = game;
        _inputState = inputState;
        _spriteBatch = spriteBatch;
        _graphicsDevice = graphicsDevice;
        _diceRoller = diceRoller ?? Dice.DefaultRoller;
        _combatResolver = new CombatResolver(_diceRoller);
        _entityFactory = entityFactory ?? new EntityFactory(_diceRoller);
        _tilemap = new Tilemap(mapFilename, tilesetAssetName);
    }

    public override void LoadContent(ContentManager content)
    {
        _content = content;

        _attackLogFont = content.Load<SpriteFont>("arial");
        _attackLog = new AttackLog(_spriteBatch, _attackLogFont, _graphicsDevice);

        _fireballShoot = content.Load<SoundEffect>("DungeonSFX/10_human_special_atk_2");
        _fireballHit = content.Load<SoundEffect>("DungeonSFX/20_orc_special_atk");
        _deathSound = content.Load<SoundEffect>("DungeonSFX/24_orc_death_spin");
        _backgroundMusic = content.Load<Song>("DungeonMusic/Goblins_Dance_(Battle)");
        MediaPlayer.IsRepeating = true;
        _tilemap.LoadContent(content);
        _tilemap.FitToViewport(_graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height);

        List<Vector2> playerSpawns = _tilemap.GetTilePositions(
            "Char/MobSpawn",
            _entityFactory.PlayerSpawnGid
        );

        if (playerSpawns.Count != 1)
            throw new InvalidDataException("Expected exactly one player spawn in Char/MobSpawn.");

        var mobSpawns = new List<(int SpawnGid, Vector2 Position)>();
        var enemySpawnGids = new HashSet<int>(_entityFactory.EnemySpawnGids);
        foreach (int spawnGid in _tilemap.GetTileGids("Char/MobSpawn"))
        {
            if (spawnGid == _entityFactory.PlayerSpawnGid)
                continue;
            if (!enemySpawnGids.Contains(spawnGid))
                throw new InvalidDataException(
                    $"No enemy factory is registered for spawn GID {spawnGid}."
                );

            foreach (Vector2 position in _tilemap.GetTilePositions("Char/MobSpawn", spawnGid))
                mobSpawns.Add((spawnGid, position));
        }

        if (mobSpawns.Count == 0)
            throw new InvalidDataException("Expected at least one enemy spawn in Char/MobSpawn.");

        Player = _entityFactory.CreatePlayer(playerSpawns[0]);
        _entities.Add(Player);

        foreach ((int spawnGid, Vector2 position) in mobSpawns)
        {
            Entity mob = _entityFactory.CreateEnemy(spawnGid, position);
            _mobs.Add(mob);
            _entities.Add(mob);
        }

        foreach (Entity entity in _entities)
        {
            if (entity.Sprite is IScalableSprite scalableSprite)
                scalableSprite.DisplaySize = _tilemap.DisplayTileSize;

            entity.Sprite.LoadContent(content);
        }

        _fireballExplosion = new FireballExplosion(_game, 20, _spriteBatch, content);
        _fireballExplosion.EnsureContentLoaded();
        _game.Components.Add(_fireballExplosion);
    }

    public override void UnloadContent()
    {
        MediaPlayer.Stop();
        _attackLog?.Dispose();
        if (_fireballExplosion != null)
        {
            _game.Components.Remove(_fireballExplosion);
            _fireballExplosion.Dispose();
            _fireballExplosion = null;
        }
    }

    public override void OnActivated()
    {
        if (_fireballExplosion != null)
        {
            _fireballExplosion.Enabled = true;
            _fireballExplosion.Visible = true;
        }

        if (_backgroundMusic != null)
            MediaPlayer.Play(_backgroundMusic);
    }

    public override void OnDeactivated()
    {
        if (_fireballExplosion != null)
        {
            _fireballExplosion.Enabled = false;
            _fireballExplosion.Visible = false;
        }

        MediaPlayer.Stop();
    }

    public override bool HandleCancel()
    {
        if (_entityDescription?.IsOpen != true)
            return false;

        _entityDescription.Close();
        return true;
    }

    public override void Update(GameTime gameTime)
    {
        _gameTime = gameTime;
        if (_inspectAction.Occurred(_inputState, null, out _))
        {
            Point mouse = _inputState.MousePosition;
            var clickBounds = new BoundingRectangle(mouse.X, mouse.Y, 1, 1);

            // Check from last drawn to first, in case entities overlap.
            for (int i = _entities.Count - 1; i >= 0; i--)
            {
                Entity entity = _entities[i];
                if (!entity.IsAlive)
                    continue;
                if (
                    entity.Sprite.HitBox is BoundingRectangle hitBox
                    && hitBox.CollidesWith(clickBounds)
                )
                {
                    _entityDescription = new EntityDescription(
                        entity,
                        _spriteBatch,
                        _attackLogFont,
                        _graphicsDevice
                    );

                    _entityDescription.Open();
                    return;
                }
            }

            _entityDescription?.Close();
        }
        if (Player.IsAlive && _shootAction.Occurred(_inputState, null, out _))
        {
            Point mouse = _inputState.MousePosition;
            Vector2 target = new Vector2(mouse.X, mouse.Y);

            if (
                Player.Sprite is WizardSprite wizardSprite
                && wizardSprite.TryShootAt(
                    _fireballExplosion,
                    target,
                    out FireballSprite fireball
                )
            )
            {
                fireball.LoadContent(_content);
                _fireballs.Add(fireball);
                _fireballShoot.Play();

                foreach (Entity mob in _mobs)
                {
                    if (!mob.IsAlive || !mob.Sprite.HitBox.CollidesWith(fireball.Destination))
                        continue;

                    CombatResult result = _combatResolver.Resolve(
                        Player,
                        mob,
                        FireballAttack
                    );
                    _latestAttackLog = result.Log;

                    if (!mob.IsAlive)
                        _deathSound.Play();

                    _fireballHit.Play();
                    break;
                }
            }
        }
        for (int i = _fireballs.Count - 1; i >= 0; i--)
        {
            FireballSprite fireball = _fireballs[i];
            fireball.Update(gameTime);

            if (fireball.Destroyed)
                _fireballs.RemoveAt(i);
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        _tilemap.Draw(_gameTime, spriteBatch);
        if (Player.IsAlive)
            Player.Sprite.Draw(_gameTime, spriteBatch);
        foreach (Entity mob in _mobs)
        {
            if (mob.IsAlive)
                mob.Sprite.Draw(_gameTime, spriteBatch);
        }

        foreach (FireballSprite fireball in _fireballs)
            fireball.Draw(_gameTime, spriteBatch);
    }

    public override void DrawOverlay(SpriteBatch spriteBatch)
    {
        _attackLog.Draw(_latestAttackLog);
        _entityDescription?.Draw(_gameTime);
    }
}
