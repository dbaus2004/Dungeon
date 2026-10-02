using Microsoft.Xna.Framework;

namespace Dungeon.Collisions;

public class BoundingCircle: Bounds
{
    public Vector2 Center;
    public float Radius;

    public BoundingCircle(Vector2 center, float radius)
    {
        Center = center;
        Radius = radius;
    }

    public override bool CollidesWith(BoundingRectangle other)
    {
        return CollisionHelper.Collides(this, other);
    }
    public override bool CollidesWith(BoundingCircle other)
    {
        return CollisionHelper.Collides(this, other);
    }
    public override bool CollidesWith(Bounds other)
    {
        if(other.GetType() == typeof(BoundingCircle)){
            return CollisionHelper.Collides(this, (BoundingCircle)other);
        }
        else if(other.GetType() == typeof(BoundingRectangle)){
            return CollisionHelper.Collides(this, (BoundingRectangle)other);
        }
        else
        {
            return false;
        }
    }
}