using System.Threading;

namespace DesignPatterns.Patterns.Singleton;

/// <summary>
/// A shared counter so the demo can *prove* how many times each singleton's
/// constructor actually ran. A correct singleton increments this exactly once
/// no matter how many threads race for the instance.
/// </summary>
public static class InstanceCounter
{
    private static int _windowsCount;

    public static void Reset() => Interlocked.Exchange(ref _windowsCount, 0);

    /// <summary>
    /// Called from each singleton constructor. The small spin widens the race
    /// window so the broken implementation reliably fails in the demo instead of
    /// only failing "sometimes in production at 3am".
    /// </summary>
    public static int RegisterConstruction()
    {
        var value = Interlocked.Increment(ref _windowsCount);
        Thread.SpinWait(20_000); // simulate non-trivial initialization work
        return value;
    }

    public static int Count => Volatile.Read(ref _windowsCount);
}

// ---------------------------------------------------------------------------
// 1) ❌ NOT THREAD-SAFE — the classic interview trap.
// ---------------------------------------------------------------------------

/// <summary>
/// The "textbook" lazy singleton that every tutorial used to show. It is
/// perfectly fine in single-threaded code and completely broken under
/// concurrency: two threads can both pass the null check before either assigns.
/// </summary>
public sealed class NotThreadSafeSingleton
{
    private static NotThreadSafeSingleton? _instance;

    public int InstanceId { get; }

    private NotThreadSafeSingleton() => InstanceId = InstanceCounter.RegisterConstruction();

    public static NotThreadSafeSingleton Instance
    {
        get
        {
            // RACE: between the check and the assignment another thread may run
            // the exact same lines, so `new` executes more than once.
            if (_instance is null)
                _instance = new NotThreadSafeSingleton();
            return _instance;
        }
    }
}

// ---------------------------------------------------------------------------
// 2) ⚠ LOCK ON EVERY ACCESS — correct but pays a lock on every single read.
// ---------------------------------------------------------------------------

/// <summary>
/// Thread-safe because the whole getter is serialized by a lock. Correct, but
/// every caller takes the lock forever — even after the instance exists — which
/// is needless contention on a hot path.
/// </summary>
public sealed class LockEveryAccessSingleton
{
    private static LockEveryAccessSingleton? _instance;
    private static readonly object Gate = new();

    public int InstanceId { get; }

    private LockEveryAccessSingleton() => InstanceId = InstanceCounter.RegisterConstruction();

    public static LockEveryAccessSingleton Instance
    {
        get
        {
            lock (Gate)
            {
                return _instance ??= new LockEveryAccessSingleton();
            }
        }
    }
}

// ---------------------------------------------------------------------------
// 3) ⚠ DOUBLE-CHECKED LOCKING — correct, but easy to get subtly wrong.
// ---------------------------------------------------------------------------

/// <summary>
/// Locks only on first construction, then the fast path is lock-free. Correct in
/// C# — but ONLY because <c>volatile</c> prevents the publication of a
/// partially-constructed object being seen by another thread. Drop the
/// <c>volatile</c> and it becomes a famously hard-to-find bug.
/// </summary>
public sealed class DoubleCheckedLockingSingleton
{
    private static volatile DoubleCheckedLockingSingleton? _instance;
    private static readonly object Gate = new();

    public int InstanceId { get; }

    private DoubleCheckedLockingSingleton() => InstanceId = InstanceCounter.RegisterConstruction();

    public static DoubleCheckedLockingSingleton Instance
    {
        get
        {
            if (_instance is null)               // 1st check: no lock, fast path
            {
                lock (Gate)
                {
                    if (_instance is null)       // 2nd check: inside the lock
                        _instance = new DoubleCheckedLockingSingleton();
                }
            }
            return _instance;
        }
    }
}

// ---------------------------------------------------------------------------
// 4) ⚠ EAGER / STATIC CONSTRUCTOR — thread-safe but not lazy (gotchas apply).
// ---------------------------------------------------------------------------

/// <summary>
/// The CLR guarantees a type's static constructor runs once, before first use,
/// in a thread-safe way — so this is safe without any locks. The catch: the
/// presence of an explicit static constructor disables the <c>beforefieldinit</c>
/// optimization, and initialization happens eagerly on first *touch* of the
/// type (e.g. referencing any member), not necessarily when you intend.
/// </summary>
public sealed class EagerStaticCtorSingleton
{
    private static readonly EagerStaticCtorSingleton _instance;

    public int InstanceId { get; }

    // Explicit static ctor => deterministic timing, but kills beforefieldinit.
    static EagerStaticCtorSingleton() => _instance = new EagerStaticCtorSingleton();

    private EagerStaticCtorSingleton() => InstanceId = InstanceCounter.RegisterConstruction();

    public static EagerStaticCtorSingleton Instance => _instance;
}

// ---------------------------------------------------------------------------
// 5) ✅ Lazy<T> — the modern recommended approach.
// ---------------------------------------------------------------------------

/// <summary>
/// <see cref="Lazy{T}"/> gives you thread-safe, lazy initialization with no
/// hand-written locking and no <c>volatile</c> to get wrong. Default mode is
/// <see cref="LazyThreadSafetyMode.ExecutionAndPublication"/> — exactly what a
/// singleton wants. This is the one to reach for today.
/// </summary>
public sealed class LazySingleton
{
  

    private static readonly Lazy<LazySingleton> _instance = new(()=> new LazySingleton(), LazyThreadSafetyMode.ExecutionAndPublication);


    public int InstanceId { get; }

    private LazySingleton() => InstanceId = InstanceCounter.RegisterConstruction();

    public static LazySingleton Instance => _instance.Value;

    //public static LazySingleton Intstance => _instance.Value;
}
