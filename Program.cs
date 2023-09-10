new Engine()
    // --- Startup Systems ---
    .AddStartupSystems(Orb.Startup)
    // --- Update Systems ---
    .AddUpdateSystems(Orb.DrawOrbs)
    .AddUpdateSystems(GenericSystems.LogPositions)
    // --- Run ---
    .Run();