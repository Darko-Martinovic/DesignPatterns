using System.Collections.Concurrent;
using System.Threading;

namespace DesignPatterns.Patterns.Singleton;

/// <summary>
/// A repeatable reproduction of each construction strategy. The canonical,
/// production-shaped classes live in <c>SingletonImplementations.cs</c>, but a
/// real singleton stores its instance in a <c>static</c> field that lives for
/// the whole process — so it can only ever demonstrate a race on the very first
/// access. These strategy objects mirror the exact locking logic on per-instance
/// fields, so the live race can be re-run from the menu as many times as you like.
/// </summary>
public abstract class SingletonStrategy
{
    private int _constructions;

    public abstract string Name { get; }
    public abstract bool ThreadSafe { get; }
    public abstract string OneLiner { get; }

    /// <summary>How many times an instance was actually constructed (must be 1).</summary>
    public int Constructions => Volatile.Read(ref _constructions);

    /// <summary>
    /// Stand-in for "expensive object creation". The spin widens the race window
    /// so a broken strategy fails deterministically in the demo rather than only
    /// under rare production timing.
    /// </summary>
    protected object Construct()
    {
        Interlocked.Increment(ref _constructions);
        Thread.SpinWait(20_000);
        return new object();
    }

    /// <summary>Called concurrently by every racing thread.</summary>
    public abstract object GetInstance();
}

/// <summary>❌ No synchronization: two threads can both pass the null check.</summary>
public sealed class NotThreadSafeStrategy : SingletonStrategy
{
    private object? _instance;
    public override string Name => "Lazy null-check, no locking";
    public override bool ThreadSafe => false;
    public override string OneLiner => "if (_instance is null) _instance = new();";
    public override object GetInstance() => _instance ??= Construct();
}

/// <summary>✔ Correct, but every read pays for a lock — even after init.</summary>
public sealed class LockEveryAccessStrategy : SingletonStrategy
{
    private readonly object _gate = new();
    private object? _instance;
    public override string Name => "lock on every access";
    public override bool ThreadSafe => true;
    public override string OneLiner => "lock (gate) { return _instance ??= new(); }";
    public override object GetInstance()
    {
        lock (_gate)
            return _instance ??= Construct();
    }
}

/// <summary>✔ Correct AND fast — but only because the field is volatile.</summary>
public sealed class DoubleCheckedStrategy : SingletonStrategy
{
    private readonly object _gate = new();
    private volatile object? _instance;
    public override string Name => "double-checked locking";
    public override bool ThreadSafe => true;
    public override string OneLiner => "if (null) lock { if (null) _instance = new(); }";
    public override object GetInstance()
    {
        if (_instance is null)
        {
            lock (_gate)
            {
                if (_instance is null)
                    _instance = Construct();
            }
        }
        return _instance;
    }
}

/// <summary>✔ Eager init in a static ctor — safe by CLR guarantee, but not lazy.</summary>
public sealed class EagerStaticCtorStrategy : SingletonStrategy
{
    private readonly object _instance;
    public EagerStaticCtorStrategy() => _instance = Construct(); // happens once, up front
    public override string Name => "eager / static constructor";
    public override bool ThreadSafe => true;
    public override string OneLiner => "static Foo() => _instance = new();";
    public override object GetInstance() => _instance;
}

/// <summary>✅ The modern recommendation: thread-safe AND lazy, no hand-rolled locks.</summary>
public sealed class LazyStrategy : SingletonStrategy
{
    private readonly Lazy<object> _instance;
    public LazyStrategy() => _instance = new Lazy<object>(Construct);
    public override string Name => "Lazy<T>";
    public override bool ThreadSafe => true;
    public override string OneLiner => "new Lazy<Foo>(() => new Foo());";
    public override object GetInstance() => _instance.Value;
}

/// <summary>Runs many threads at a single instant against one strategy.</summary>
public static class SingletonRace
{
    public static (int constructions, int distinctInstances) Run(SingletonStrategy strategy, int threads)
    {
        var barrier = new Barrier(threads);
        var seen = new ConcurrentDictionary<object, byte>(ReferenceEqualityComparer.Instance);
        var workers = new Thread[threads];

        for (var i = 0; i < threads; i++)
        {
            workers[i] = new Thread(() =>
            {
                barrier.SignalAndWait();           // release every thread at the same moment
                seen.TryAdd(strategy.GetInstance(), 0);
            });
        }

        foreach (var w in workers) w.Start();
        foreach (var w in workers) w.Join();

        return (strategy.Constructions, seen.Count);
    }
}
