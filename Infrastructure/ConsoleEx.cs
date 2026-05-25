namespace DesignPatterns.Infrastructure;

/// <summary>
/// Tiny helper around <see cref="Console"/> so the demos read cleanly and the
/// output is consistently colorized. Nothing pattern-related lives here.
/// </summary>
public static class ConsoleEx
{
    public static void Write(string text, ConsoleColor color)
    {
        var previous = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.Write(text);
        Console.ForegroundColor = previous;
    }

    public static void WriteLine(string text, ConsoleColor color)
    {
        Write(text, color);
        Console.WriteLine();
    }

    /// <summary>Big banner used at the top of every demo.</summary>
    public static void Banner(string title, ConsoleColor color = ConsoleColor.Cyan)
    {
        var line = new string('═', Math.Max(title.Length + 4, 40));
        Console.WriteLine();
        WriteLine(line, color);
        WriteLine("  " + title, color);
        WriteLine(line, color);
    }

    /// <summary>Section sub-heading inside a demo.</summary>
    public static void Section(string title) =>
        WriteLine($"\n── {title} " + new string('─', Math.Max(0, 50 - title.Length)), ConsoleColor.Magenta);

    public static void Good(string text) => Tagged("[ GOOD ]", text, ConsoleColor.Green);
    public static void Bad(string text) => Tagged("[ BAD  ]", text, ConsoleColor.Red);
    public static void Warn(string text) => Tagged("[ WARN ]", text, ConsoleColor.Yellow);
    public static void Info(string text) => Tagged("[ INFO ]", text, ConsoleColor.Gray);

    private static void Tagged(string tag, string text, ConsoleColor color)
    {
        Write($"  {tag}  ", color);
        Console.WriteLine(text);
    }

    public static void Bullet(string text)
    {
        Write("    • ", ConsoleColor.DarkGray);
        Console.WriteLine(text);
    }

    /// <summary>Renders a snippet of source so good/bad code is visible side by side with its commentary.</summary>
    public static void Code(string code)
    {
        foreach (var rawLine in code.Replace("\r\n", "\n").Split('\n'))
            WriteLine("      " + rawLine, ConsoleColor.DarkCyan);
    }

    public static void Pause()
    {
        if (Console.IsInputRedirected)
            return; // non-interactive run (e.g. `--all` / piped) — don't block.

        WriteLine("\n  Press any key to return to the menu…", ConsoleColor.DarkGray);
        Console.ReadKey(intercept: true);
    }

    /// <summary>Console.Clear() throws when output is redirected; swallow that.</summary>
    public static void SafeClear()
    {
        try { Console.Clear(); } catch { /* redirected output */ }
    }
}
