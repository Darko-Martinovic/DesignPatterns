using DesignPatterns.Infrastructure;

namespace DesignPatterns.Patterns.Singleton;

public sealed class SingletonDemo : IPatternDemo
{
    public string Title => "Singleton";
    public string Summary => "one instance, global access — and every way to get thread-safety wrong";

    public void Run()
    {
        ConsoleEx.Banner("SINGLETON");
        Intro();
        ShowImplementations();
        RunLiveRace();
        Takeaways();
        ConsoleEx.Pause();
    }

    private static void Intro()
    {
        ConsoleEx.Info("Intent: guarantee a class has exactly ONE instance and a global point of access.");
        ConsoleEx.Bullet("Typical uses: configuration, logging, a connection/factory cache.");
        ConsoleEx.Bullet("The hard part isn't 'one instance' — it's getting there safely from many threads.");
        ConsoleEx.Bullet("(Also: an over-used pattern — it's global mutable state and fights testability/DI.)");
    }

    private static void ShowImplementations()
    {
        ConsoleEx.Section("The approaches you'll see in the wild");

        ConsoleEx.Bad("Lazy null-check with NO locking — the classic tutorial code.");
        ConsoleEx.Code("if (_instance is null)\n    _instance = new Singleton();   // two threads can both enter\nreturn _instance;");
        ConsoleEx.Bullet("Fine single-threaded; under load it constructs the 'singleton' more than once.");

        ConsoleEx.Warn("Lock on EVERY access — correct, but slow.");
        ConsoleEx.Code("lock (_gate) { return _instance ??= new Singleton(); }");
        ConsoleEx.Bullet("Every caller takes the lock forever, even long after init. Needless contention.");

        ConsoleEx.Warn("Double-checked locking — correct, but fragile.");
        ConsoleEx.Code("if (_instance is null)\n    lock (_gate)\n        if (_instance is null)\n            _instance = new Singleton();");
        ConsoleEx.Bullet("Works ONLY because the field is `volatile` — drop it and you can publish a");
        ConsoleEx.Bullet("half-constructed object. Lots of moving parts to memorize correctly.");

        ConsoleEx.Warn("Eager init via static constructor — thread-safe, but not lazy.");
        ConsoleEx.Code("static Singleton() => _instance = new Singleton();");
        ConsoleEx.Bullet("The CLR runs a static ctor once, safely — no locks needed.");
        ConsoleEx.Bullet("But: an explicit static ctor disables `beforefieldinit`, and init is eager,");
        ConsoleEx.Bullet("so you pay the cost even if the instance is never used.");

        ConsoleEx.Good("Lazy<T> — thread-safe AND lazy, nothing to get wrong.");
        ConsoleEx.Code("private static readonly Lazy<Singleton> _i = new(() => new Singleton());\npublic static Singleton Instance => _i.Value;");
        ConsoleEx.Bullet("Default mode is ExecutionAndPublication — exactly what a singleton needs.");
        ConsoleEx.Bullet("This is the modern default. (Better still: register one instance in a DI container.)");
    }

    private static void RunLiveRace()
    {
        const int threads = 64;
        ConsoleEx.Section($"Live thread race — {threads} threads released at the same instant");
        ConsoleEx.Bullet("A correct singleton constructs its instance exactly ONCE. Watch the broken one.");
        Console.WriteLine();

        SingletonStrategy[] strategies =
        [
            new NotThreadSafeStrategy(),
            new LockEveryAccessStrategy(),
            new DoubleCheckedStrategy(),
            new EagerStaticCtorStrategy(),
            new LazyStrategy(),
        ];

        // header
        ConsoleEx.Write("    ", ConsoleColor.Gray);
        ConsoleEx.Write("Strategy".PadRight(30), ConsoleColor.White);
        ConsoleEx.Write("constructions".PadRight(15), ConsoleColor.White);
        ConsoleEx.Write("distinct".PadRight(11), ConsoleColor.White);
        ConsoleEx.WriteLine("verdict", ConsoleColor.White);
        ConsoleEx.WriteLine("    " + new string('-', 64), ConsoleColor.DarkGray);

        foreach (var strategy in strategies)
        {
            var (constructions, distinct) = SingletonRace.Run(strategy, threads);
            var broke = constructions > 1 || distinct > 1;

            ConsoleEx.Write("    ", ConsoleColor.Gray);
            ConsoleEx.Write(strategy.Name.PadRight(30), broke ? ConsoleColor.Red : ConsoleColor.Green);
            ConsoleEx.Write(constructions.ToString().PadRight(15), broke ? ConsoleColor.Red : ConsoleColor.Green);
            ConsoleEx.Write(distinct.ToString().PadRight(11), broke ? ConsoleColor.Red : ConsoleColor.Green);

            if (broke)
                ConsoleEx.WriteLine("BROKEN — multiple instances!", ConsoleColor.Red);
            else
                ConsoleEx.WriteLine("ok — single instance", ConsoleColor.Green);
        }

        Console.WriteLine();
        ConsoleEx.Bullet("'constructions' = times the object was actually built. Anything > 1 means the");
        ConsoleEx.Bullet("singleton guarantee was violated. Re-run from the menu to confirm it's not a fluke.");
    }

    private static void Takeaways()
    {
        ConsoleEx.Section("Takeaways");
        ConsoleEx.Good("Reach for Lazy<T> (or DI lifetime = singleton). Lazy, safe, minimal code.");
        ConsoleEx.Warn("Static ctor / eager init is fine when you KNOW the instance is always needed.");
        ConsoleEx.Bad("Never ship the bare null-check. It passes every single-threaded test you write.");
        ConsoleEx.Info("Interview gotcha: double-checked locking is only correct WITH `volatile`.");
    }
}
