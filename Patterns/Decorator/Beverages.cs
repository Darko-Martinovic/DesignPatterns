namespace DesignPatterns.Patterns.Decorator;

// ===========================================================================
// ✅ THE DECORATOR PATTERN (GoF)
//
// Attach responsibilities to an object dynamically by WRAPPING it. Decorators
// share the component's interface, so a decorated object is still "an
// IBeverage" and can be wrapped again. Behavior is composed at runtime instead
// of fixed by a class hierarchy — a flexible alternative to subclassing.
// ===========================================================================

/// <summary>The component interface shared by drinks and condiments alike.</summary>
public interface IBeverage
{
    string Description { get; }
    decimal Cost { get; }
}

// --- Concrete components (the base drinks) ---

public sealed class Espresso : IBeverage
{
    public string Description => "Espresso";
    public decimal Cost => 1.90m;
}

public sealed class HouseBlend : IBeverage
{
    public string Description => "House Blend";
    public decimal Cost => 1.40m;
}

// --- The decorator base: IS-A beverage and HAS-A wrapped beverage ---

public abstract class CondimentDecorator(IBeverage inner) : IBeverage
{
    protected readonly IBeverage Inner = inner;
    public abstract string Description { get; }
    public abstract decimal Cost { get; }
}

// --- Concrete decorators: each adds to the wrapped beverage's behavior ---

public sealed class Milk(IBeverage inner) : CondimentDecorator(inner)
{
    public override string Description => $"{Inner.Description} + Milk";
    public override decimal Cost => Inner.Cost + 0.30m;
}

public sealed class Mocha(IBeverage inner) : CondimentDecorator(inner)
{
    public override string Description => $"{Inner.Description} + Mocha";
    public override decimal Cost => Inner.Cost + 0.50m;
}

public sealed class Whip(IBeverage inner) : CondimentDecorator(inner)
{
    public override string Description => $"{Inner.Description} + Whip";
    public override decimal Cost => Inner.Cost + 0.40m;
}
