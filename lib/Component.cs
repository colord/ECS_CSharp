public class Component
{
    public uint entityID;
}

public class Position : Component
{
    public float x;
    public float y;
}

public class Velocity : Component
{
    public float x;
    public float y;
}

public class Circle : Component
{
    public float radius;
    public Color color;
}

public class CircleWallBounce : Component { }