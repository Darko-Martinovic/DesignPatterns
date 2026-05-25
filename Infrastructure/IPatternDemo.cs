namespace DesignPatterns.Infrastructure;

/// <summary>
/// Every runnable demo (one per pattern) implements this so the menu can stay
/// data-driven: add a demo to the list and it shows up automatically.
/// </summary>
public interface IPatternDemo
{
    /// <summary>Label shown in the menu, e.g. "Singleton".</summary>
    string Title { get; }

    /// <summary>One-line description shown next to the menu entry.</summary>
    string Summary { get; }

    /// <summary>Runs the interactive demonstration.</summary>
    void Run();
}
