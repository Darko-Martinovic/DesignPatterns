namespace DesignPatterns.Patterns.Factory;

// ===========================================================================
// Shared products used by both the "simple factory" and the Factory Method.
// ===========================================================================

public interface ITransport
{
    string Deliver(string cargo);
}

public sealed class Truck : ITransport
{
    public string Deliver(string cargo) => $"Delivering '{cargo}' by ROAD in a truck.";
}

public sealed class Ship : ITransport
{
    public string Deliver(string cargo) => $"Delivering '{cargo}' by SEA in a container ship.";
}

// ===========================================================================
// ⚠ "Simple Factory" — NOT a GoF pattern, just a helper. Useful, but it does
// not scale: every new product forces you to crack open this switch and
// recompile it. That is an Open/Closed Principle violation.
// ===========================================================================

public enum TransportKind
{
    Truck,
    Ship,
}

public static class SimpleTransportFactory
{
    public static ITransport Create(TransportKind kind) => kind switch
    {
        TransportKind.Truck => new Truck(),
        TransportKind.Ship => new Ship(),
        // To add Drone/Train you must EDIT this method — existing, tested code
        // has to change. That's the smell the Factory Method pattern removes.
        _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unknown transport"),
    };
}
