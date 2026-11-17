using Microsoft.Xna.Framework;

namespace Dungeon.ParticleSystem
{
    public interface IParticleEmitter
    {
        void Emit(Vector2 position);
    }
}