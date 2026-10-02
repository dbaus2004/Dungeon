using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Dungeon.Collisions;

namespace Dungeon.Sprites;

public class NoSprite: ISprite
{
    private Vector2 position;

    private Texture2D texture;
    public Vector2 Position
    {
        get
        {
            return position;
        }
        set
        {
            position = value;
        }
    }
    public Bounds HitBox
    {
        get
        {
            return new BoundingRectangle(position.X, position.Y, 64, 64);
        }
    }

    public NoSprite(Vector2 position)
    {
        this.position = position;
    }

    public void LoadContent(ContentManager content)
    {
        texture = content.Load<Texture2D>("Textures/NoTexture");
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
