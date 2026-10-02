using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Dungeon.Input;

namespace Dungeon;
public abstract class GameScreen
{
    public virtual void LoadContent() { }
    public virtual void UnloadContent() { }
    public virtual void LoadContent(ContentManager content) { }
    public virtual void Update(GameTime gameTime) { }
    public virtual void Update(GameTime gameTime, InputState inputState) { }
    public virtual void Draw(SpriteBatch spriteBatch) { }
}
