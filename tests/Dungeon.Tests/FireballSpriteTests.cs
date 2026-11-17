using Dungeon.ParticleSystem;
using Dungeon.Sprites;
using Microsoft.Xna.Framework;

namespace Dungeon.Tests;

public class FireballSpriteTests
{
    [Fact]
    public void FireballRemainsUntilItReachesItsDestination()
    {
        var explosion = new RecordingEmitter();
        var fireball = new FireballSprite(
            explosion,
            new Vector2(10, 10),
            new Vector2(1210, 10)
        );

        fireball.Update(new GameTime(TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(100)));

        Assert.False(fireball.Destroyed);
        Assert.Equal(70f, fireball.Position.X);
        Assert.Equal(0, explosion.EmissionCount);
    }

    [Fact]
    public void FireballEmitsExplosionAndIsDestroyedOnArrival()
    {
        var explosion = new RecordingEmitter();
        var fireball = new FireballSprite(
            explosion,
            new Vector2(10, 10),
            new Vector2(70, 10)
        );

        fireball.Update(new GameTime(TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(100)));

        Assert.True(fireball.Destroyed);
        Assert.Equal(new Vector2(70, 10), fireball.Position);
        Assert.Equal(1, explosion.EmissionCount);
        Assert.Equal(new Vector2(70, 10), explosion.LastPosition);
    }

    private sealed class RecordingEmitter : IParticleEmitter
    {
        public int EmissionCount { get; private set; }
        public Vector2 LastPosition { get; private set; }

        public void Emit(Vector2 position)
        {
            EmissionCount++;
            LastPosition = position;
        }
    }
}
