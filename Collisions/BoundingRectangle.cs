namespace Dungeon.Collisions;

public class BoundingRectangle: Bounds
{
    public float X;
    public float Y;
    public float Width;
    public float Height;

    public float Left => X;
    public float Right => X + Width;
    public float Top => Y;
    public float Bottom => Y + Height;

    public BoundingRectangle(float x, float y, float width, float height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
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