# Design Patterns in .NET — Singleton · Factory · Decorator

A small, colorized console app that demonstrates three classic design patterns.
For each pattern it deliberately shows **the wrong / naive approach next to the
recommended one**, so the trade-offs are concrete rather than abstract.

Built for interview prep — the comments in the source explain *why*, not just *how*.

## Run it

```bash
dotnet run            # interactive colorized menu
dotnet run -- --all   # print every demo top-to-bottom (no key presses)
```

Requires the .NET 10 SDK.

## What each demo shows

### Singleton — `Patterns/Singleton/`
The point isn't "one instance", it's getting there **safely from many threads**.
The demo spins up 64 threads at the same instant and counts how many times each
strategy actually constructs its instance:

| Strategy | Verdict |
|---|---|
| Lazy null-check, no locking | ❌ **BROKEN** — constructs multiple instances under load |
| `lock` on every access | ✔ correct, but pays a lock forever |
| Double-checked locking | ✔ correct — *only because the field is `volatile`* |
| Eager / static constructor | ✔ safe (CLR guarantee), but not lazy; kills `beforefieldinit` |
| `Lazy<T>` | ✅ **recommended** — thread-safe **and** lazy, nothing to get wrong |

The canonical, production-shaped classes are in `SingletonImplementations.cs`;
`SingletonRaceHarness.cs` mirrors each strategy on per-run fields so the race is
repeatable from the menu (a real singleton's `static` field can only race once
per process).

### Factory — `Patterns/Factory/`
Three things share the name "factory" — only two are GoF patterns:

- **Simple Factory** (`SimpleFactory.cs`): a `switch` in a static method. Handy,
  but adding a type means editing tested code → Open/Closed violation.
- **Factory Method** (`FactoryMethod.cs`): subclasses decide which single product
  to create. Add a transport = add a subclass, touch nothing else.
- **Abstract Factory** (`AbstractFactory.cs`): create a whole **family** of related
  products (cross-platform GUI) that's guaranteed to stay consistent.

### Decorator — `Patterns/Decorator/`
- **Wrong** (`SubclassExplosion.cs`): one subclass per combination — 2 drinks ×
  2⁸ condiments = 512 classes, with pricing copy-pasted into each.
- **Right** (`Beverages.cs`): compose behavior at runtime by **wrapping**. Any
  combination (even "double mocha") is built from a handful of small classes.
  Real .NET examples: `System.IO` streams and ASP.NET Core middleware.

## Layout

```
Program.cs                     menu + non-interactive --all runner
Infrastructure/                colorized console helpers + IPatternDemo
Patterns/Singleton/            implementations, race harness, demo
Patterns/Factory/              simple factory, factory method, abstract factory, demo
Patterns/Decorator/            subclass explosion, decorators, demo
```
