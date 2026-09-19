using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Dungeon.Collisions;
using System;

namespace Dungeon;

public class DungeonGame : Game
{
    private bool _showHitBoxes = false;
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private EvilWizardSprite[] _evilWizards;
    private WizardSprite _wizard;
    private List<FireballSprite> _fireballs;

    private SpriteFont _spriteFont;
    private Texture2D _hitboxTexture;
    private Texture2D _circleTexture;

    private MouseState _previousMouseState;

    public DungeonGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        Random rand = new();

        _evilWizards =
        [
            new EvilWizardSprite(new Vector2(
                (float)rand.NextDouble() * GraphicsDevice.Viewport.Width,
                (float)rand.NextDouble() * GraphicsDevice.Viewport.Height)),

            new EvilWizardSprite(new Vector2(
                (float)rand.NextDouble() * GraphicsDevice.Viewport.Width,
                (float)rand.NextDouble() * GraphicsDevice.Viewport.Height)),

            new EvilWizardSprite(new Vector2(
                (float)rand.NextDouble() * GraphicsDevice.Viewport.Width,
                (float)rand.NextDouble() * GraphicsDevice.Viewport.Height)),

            new EvilWizardSprite(new Vector2(
                (float)rand.NextDouble() * GraphicsDevice.Viewport.Width,
                (float)rand.NextDouble() * GraphicsDevice.Viewport.Height)),

            new EvilWizardSprite(new Vector2(
                (float)rand.NextDouble() * GraphicsDevice.Viewport.Width,
                (float)rand.NextDouble() * GraphicsDevice.Viewport.Height)),

            new EvilWizardSprite(new Vector2(
                (float)rand.NextDouble() * GraphicsDevice.Viewport.Width,
                (float)rand.NextDouble() * GraphicsDevice.Viewport.Height)),

            new EvilWizardSprite(new Vector2(
                (float)rand.NextDouble() * GraphicsDevice.Viewport.Width,
                (float)rand.NextDouble() * GraphicsDevice.Viewport.Height))
        ];

        _wizard = new WizardSprite(
            new Vector2(
                GraphicsDevice.Viewport.Width / 2,
                GraphicsDevice.Viewport.Height / 2));

        _fireballs = new List<FireballSprite>();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _hitboxTexture = new Texture2D(GraphicsDevice, 1, 1);
        _hitboxTexture.SetData(new[] { Color.White });

        foreach (var eWiz in _evilWizards)
            eWiz.LoadContent(Content);

        _wizard.LoadContent(Content);

        int size = 32;
        _circleTexture = new Texture2D(GraphicsDevice, size, size);

        Color[] data = new Color[size * size];

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = x - size / 2f;
                float dy = y - size / 2f;

                if (dx * dx + dy * dy <= (size / 2f) * (size / 2f))
                    data[y * size + x] = Color.White;
                else
                    data[y * size + x] = Color.Transparent;
            }
        }

_circleTexture.SetData(data);

        _spriteFont = Content.Load<SpriteFont>("arial");
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        MouseState mouse = Mouse.GetState();

        if (mouse.LeftButton == ButtonState.Pressed &&
            _previousMouseState.LeftButton == ButtonState.Released)
        {
            Vector2 target = new Vector2(mouse.X, mouse.Y);
            Vector2 direction = target - _wizard.Position;

            if (direction != Vector2.Zero)
            {
                direction.Normalize();

                FireballSprite fireball =
                    new FireballSprite(
                        _wizard.Position + new Vector2(32, 32),
                        direction);

                fireball.LoadContent(Content);

                _fireballs.Add(fireball);
            }
        }

        foreach (var fireball in _fireballs)
        {
            fireball.Update(gameTime);
        }

        foreach (var fireball in _fireballs)
        {
            foreach (var wizard in _evilWizards)
            {
                if (!fireball.Destroyed &&
                    !wizard.Dead &&
                    fireball.Bounds.CollidesWith(wizard.Bounds))
                {
                    fireball.Hit();
                    wizard.Hit();
                }
            }
        }

        _fireballs.RemoveAll(f => f.Destroyed);

        _previousMouseState = mouse;

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();

        foreach (var fireball in _fireballs)
        {
            fireball.Draw(gameTime, _spriteBatch);
        }

        foreach (var eWiz in _evilWizards)
        {
            eWiz.Draw(gameTime, _spriteBatch);
        }

        _wizard.Draw(gameTime, _spriteBatch);

        foreach (var wizard in _evilWizards)
        {
            if (!wizard.Dead)
            {
                Rectangle rectangle = new Rectangle(
                    (int)wizard.Bounds.Left,
                    (int)wizard.Bounds.Top,
                    (int)wizard.Bounds.Width,
                    (int)wizard.Bounds.Height);
                if(_showHitBoxes){
                    _spriteBatch.Draw(
                        _hitboxTexture,
                        rectangle,
                        Color.Red * 0.5f);
                }
            }
        }

        foreach (var fireball in _fireballs)
        {
            if (!fireball.Destroyed)
            {
                Rectangle rectangle = new Rectangle(
                    (int)(fireball.Bounds.Center.X - fireball.Bounds.Radius),
                    (int)(fireball.Bounds.Center.Y - fireball.Bounds.Radius),
                    (int)(fireball.Bounds.Radius * 2),
                    (int)(fireball.Bounds.Radius * 2));

                if(_showHitBoxes){
                        _spriteBatch.Draw(
                        _circleTexture,
                        fireball.Bounds.Center,
                        null,
                        Color.Blue * 0.5f,
                        0f,
                        new Vector2(16, 16),
                        fireball.Bounds.Radius / 16f,
                        SpriteEffects.None,
                        0f);
                }
            }
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}