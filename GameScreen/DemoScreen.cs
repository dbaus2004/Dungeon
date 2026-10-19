using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Dungeon.Collisions;
using Dungeon.Input;
using Dungeon.Objects;
using Dungeon.Sprites;
using Dungeon.DiceClasses;
using Dungeon.Menu;
using System;

namespace Dungeon;

public class DemoScreen : GameScreen
{
    private readonly Game _game;
    private readonly SpriteBatch _spriteBatch;
    private AttackLog _attackLog;
    private readonly GraphicsDevice _graphicsDevice;
    private EntityDescription _entityDescription;
    private SpriteFont _attackLogFont;
    private string _latestAttackLog = "Right click for info, left click to shoot";
    private readonly ScreenManager _screenManager;
    private readonly InputAction _inspectAction = new InputAction(null, null, true, rightMouseButton: true);
    private readonly Entity[] _entities;
    private SoundEffect FireBallShoot;
    private SoundEffect FireBallHit;
    private SoundEffect Death;
    private Song backgroundMusic;
    private readonly IInputState _inputState;
    private readonly InputAction _shootAction = new InputAction(null, null, true, leftMouseButton: true);
    private readonly List<FireballSprite> _fireballs = new List<FireballSprite>();
    private readonly Entity[] _mobs;
    private ContentManager _content;
    public Entity Player = new PlayerWizard(new Vector2(200, 200));
    public Entity EvilWizard = new EvilWizard(new Vector2(200, 400));
    public Entity EvilWizard2 = new EvilWizard(new Vector2(400, 200));
    public Entity EvilWizard3 = new EvilWizard(new Vector2(400, 400));
    public GameTime GameTime;


    public DemoScreen(
    IInputState inputState,
    ScreenManager screenManager,
    SpriteBatch spriteBatch,
    GraphicsDevice graphicsDevice,
    Game game)
    {
        _game = game;
        _inputState = inputState;
        _screenManager = screenManager;
        _spriteBatch = spriteBatch;
        _graphicsDevice = graphicsDevice;

        _mobs = new[] { EvilWizard, EvilWizard2, EvilWizard3 };
        _entities = new[] { Player, EvilWizard, EvilWizard2, EvilWizard3 };
    }
    public override void LoadContent(ContentManager content)
    {
        _content = content;

        _attackLogFont = content.Load<SpriteFont>("arial");
        _attackLog = new AttackLog(_spriteBatch, _attackLogFont, _graphicsDevice);

        Player.Sprite.LoadContent(content);

        foreach (Entity mob in _mobs)
            mob.Sprite.LoadContent(content);

        FireBallShoot = content.Load<SoundEffect>("DungeonSFX/10_human_special_atk_2");
        FireBallHit = content.Load<SoundEffect>("DungeonSFX/20_orc_special_atk");
        Death = content.Load<SoundEffect>("DungeonSFX/24_orc_death_spin");
        backgroundMusic = content.Load<Song>("DungeonMusic/Goblins_Dance_(Battle)");
        MediaPlayer.IsRepeating = true;
        MediaPlayer.Play(backgroundMusic);

    }
    public override void UnloadContent()
    {
        _attackLog?.Dispose();
    }
    public override void Update(GameTime gameTime)
    {
        GameTime = gameTime;
        if (_inspectAction.Occured(_inputState, null, out _))
        {
            Point mouse = _inputState.MousePosition;
            var clickBounds = new BoundingRectangle(mouse.X, mouse.Y, 1, 1);

            // Check from last drawn to first, in case entities overlap.
            for (int i = _entities.Length - 1; i >= 0; i--)
            {
                Entity entity = _entities[i];
                if (!entity.IsAlive)
                    continue;
                if (entity.Sprite.HitBox is BoundingRectangle hitBox &&
                    hitBox.CollidesWith(clickBounds))
                {
                    _entityDescription = new EntityDescription(
                        entity,
                        _spriteBatch,
                        _attackLogFont,
                        _graphicsDevice);

                    _entityDescription.Open();
                    return;
                }
            }
        }
        if (_shootAction.Occured(_inputState, null, out _))
        {
            Point mouse = _inputState.MousePosition;
            Vector2 target = new Vector2(mouse.X, mouse.Y);

            if (Player.Sprite is WizardSprite wizardSprite &&
                wizardSprite.TryShootAt(_game, target, out FireballSprite fireball))
            {
                fireball.LoadContent(_content);
                _fireballs.Add(fireball);
                FireBallShoot.Play();

                foreach (Entity mob in _mobs)
                {
                    if (!mob.IsAlive || !mob.Sprite.HitBox.CollidesWith(fireball.Destination))
                        continue;

                    var attack = new AttackRoll(
                        Player.Name,
                        mob.Name,
                        mob.AC,
                        new (int, string)[] { (10, "Fireball accuracy") });

                    _latestAttackLog = attack.Log;

                    if (attack.IsHit)
                    {
                        int damageRoll = Dice.D20();
                        uint damage = (uint)(damageRoll + 10);

                        mob.TakeTrueDamage(damage);
                        _latestAttackLog +=
                            $" \nDamage: d20 ({damageRoll}) + 10 = {damage} true damage. " +
                            $"\nRemaining HP: {mob.HP}.";

                        if (!mob.IsAlive)
                            Death.Play();
                    }

                    FireBallHit.Play();
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
        _entityDescription?.Draw(GameTime);
        
        if (Player.IsAlive)
            Player.Sprite.Draw(GameTime, spriteBatch);
        _attackLog.Draw(_latestAttackLog);
        foreach (Entity mob in _mobs)
        {
            if (mob.IsAlive)
                mob.Sprite.Draw(GameTime, spriteBatch);
        }

        foreach (FireballSprite fireball in _fireballs)
            fireball.Draw(GameTime, spriteBatch);
    }
}