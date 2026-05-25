namespace DesignPatterns.Patterns.Factory;

// ===========================================================================
// ✅ FACTORY METHOD (GoF)
//
// Define an interface for creating an object, but let SUBCLASSES decide which
// class to instantiate. The base class holds the workflow that depends on the
// product; subclasses only override the single "which product?" decision.
//
// Adding a new transport = add a new Logistics subclass. No existing class is
// edited — that is the Open/Closed win over the simple switch-factory above.
// ===========================================================================

public abstract class Logistics
{
    /// <summary>The factory method: subclasses decide the concrete transport.</summary>
    protected abstract ITransport CreateTransport();

    /// <summary>
    /// The reason the pattern exists: shared business logic lives here in the
    /// creator and depends only on the <see cref="ITransport"/> abstraction.
    /// </summary>
    public string Plan(string cargo)
    {
        var transport = CreateTransport();   // <-- defer the concrete type to subclass
        return transport.Deliver(cargo);
    }
}

public sealed class RoadLogistics : Logistics
{
    protected override ITransport CreateTransport() => new Truck();
}

public sealed class SeaLogistics : Logistics
{
    protected override ITransport CreateTransport() => new Ship();
}
