namespace DesignPatterns.Patterns.Factory;

// ===========================================================================
// ✅ ABSTRACT FACTORY (GoF)
//
// Provide an interface for creating FAMILIES of related objects without naming
// their concrete classes. Where Factory Method makes one product, an Abstract
// Factory makes a whole consistent SET — and guarantees you never mix a Windows
// button with a macOS checkbox.
// ===========================================================================

// --- Family of related products ---
public interface IButton
{
    string Render();
}

public interface ICheckbox
{
    string Render();
}

// --- Windows variant of the family ---
public sealed class WindowsButton : IButton
{
    public string Render() => "[ Windows button ]  (square corners, system blue)";
}

public sealed class WindowsCheckbox : ICheckbox
{
    public string Render() => "[x] Windows checkbox (filled square)";
}

// --- macOS variant of the family ---
public sealed class MacButton : IButton
{
    public string Render() => "( macOS button )    (rounded, pill shape)";
}

public sealed class MacCheckbox : ICheckbox
{
    public string Render() => "(✓) macOS checkbox  (rounded tick)";
}

// --- The abstract factory: creates a whole matching family ---
public interface IGuiFactory
{
    IButton CreateButton();
    ICheckbox CreateCheckbox();
}

public sealed class WindowsFactory : IGuiFactory
{
    public IButton CreateButton() => new WindowsButton();
    public ICheckbox CreateCheckbox() => new WindowsCheckbox();
}

public sealed class MacFactory : IGuiFactory
{
    public IButton CreateButton() => new MacButton();
    public ICheckbox CreateCheckbox() => new MacCheckbox();
}

/// <summary>
/// Client code that builds a UI. It depends ONLY on the abstract factory and
/// the product interfaces — it has no idea whether it's drawing Windows or Mac
/// widgets, and it cannot accidentally combine the two.
/// </summary>
public sealed class Application(IGuiFactory factory)
{
    private readonly IButton _button = factory.CreateButton();
    private readonly ICheckbox _checkbox = factory.CreateCheckbox();

    public IEnumerable<string> RenderUi()
    {
        yield return _button.Render();
        yield return _checkbox.Render();
    }
}
