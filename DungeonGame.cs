using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Dungeon.Collisions;
using Dungeon.Input;
using System;

namespace Dungeon;

public class DungeonGame : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private ScreenManager _screenManager;
    private InputState _inputState;
    public DungeonGame()
    {
        _graphics = new GraphicsDeviceManager(this);

        var displayMode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
        _graphics.PreferredBackBufferWidth = displayMode.Width;
        _graphics.PreferredBackBufferHeight = displayMode.Height;
        _graphics.IsFullScreen = true;

        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        _inputState = new InputState(this);
        Components.Add(_inputState);
        Services.AddService<IInputState>(_inputState);
    }

    protected override void Initialize()
    {
        _screenManager = new ScreenManager();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _screenManager.ChangeScreen(new DemoScreen(_inputState, _screenManager, _spriteBatch, GraphicsDevice, this), Content);
    }

    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (_inputState.IsNewKeyPress(Keys.Escape))
        {
            Exit();
            return;
        }

        _screenManager.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        _screenManager.Draw(_spriteBatch);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}