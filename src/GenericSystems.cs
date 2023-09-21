public static class GenericSystems
{
    public static void MovePositions(Ctx ctx)
    {
        // var qry = ctx.Query<Position, Velocity>();
        var qry = ctx.Query(typeof(Position), typeof(Velocity));

        foreach (var group in qry)
        {
            var position = (Position)group[0];
            var velocity = (Velocity)group[1];

            position.x += velocity.x;
            position.y += velocity.y;
        }
    }
}