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
using System;

namespace Dungeon;

public class DemoScreen : GameScreen
{
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


    public DemoScreen(IInputState inputState, ScreenManager screenManager)
    {
        _inputState = inputState;
        _screenManager = screenManager;
        _mobs = new[] { EvilWizard, EvilWizard2, EvilWizard3 };
        _entities = new[] { Player, EvilWizard, EvilWizard2, EvilWizard3 };
    }
    public override void LoadContent(ContentManager content)
    {
        _content = content;

        _attackLogFont = content.Load<SpriteFont>("arial");

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
                    _screenManager.PushScreen(
                        new EntityDescription(entity, _inputState, _screenManager));
                    return;
                }
            }
        }
        if (_shootAction.Occured(_inputState, null, out _))
        {
            Point mouse = _inputState.MousePosition;
            Vector2 target = new Vector2(mouse.X, mouse.Y);

            if (Player.Sprite is WizardSprite wizard &&
                wizard.TryShootAt(target, out FireballSprite fireball))
            {
                fireball.LoadContent(_content);
                _fireballs.Add(fireball);
                FireBallShoot.Play();
            }
        }

        for (int i = _fireballs.Count - 1; i >= 0; i--)
        {
            FireballSprite fireball = _fireballs[i];
            fireball.Update(gameTime);

            bool collidedWithWizard = false;

            foreach (Entity mob in _mobs)
            {
                if (mob is not EvilWizard wizard || !fireball.CheckCollision(wizard))
                    continue;

                collidedWithWizard = true;

                var attack = new AttackRoll(
                    Player.Name,
                    wizard.Name,
                    wizard.AC,
                    new (int, string)[] { (10, "Fireball accuracy") });

                _latestAttackLog = attack.Log;

                if (attack.IsHit)
                {
                    int damageRoll = Dice.D20();
                    uint damage = (uint)(damageRoll + 10);

                    wizard.TakeTrueDamage(damage);

                    _latestAttackLog +=
                        $" \nDamage: d20 ({damageRoll}) + 10 = {damage} true damage. " +
                        $"\nRemaining HP: {wizard.HP}.";

                    if (!wizard.IsAlive)
                        Death.Play();
                }

                break;
            }

            if (fireball.Destroyed)
            {
                if (collidedWithWizard)
                    FireBallHit.Play();

                _fireballs.RemoveAt(i);
            }
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.DrawString(
    _attackLogFont,
    _latestAttackLog,
    new Vector2(10, 10),
    Color.White,
    0f,
    Vector2.Zero,
    0.7f,
    SpriteEffects.None,
    0f);
        if (Player.IsAlive)
            Player.Sprite.Draw(GameTime, spriteBatch);

        foreach (Entity mob in _mobs)
        {
            if (mob.IsAlive)
                mob.Sprite.Draw(GameTime, spriteBatch);
        }

        foreach (FireballSprite fireball in _fireballs)
            fireball.Draw(GameTime, spriteBatch);
    }
}