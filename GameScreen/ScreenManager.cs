using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Dungeon;

public class ScreenManager
{
    private GameScreen _currentScreen;
    private readonly Stack<GameScreen> _previousScreens = new Stack<GameScreen>();
    private ContentManager _content;

    public void ChangeScreen(GameScreen newScreen, ContentManager content)
    {
        ArgumentNullException.ThrowIfNull(newScreen);
        ArgumentNullException.ThrowIfNull(content);

        _currentScreen?.UnloadContent();
        foreach (GameScreen previousScreen in _previousScreens)
            previousScreen.UnloadContent();

        _previousScreens.Clear();
        _content = content;
        _currentScreen = newScreen;
        _currentScreen.LoadContent(content);
        _currentScreen.OnActivated();
    }

    public void Update(GameTime gameTime)
    {
        _currentScreen?.Update(gameTime);
    }

    public bool HandleCancel()
    {
        return _currentScreen?.HandleCancel() ?? false;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _currentScreen?.Draw(spriteBatch);
    }

    public void DrawOverlay(SpriteBatch spriteBatch)
    {
        _currentScreen?.DrawOverlay(spriteBatch);
    }

    public void PushScreen(GameScreen newScreen)
    {
        ArgumentNullException.ThrowIfNull(newScreen);

        if (_currentScreen == null || _content == null)
            throw new InvalidOperationException("Cannot push a screen before the initial screen is loaded.");

        _currentScreen.OnDeactivated();
        _previousScreens.Push(_currentScreen);
        _currentScreen = newScreen;
        _currentScreen.LoadContent(_content);
        _currentScreen.OnActivated();
    }

    public void PopScreen()
    {
        if (_previousScreens.Count == 0)
            throw new InvalidOperationException("There is no previous screen to return to.");

        _currentScreen?.UnloadContent();
        _currentScreen = _previousScreens.Pop();
        _currentScreen.OnActivated();
    }
}