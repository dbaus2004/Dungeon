using Microsoft.Xna.Framework;

namespace Dungeon.Collisions;

public struct BoundingCircle
{
    public Vector2 Center;
    public float Radius;

    public BoundingCircle(Vector2 center, float radius)
    {
        Center = center;
        Radius = radius;
    }

    public bool CollidesWith(BoundingCircle other)
    {
        return CollisionHelper.Collides(this, other);
    }

    public bool CollidesWith(BoundingRectangle other)
    {
        return CollisionHelper.Collides(this, other);
    }
}