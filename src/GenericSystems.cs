public static class GenericSystems
{
    public static void LogPositions(EngineContext ctx)
    {
        var qry = ctx.Query<Position, Velocity>();

        foreach (var (position, velocity) in qry)
        {
            position.x += velocity.x;
            position.y += velocity.y;
        }
    }
}