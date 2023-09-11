global using Raylib_cs;
using System.Collections;
using System.ComponentModel;

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
    private Dictionary<Type, object> resources = new();

    public EngineContext(Dictionary<Type, object> resources)
    {
        this.resources = resources;
    }

    public T? GetResource<T>()
    {
        if (resources.TryGetValue(typeof(T), out var resource))
            return (T) resource;
        return default;
    }

    public void AddEntity(params Component?[] components)
    {
        foreach (var component in components)
        {
            if (component == null) continue;

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
    public List<T> Query<T>() where T : Component
    {
        if (!this.componentArrays.TryGetValue(typeof(T), out var components)) return new List<T>();
        return new EntityQuery<T, Component>(components).ToList();
    }
    public List<(T1, T2)> Query<T1, T2>()
        where T1 : Component
        where T2 : Component
    {
        if (!this.componentArrays.TryGetValue(typeof(T1), out var components1) ||
            !this.componentArrays.TryGetValue(typeof(T2), out var components2)) return new List<(T1, T2)>();

        var joinedList = from c1 in components1
                         join c2 in components2 on c1.entityID equals c2.entityID
                         select
                         (
                             (T1)c1,
                             (T2)c2
                         );

        return joinedList.ToList();
    }
    public List<(T1, T2, T3)> Query<T1, T2, T3>()
        where T1 : Component
        where T2 : Component
        where T3 : Component
    {
        if (!this.componentArrays.TryGetValue(typeof(T1), out var components1) ||
            !this.componentArrays.TryGetValue(typeof(T2), out var components2) ||
            !this.componentArrays.TryGetValue(typeof(T3), out var components3)) return new List<(T1, T2, T3)>();

        var joinedList = from c1 in components1
                         join c2 in components2 on c1.entityID equals c2.entityID
                         join c3 in components3 on c1.entityID equals c3.entityID
                         select
                         (
                             (T1)c1,
                             (T2)c2,
                             (T3)c3
                         );

        return joinedList.ToList();
    }
    public List<(T1, T2, T3, T4)> Query<T1, T2, T3, T4>()
        where T1 : Component
        where T2 : Component
        where T3 : Component
        where T4 : Component
    {
        if (!this.componentArrays.TryGetValue(typeof(T1), out var components1) ||
            !this.componentArrays.TryGetValue(typeof(T2), out var components2) ||
            !this.componentArrays.TryGetValue(typeof(T3), out var components3) ||
            !this.componentArrays.TryGetValue(typeof(T4), out var components4)) return new List<(T1, T2, T3, T4)>();

        var joinedList = from c1 in components1
                         join c2 in components2 on c1.entityID equals c2.entityID
                         join c3 in components3 on c1.entityID equals c3.entityID
                         join c4 in components4 on c1.entityID equals c4.entityID
                         select
                         (
                             (T1)c1,
                             (T2)c2,
                             (T3)c3,
                             (T4)c4
                         );

        return joinedList.ToList();
    }
}

public class Engine
{
    private EngineContext context;
    private List<SystemFunction> startupSystems = new();
    private List<SystemFunction> updateSystems = new();
    private Dictionary<Type, object> resources = new();
    public delegate void SystemFunction(EngineContext ctx);
    public Engine()
    {
        this.context = new EngineContext(resources);
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
    public Engine AddResources(params object[] resources)
    {
        foreach (var resource in resources)
        {
            this.resources[resource.GetType()] = resource;
        }

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
            Raylib.DrawFPS(10, 10);
            this.RunUpdateSystems();
            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }
}