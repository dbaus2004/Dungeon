namespace Dungeon.Collisions;

public class BoundingRectangle
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

    public bool CollidesWith(BoundingRectangle other)
    {
        return Left < other.Right &&
               Right > other.Left &&
               Top < other.Bottom &&
               Bottom > other.Top;
    }
}