namespace DesignPatterns.Patterns.Decorator;

// ===========================================================================
// ❌ THE WRONG APPROACH: model every option with a subclass.
//
// "A coffee can have milk and mocha" becomes one class per combination. With
// 2 base drinks and just 3 condiments you already need 2 * 2^3 = 16 classes,
// and every new condiment DOUBLES the count. Pricing logic gets copy-pasted
// into each one, so a price change means editing dozens of files.
//
// Below is only the start of the explosion — enough to feel the pain.
// ===========================================================================

public sealed class EspressoSubclass
{
    public string Description => "Espresso";
    public decimal Cost => 1.90m;
}

public sealed class EspressoWithMilk
{
    public string Description => "Espresso + Milk";
    public decimal Cost => 1.90m + 0.30m;
}

public sealed class EspressoWithMilkAndMocha
{
    public string Description => "Espresso + Milk + Mocha";
    public decimal Cost => 1.90m + 0.30m + 0.50m;
}

public sealed class EspressoWithMilkMochaAndWhip
{
    public string Description => "Espresso + Milk + Mocha + Whip";
    public decimal Cost => 1.90m + 0.30m + 0.50m + 0.40m;
}

// ...and we still owe: EspressoWithMocha, EspressoWithWhip, EspressoWithMochaAndWhip,
// then the entire HouseBlend* set, then every SoyMilk* variant — forever.

/// <summary>Just the arithmetic of how badly this scales.</summary>
public static class ExplosionMath
{
    public static long ClassCount(int baseDrinks, int condiments) =>
        baseDrinks * (long)Math.Pow(2, condiments);
}
