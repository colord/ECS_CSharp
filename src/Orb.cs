using System.Numerics;

public static class Orb
{
    public static readonly Color[] orbColors = { Color.LIME, Color.GREEN, Color.DARKGREEN };

    public static Texture2D orbTexture;

    public static void Startup(Ctx ctx)
    {
        // Just trying this out
        // I tested it and draw textures is at least twice as fast as drawing circles using DrawCircle()
        RenderTexture2D target = Raylib.LoadRenderTexture(40, 40);
        Raylib.BeginTextureMode(target);
        Raylib.ClearBackground(new Color(0, 0, 0, 0));
        Raylib.DrawCircle(target.texture.width / 2, target.texture.height / 2, 20, Color.WHITE);
        Raylib.EndTextureMode();
        orbTexture = target.texture;

        WriteLine("Orb Startup!");
    }

    public static void DrawOrbCounter(Ctx ctx)
    {
        var counter = ctx.GetResource<OrbCounter>();
        Raylib.DrawText($"Orbs: {counter!.count}", 10, 40, 20, Color.BLACK);
    }

    public static void DrawOrbs(Ctx ctx)
    {
        var qry = ctx.Query<Position, Circle>();

        // WriteLine($"Query: {qry}");

        foreach (var (position, circle) in qry)
        {
            //Raylib.DrawTexture(orbTexture, (int)position.x-20, (int)position.y-20, circle.color);
            //Raylib.DrawCircle((int)position.x, (int)position.y, circle.radius, circle.color);
            Rectangle sourceRect = new Rectangle(0, 0, orbTexture.width, orbTexture.height);
            Rectangle destRect = new Rectangle((int)position.x - circle.radius, (int)position.y - circle.radius, circle.radius * 2, circle.radius * 2);
            Raylib.DrawTexturePro(orbTexture, sourceRect, destRect, Vector2.Zero, 0, circle.color);
        }
    }

    public static void BounceOrbs(Ctx ctx)
    {
        var qry = ctx.Query<Position, Velocity, Circle, CircleWallBounce>();

        foreach (var (position, velocity, circle, _) in qry)
        {
            if (position.x < circle.radius)
            {
                position.x = circle.radius;
                velocity.x *= -1;
            }
            else if (position.y < circle.radius)
            {
                position.y = circle.radius;
                velocity.y *= -1;
            }
            else if (position.x > Constants.WINDOW_WIDTH - circle.radius)
            {
                position.x = Constants.WINDOW_WIDTH - circle.radius;
                velocity.x *= -1;
            }
            else if (position.y > Constants.WINDOW_HEIGHT - circle.radius)
            {
                position.y = Constants.WINDOW_HEIGHT - circle.radius;
                velocity.y *= -1;
            }
        }
    }

    public static void AddOrbOnClick(Ctx ctx)
    {
        if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
        {
            for (var i = 0; i < 1000; i++) AddOrb(ctx);
        }
    }

    public static void OrbGravity(Ctx ctx)
    {
        var qry = ctx.Query<Position, Velocity, CenterGravity>();

        foreach (var (position, velocity, _) in qry)
        {
            var toCenter = Math.Atan2((double)(Constants.WINDOW_HEIGHT / 2 - position.y), (double)(Constants.WINDOW_WIDTH / 2 - position.x));
            var magnitude = 1d;

            velocity.x += (float)(Math.Cos(toCenter) * magnitude);
            velocity.y += (float)(Math.Sin(toCenter) * magnitude);
        }
    }

    private static void AddOrb(Ctx ctx)
    {
        var counter = ctx.GetResource<OrbCounter>();
        counter!.count++;

        var mx = Raylib.GetMouseX();
        var my = Raylib.GetMouseY();
        var toCenter = Math.Atan2(Constants.WINDOW_HEIGHT / 2 - my, Constants.WINDOW_WIDTH / 2 - mx);
        var offAngle = toCenter + Math.PI / 2;
        var magnitude = 20d;

        ctx.AddEntity(
            //new Position { x = Raylib.GetRandomValue(0, Constants.WINDOW_WIDTH), y = Raylib.GetRandomValue(0, Constants.WINDOW_HEIGHT) },
            new Position {
                x = mx + Raylib.GetRandomValue(-100, 100),
                y = my + Raylib.GetRandomValue(-100, 100)
            },
            new Velocity
            {
                x = (float)(Math.Cos(offAngle) * magnitude),
                y = (float)(Math.Sin(offAngle) * magnitude)
            },
            new Circle {
                radius = Raylib.GetRandomValue(5, 20),
                color = orbColors[Raylib.GetRandomValue(0, orbColors.Count()-1)]
            },
            new CircleWallBounce()
            // new CenterGravity()
            //Raylib.GetRandomValue(0, 1) == 0 ? null : new CenterGravity()
        );
    }
}

// Used to keep track of the amount of orbs
public class OrbCounter
{
    public int count = 0;
}