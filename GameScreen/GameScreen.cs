using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Dungeon;

public abstract class GameScreen
{
    public virtual void UnloadContent() { }
    public virtual void LoadContent(ContentManager content) { }
    public virtual void OnActivated() { }
    public virtual void OnDeactivated() { }
    public virtual void Update(GameTime gameTime) { }
    public virtual bool HandleCancel() => false;
    public virtual void Draw(SpriteBatch spriteBatch) { }
    public virtual void DrawOverlay(SpriteBatch spriteBatch) { }
}
