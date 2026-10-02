using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Dungeon.Collisions;

namespace Dungeon.Sprites
{

    public interface ISprite
    {
        public abstract Vector2 Position { get; set; }
        public abstract Bounds HitBox { get; }
        public abstract void LoadContent(ContentManager content);
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}