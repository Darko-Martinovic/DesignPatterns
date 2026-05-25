using DesignPatterns.Infrastructure;

namespace DesignPatterns.Patterns.Decorator;

public sealed class DecoratorDemo : IPatternDemo
{
    public string Title => "Decorator";
    public string Summary => "add behavior by wrapping — instead of a subclass for every combination";

    public void Run()
    {
        ConsoleEx.Banner("DECORATOR");
        ConsoleEx.Info("Intent: add responsibilities to an object dynamically by wrapping it,");
        ConsoleEx.Info("keeping the same interface — a flexible alternative to subclassing.");

        ShowWrongApproach();
        ShowDecorator();
        Takeaways();

        ConsoleEx.Pause();
    }

    private static void ShowWrongApproach()
    {
        ConsoleEx.Section("The wrong approach: a subclass per combination");
        ConsoleEx.Code("class EspressoWithMilk { ... }\nclass EspressoWithMilkAndMocha { ... }\nclass EspressoWithMilkMochaAndWhip { ... }   // ... and on, and on");

        ConsoleEx.Bad("Behavior is frozen at compile time — every mix needs its own class.");
        Console.WriteLine();
        ConsoleEx.WriteLine("    How fast it explodes (baseDrinks x 2^condiments):", ConsoleColor.Gray);
        foreach (var condiments in new[] { 3, 4, 6, 8 })
        {
            var count = ExplosionMath.ClassCount(baseDrinks: 2, condiments);
            ConsoleEx.Bullet($"2 drinks x {condiments} condiments  ->  {count,4} classes to hand-write & maintain");
        }
        ConsoleEx.Bullet("...and pricing logic is copy-pasted into every one of them.");
    }

    private static void ShowDecorator()
    {
        ConsoleEx.Section("The decorator approach: compose at runtime");
        ConsoleEx.Code("IBeverage drink = new Espresso();\ndrink = new Milk(drink);\ndrink = new Mocha(drink);   // each wrap adds one responsibility");

        ConsoleEx.WriteLine("\n    Building drinks by wrapping — no new classes needed:", ConsoleColor.Gray);

        IBeverage plain = new Espresso();
        Show(plain);

        IBeverage latteish = new Milk(new Espresso());
        Show(latteish);

        IBeverage fancy = new Whip(new Mocha(new Milk(new Espresso())));
        Show(fancy);

        // Double mocha: just wrap twice. Impossible to express cleanly with subclasses.
        IBeverage doubleMocha = new Mocha(new Mocha(new HouseBlend()));
        Show(doubleMocha);

        ConsoleEx.Good("Each condiment is ONE small class; any combination is built by wrapping.");
        ConsoleEx.Good("Want it twice? Wrap it twice. The set of combinations is open-ended and free.");
    }

    private static void Show(IBeverage beverage)
    {
        ConsoleEx.Write("    • ", ConsoleColor.DarkGray);
        ConsoleEx.Write(beverage.Description.PadRight(42), ConsoleColor.White);
        ConsoleEx.WriteLine($"{beverage.Cost,6:C}", ConsoleColor.Green);
    }

    private static void Takeaways()
    {
        ConsoleEx.Section("Takeaways");
        ConsoleEx.Good("Favor composition over inheritance: wrap to add behavior, don't subclass per case.");
        ConsoleEx.Info("Decorator and wrapped object share an interface, so wrapping is transparent to callers.");
        ConsoleEx.Info("Real .NET examples: System.IO streams (BufferedStream, GZipStream, CryptoStream),");
        ConsoleEx.Info("and ASP.NET Core middleware — each wraps the next in the pipeline.");
        ConsoleEx.Warn("Cost: many small objects and deep wrapping can be harder to debug than one class.");
    }
}
