global using Raylib_cs;
using System.Collections;

public class EntityQuery<T, U> : IEnumerable<T> where T : U
{
    private List<U> items = new List<U>();
    public EntityQuery(List<U> list)
    {
        this.items = list;
    }
    public IEnumerator<T> GetEnumerator()
    {
        foreach (var item in items)
        {
            yield return (T)item!;
        }
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class EngineContext
{
    private uint currentEntityID = 0;
    public Dictionary<Type, List<Component>> componentArrays = new();
    public void AddEntity(params Component[] components)
    {
        foreach (Component component in components)
        {
            component.entityID = this.currentEntityID;
            var type = component.GetType();
            if (this.componentArrays.ContainsKey(type))
            {
                this.componentArrays[type].Add(component);
            }
            else
            {
                this.componentArrays[type] = new() { component };
            }
        }
        this.currentEntityID++;
    }
    public EntityQuery<T, Component> Query<T>() where T : Component
    {
        return new EntityQuery<T, Component>(this.componentArrays[typeof(T)]);
    }
    public IEnumerable<(T1, T2)> Query<T1, T2>()
        where T1 : Component
        where T2 : Component
    {
        var components1 = this.componentArrays[typeof(T1)];
        var components2 = this.componentArrays[typeof(T2)];

        var joined = components1.Join(
            components2,
            comp => comp.entityID,
            comp => comp.entityID,
            (comp1, comp2) => ((T1)comp1, (T2)comp2)
        );

        return joined;
    }
    public IEnumerable<(T1, T2, T3)> Query<T1, T2, T3>()
        where T1 : Component
        where T2 : Component
        where T3 : Component
    {
        var components1 = this.componentArrays[typeof(T1)];
        var components2 = this.componentArrays[typeof(T2)];
        var components3 = this.componentArrays[typeof(T3)];

        var joined = components1
        .Join(
            components2,
            comp => comp.entityID,
            comp => comp.entityID,
            (comp1, comp2) => ((T1)comp1, (T2)comp2)
        ).Join(
            components3,
            combined => combined.Item1.entityID,
            comp => comp.entityID,
            (combined, comp3) => (combined.Item1, combined.Item2, (T3)comp3)
        );

        return joined;
    }
}

public class Engine
{
    private EngineContext context;
    private List<SystemFunction> startupSystems = new();
    private List<SystemFunction> updateSystems = new();
    public delegate void SystemFunction(EngineContext ctx);
    public Engine()
    {
        this.context = new EngineContext();
    }
    public Engine AddStartupSystems(params SystemFunction[] systems)
    {
        this.startupSystems.AddRange(systems);
        return this;
    }
    private void RunStartupSystems()
    {
        foreach (var func in this.startupSystems)
        {
            func(this.context);
        }
    }
    public Engine AddUpdateSystems(params SystemFunction[] systems)
    {
        this.updateSystems.AddRange(systems);
        return this;
    }
    private void RunUpdateSystems()
    {
        foreach (var func in this.updateSystems)
        {
            func(this.context);
        }
    }
    public void Run()
    {
        Raylib.InitWindow(Constants.WINDOW_WIDTH, Constants.WINDOW_HEIGHT, Constants.WINDOW_TITLE);
        Raylib.SetTargetFPS(Constants.TARGET_FPS);

        this.RunStartupSystems();

        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Constants.BACKGROUND_COLOR);
            this.RunUpdateSystems();
            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }
}