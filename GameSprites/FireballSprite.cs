using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Dungeon.Collisions;
using Dungeon.ParticleSystem;
using Dungeon.Objects;

namespace Dungeon.Sprites;

public class FireballSprite
{
    private readonly FireballExplosion _explosion;
    private Texture2D texture;
    private double animationTimer;
    private short animationFrame = 0;
    private Vector2 velocity;
    private BoundingCircle bounds;
    public Vector2 Position;
    public Vector2 Destination;
    public float Direction;

    public bool Destroyed { get; private set; }

    public BoundingCircle Bounds => bounds;

    public FireballSprite(Game game, Vector2 position, Vector2 direction, Vector2 destination)
    {

        Position = position;

        Destination = destination;

        _explosion = new FireballExplosion(game, 1);
        game.Components.Add(_explosion);

        velocity = direction * 300f;

        Direction = (float)Math.Atan2(
            direction.Y,
            direction.X) + MathHelper.PiOver2;

        bounds = new BoundingCircle(
            Position,
            16);
    }

    public void LoadContent(ContentManager content)
    {
        texture = content.Load<Texture2D>("Textures/FireballSheet");
    }

    public void Update(GameTime gameTime)
    {
        float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
        float distanceToTravel = velocity.Length() * elapsed;
        Vector2 toDestination = Destination - Position;

        if (toDestination.Length() <= distanceToTravel)
        {
            Position = Destination;
            bounds.Center = Position;
            _explosion.PlaceEcplosion(Position);
            Destroyed = true;
            return;
        }
        Position += velocity * elapsed;
        bounds.Center = Position;

        animationTimer += gameTime.ElapsedGameTime.TotalSeconds;

        if (animationTimer >= 0.1)
        {
            animationFrame++;

            if (animationFrame >= 3)
                animationFrame = 0;

            animationTimer -= 0.1;
        }

        if (Position.X < 0 || Position.X > 1280 ||
            Position.Y < 0 || Position.Y > 720)
        {
            Destroyed = true;
        }
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