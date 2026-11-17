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
    private readonly IParticleEmitter _explosion;
    private Texture2D texture;
    private double animationTimer;
    private short animationFrame = 0;
    private readonly Vector2 _velocity;
    private readonly BoundingCircle _bounds;
    public Vector2 Position { get; private set; }
    public Vector2 Destination { get; }
    public float Direction { get; }

    public bool Destroyed { get; private set; }

    public BoundingCircle Bounds => _bounds;

    public FireballSprite(
        IParticleEmitter explosion,
        Vector2 position,
        Vector2 destination
    )
    {
        _explosion = explosion ?? throw new ArgumentNullException(nameof(explosion));

        Position = position;

        Destination = destination;

        Vector2 toDestination = destination - position;
        if (toDestination.LengthSquared() == 0)
            throw new ArgumentException("Fireball destination must differ from its starting position.", nameof(destination));

        Vector2 direction = Vector2.Normalize(toDestination);
        _velocity = direction * 600f;

        Direction = (float)Math.Atan2(
            direction.Y,
            direction.X) + MathHelper.PiOver2;

        _bounds = new BoundingCircle(
            Position,
            16);
    }

    public void LoadContent(ContentManager content)
    {
        texture = content.Load<Texture2D>("Textures/FireballSheet");
    }

    public void Update(GameTime gameTime)
    {
        if (Destroyed)
            return;

        float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
        float distanceToTravel = _velocity.Length() * elapsed;
        Vector2 toDestination = Destination - Position;

        if (toDestination.Length() <= distanceToTravel)
        {
            Position = Destination;
            _bounds.Center = Position;
            _explosion.Emit(Position);
            Destroyed = true;
            return;
        }
        Position += _velocity * elapsed;
        _bounds.Center = Position;

        animationTimer += gameTime.ElapsedGameTime.TotalSeconds;

        if (animationTimer >= 0.1)
        {
            animationFrame++;

            if (animationFrame >= 3)
                animationFrame = 0;

            animationTimer -= 0.1;
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