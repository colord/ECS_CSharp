public static class Orb
{
    public static void Startup(EngineContext ctx)
    {
        ctx.AddEntity(
            new Position { x = 0, y = 0 },
            new Velocity { x = 1, y = 1 },
            new Circle { radius = 10f, color = Color.RED }
        );

        ctx.AddEntity(
            new Position { x = 100, y = 0 },
            new Velocity { x = 1, y = 5 },
            new Circle { radius = 100f, color = Color.GREEN }
        );

        ctx.AddEntity(
            new Position { x = 0, y = 0 },
            new Velocity { x = 11, y = 2 },
            new Circle { radius = 20f, color = Color.PINK }
        );

        WriteLine("orb -> o");
    }

    public static void DrawOrbs(EngineContext ctx)
    {
        var qry = ctx.Query<Position, Circle>();

        foreach (var (position, circle) in qry)
        {
            Raylib.DrawCircle((int)position.x, (int)position.y, circle.radius, circle.color);
        }
    }
}