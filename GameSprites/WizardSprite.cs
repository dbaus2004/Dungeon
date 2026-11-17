using System;
using System.Collections.Generic;
using System.Text;
using Dungeon.Collisions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Dungeon.ParticleSystem;

namespace Dungeon.Sprites;

public class WizardSprite : ISprite, IScalableSprite
{
    private Vector2 position;

    private Texture2D texture;
    public Vector2 Position
    {
        get { return position; }
        set { position = value; }
    }
    public Vector2 DisplaySize { get; set; } = new Vector2(16, 16);

    public Vector2 Center => Position + DisplaySize / 2f;

    public Bounds HitBox =>
        new BoundingRectangle(position.X, position.Y, DisplaySize.X, DisplaySize.Y);

    public WizardSprite(Vector2 position)
    {
        this.position = position;
    }

    public bool TryShootAt(
        IParticleEmitter explosion,
        Vector2 target,
        out FireballSprite fireball
    )
    {
        Vector2 direction = target - Center;

        if (direction.LengthSquared() == 0)
        {
            fireball = null;
            return false;
        }

        fireball = new FireballSprite(explosion, Center, target);
        return true;
    }

    public void LoadContent(ContentManager content)
    {
        texture = content.Load<Texture2D>("Textures/Wizard");
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        Vector2 scale = new Vector2(DisplaySize.X / texture.Width, DisplaySize.Y / texture.Height);

        spriteBatch.Draw(
            texture,
            position,
            null,
            Color.White,
            0f,
            Vector2.Zero,
            scale,
            SpriteEffects.None,
            0f
        );
    }
}
