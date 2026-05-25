using DesignPatterns.Infrastructure;

namespace DesignPatterns.Patterns.Factory;

public sealed class FactoryDemo : IPatternDemo
{
    public string Title => "Factory";
    public string Summary => "Simple Factory (anti-pattern) vs. Factory Method vs. Abstract Factory";

    public void Run()
    {
        ConsoleEx.Banner("FACTORY");
        ConsoleEx.Info("Goal: create objects without hard-coding `new ConcreteType()` at the call site.");
        ConsoleEx.Bullet("Three things share that name — only two are GoF patterns. Know the difference.");

        ShowSimpleFactory();
        ShowFactoryMethod();
        ShowAbstractFactory();
        Takeaways();

        ConsoleEx.Pause();
    }

    private static void ShowSimpleFactory()
    {
        ConsoleEx.Section("1) Simple Factory — handy, but not a real pattern");
        ConsoleEx.Code("ITransport Create(kind) => kind switch { Truck => new Truck(), Ship => new Ship(), ... };");

        ConsoleEx.WriteLine("\n    Running it:", ConsoleColor.Gray);
        var truck = SimpleTransportFactory.Create(TransportKind.Truck);
        var ship = SimpleTransportFactory.Create(TransportKind.Ship);
        ConsoleEx.Bullet(truck.Deliver("furniture"));
        ConsoleEx.Bullet(ship.Deliver("containers"));

        ConsoleEx.Warn("It works and centralizes creation — but to add a Drone you must edit the switch.");
        ConsoleEx.Bad("That edits already-tested code: an Open/Closed Principle violation that grows forever.");
    }

    private static void ShowFactoryMethod()
    {
        ConsoleEx.Section("2) Factory Method (GoF) — subclasses choose the product");
        ConsoleEx.Code("abstract class Logistics { protected abstract ITransport CreateTransport();\n                          public string Plan(c) => CreateTransport().Deliver(c); }");
        ConsoleEx.Bullet("The base class owns the workflow; each subclass overrides ONE creation decision.");

        ConsoleEx.WriteLine("\n    Running it:", ConsoleColor.Gray);
        Logistics road = new RoadLogistics();
        Logistics sea = new SeaLogistics();
        ConsoleEx.Bullet(road.Plan("furniture"));
        ConsoleEx.Bullet(sea.Plan("containers"));

        ConsoleEx.Good("Add a new transport => add a new Logistics subclass. No existing class changes.");
    }

    private static void ShowAbstractFactory()
    {
        ConsoleEx.Section("3) Abstract Factory (GoF) — create whole FAMILIES that match");
        ConsoleEx.Code("interface IGuiFactory { IButton CreateButton(); ICheckbox CreateCheckbox(); }");
        ConsoleEx.Bullet("One factory produces a consistent set of related products (a button AND a checkbox).");

        RenderFor("Windows", new WindowsFactory());
        RenderFor("macOS", new MacFactory());

        ConsoleEx.Good("Client depends only on interfaces — it can't mix a Windows button with a Mac checkbox.");
        ConsoleEx.Info("Factory Method makes ONE product; Abstract Factory makes a coordinated FAMILY.");
    }

    private static void RenderFor(string os, IGuiFactory factory)
    {
        ConsoleEx.WriteLine($"\n    Building the UI with the {os} factory:", ConsoleColor.Gray);
        var app = new Application(factory);
        foreach (var widget in app.RenderUi())
            ConsoleEx.Bullet(widget);
    }

    private static void Takeaways()
    {
        ConsoleEx.Section("Takeaways");
        ConsoleEx.Warn("'Simple Factory' = a switch in a static method. Fine for a few stable types only.");
        ConsoleEx.Good("Factory Method: defer the choice of ONE product to a subclass (Open/Closed friendly).");
        ConsoleEx.Good("Abstract Factory: produce a FAMILY of related products that must stay consistent.");
        ConsoleEx.Info("In modern .NET, DI containers and `Func<T>`/typed factories often replace hand-rolled ones.");
    }
}
