using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Dungeon.Collisions;

namespace Dungeon.Sprites;

public class WizardSprite : ISprite
{
    private Vector2 position;

    private Texture2D texture;
    public Vector2 Center
    {
        get
        {
            return Position + new Vector2(32, 32);
        }
    }
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
            return new BoundingRectangle(position.X + 16, position.Y, 32, 64);
        }
    }

    public WizardSprite(Vector2 position)
    {
        this.position = position;
    }

    public bool TryShootAt(Vector2 target, out FireballSprite fireball)
    {
        Vector2 direction = target - Center;

        if (direction.LengthSquared() == 0)
        {
            fireball = null;
            return false;
        }

        fireball = new FireballSprite(Center, Vector2.Normalize(direction));
        return true;
    }

    public void LoadContent(ContentManager content)
    {
        texture = content.Load<Texture2D>("Textures/Wizard");
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
