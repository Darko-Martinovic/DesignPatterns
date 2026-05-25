using System.Text;
using DesignPatterns.Infrastructure;
using DesignPatterns.Patterns.Decorator;
using DesignPatterns.Patterns.Factory;
using DesignPatterns.Patterns.Singleton;

// Box-drawing characters and the price glyphs render correctly with UTF-8.
try { Console.OutputEncoding = Encoding.UTF8; } catch { /* redirected output */ }
Console.Title = "Design Patterns in .NET — Singleton · Factory · Decorator";

IPatternDemo[] demos =
[
    new SingletonDemo(),
    new FactoryDemo(),
    new DecoratorDemo(),
];

// Non-interactive run: `--all`, or when stdin is redirected (CI / piped).
// Runs every demo top-to-bottom and exits, so the output can be reviewed.
if (args.Contains("--all") || Console.IsInputRedirected)
{
    foreach (var demo in demos)
        demo.Run();
    return;
}

while (true)
{
    ConsoleEx.SafeClear();
    DrawMenu(demos);

    var choice = Console.ReadKey(intercept: true).KeyChar;
    if (choice is 'q' or 'Q')
        break;

    var index = choice - '1';
    if (index >= 0 && index < demos.Length)
    {
        ConsoleEx.SafeClear();
        demos[index].Run();
    }
}

ConsoleEx.WriteLine("\nThanks — happy interviewing!\n", ConsoleColor.Cyan);
return;

static void DrawMenu(IReadOnlyList<IPatternDemo> demos)
{
    ConsoleEx.Banner("DESIGN PATTERNS IN .NET", ConsoleColor.Cyan);
    ConsoleEx.WriteLine("  Each demo shows the WRONG approach alongside the recommended one.\n", ConsoleColor.DarkGray);

    for (var i = 0; i < demos.Count; i++)
    {
        ConsoleEx.Write($"   {i + 1}. ", ConsoleColor.Yellow);
        ConsoleEx.Write(demos[i].Title.PadRight(12), ConsoleColor.White);
        ConsoleEx.WriteLine(demos[i].Summary, ConsoleColor.DarkGray);
    }

    ConsoleEx.Write("\n   Q. ", ConsoleColor.Yellow);
    ConsoleEx.WriteLine("Quit", ConsoleColor.White);
    ConsoleEx.Write("\n  Choose a pattern: ", ConsoleColor.Cyan);
}
