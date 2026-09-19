using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Dungeon;

public class WizardSprite
{
    private Vector2 position;

    private Texture2D texture;
    public Vector2 Position => position;

    public WizardSprite(Vector2 position)
    {
        this.position = position;
    }

    public void LoadContent(ContentManager content)
    {
        texture = content.Load<Texture2D>("Wizard");
    }
    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(
            texture,
            position,
            null,
            Color.White,
            0f,
            Vector2.Zero,
            2.0f,
            SpriteEffects.None,
            0f);
    }
}
