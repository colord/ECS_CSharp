// Put this here because I guess it should go in a "global usings" file.
// Couldn't put it right above EngineContext class because the class wasn't defined yet
// and I didn't want to just put it under it
// and I also didn't want to put it in a random file.
global using EC = EngineContext;

new Engine()
    // --- Startup Systems ---
    .AddStartupSystems(Orb.Startup)
    // --- Update Systems ---
    .AddUpdateSystems(
        Orb.DrawOrbCounter,
        Orb.BounceOrbs,
        Orb.AddOrbOnClick,
        Orb.OrbGravity,
        Orb.DrawOrbs,
        GenericSystems.MovePositions
    )
    // --- Resources ---
    .AddResources(
        new OrbCounter()
    )
    // --- Run ---
    .Run();

/* NOTE: Here's my idea for resources

So, I was wondering what to do if I wanted to track the total amount of orbs spawned. (and also draw that number)
I could have a component for piece of text and a component notifying the engine that this text's value will be "Orbs: 100", for example.
But, do I also need that component to hold the current amount of orbs? Then I also need to query that component whenever I add/delete an orb.
So, I checked out what Bevy does and they use "resources".
Basically it's like a global component that you can access whenever you want in O(1) time.
So, anything that access the OrbCounter resource and add/subtract to it.
*/