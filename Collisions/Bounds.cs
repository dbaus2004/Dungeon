namespace Dungeon.Collisions;

public abstract class Bounds
{
    public abstract bool CollidesWith(BoundingRectangle other);
    public abstract bool CollidesWith(BoundingCircle other);
    public abstract bool CollidesWith(Bounds other);
}