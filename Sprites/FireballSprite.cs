using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Dungeon.Collisions;
using Dungeon.Objects;

namespace Dungeon.Sprites;

public class FireballSprite
{
    private Texture2D texture;

    private double animationTimer;
    private short animationFrame = 0;

    private Vector2 velocity;

    private BoundingCircle bounds;

    public Vector2 Position;
    public float Direction;

    public bool Destroyed { get; private set; }

    public BoundingCircle Bounds => bounds;

    public FireballSprite(Vector2 position, Vector2 direction)
    {
        Position = position;

        velocity = direction * 300f;

        Direction = (float)Math.Atan2(
            direction.Y,
            direction.X) + MathHelper.PiOver2;

        bounds = new BoundingCircle(
            Position,
            16);
    }
    public bool CheckCollision(Entity target)
    {
        if (Destroyed || !target.IsAlive || !bounds.CollidesWith(target.Sprite.HitBox))
            return false;

        Hit();
        return true;
    }

    public void LoadContent(ContentManager content)
    {
        texture = content.Load<Texture2D>("Textures/FireballSheet");
    }

    public void Update(GameTime gameTime)
    {
        float Time =
            (float)gameTime.ElapsedGameTime.TotalSeconds;

        Position += velocity * Time;

        bounds.Center = Position;

        animationTimer += gameTime.ElapsedGameTime.TotalSeconds;

        if (animationTimer >= 0.1)
        {
            animationFrame++;

            if (animationFrame >= 3)
                animationFrame = 0;

            animationTimer -= 0.1;
        }

        if (Position.X < 0 ||
            Position.X > 1280 ||
            Position.Y < 0 ||
            Position.Y > 720)
        {
            Destroyed = true;
        }
    }

    public void Hit()
    {
        Destroyed = true;
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        if (Destroyed)
            return;

        Rectangle source = new Rectangle(
            animationFrame * 32,
            0,
            32,
            32);

        Vector2 origin = new Vector2(16, 16);

        spriteBatch.Draw(
            texture,
            Position,
            source,
            Color.White,
            Direction,
            origin,
            2.0f,
            SpriteEffects.None,
            0f);
    }
}