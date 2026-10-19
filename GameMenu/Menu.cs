
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Dungeon.Menu
{
    public abstract class Menu
    {
        public bool IsOpen { get; private set; }

        protected SpriteBatch SpriteBatch;
        protected SpriteFont Font;
        protected GraphicsDevice GraphicsDevice;

        protected Menu(
            SpriteBatch spriteBatch,
            SpriteFont font,
            GraphicsDevice graphicsDevice)
        {
            SpriteBatch = spriteBatch;
            Font = font;
            GraphicsDevice = graphicsDevice;
        }
        protected Menu(
            SpriteBatch spriteBatch,
            SpriteFont font,
            Texture2D pixel,
            GraphicsDevice graphicsDevice)
        {
            SpriteBatch = spriteBatch;
            Font = font;
            GraphicsDevice = graphicsDevice;
        }

        public void Open()
        {
            IsOpen = true;
            OnOpen();
        }

        public void Close()
        {
            IsOpen = false;
            OnClose();
        }

        public void Toggle()
        {
            if (IsOpen)
                Close();
            else
                Open();
        }

        public void Update(GameTime gameTime, KeyboardState keyboard)
        {
            if (!IsOpen)
                return;

            HandleInput(gameTime, keyboard);
        }

        public void Draw(GameTime gameTime)
        {
            if (!IsOpen)
                return;
            DrawBackground();
            DrawContent(gameTime);
        }

        protected virtual void OnOpen() { }
        protected virtual void OnClose() { }

        protected abstract void HandleInput(
            GameTime gameTime,
            KeyboardState keyboard);

        protected abstract void DrawContent(GameTime gameTime);

        protected virtual void DrawBackground()
        {
        }
    }
}
