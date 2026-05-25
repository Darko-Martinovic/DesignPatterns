# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

A single-project .NET 10 console app for interview prep that demonstrates three GoF patterns (Singleton, Factory, Decorator). The defining convention: **every demo shows the wrong/naive approach next to the recommended one**, and source comments explain *why*, not just *how*. Treat the explanatory comments as the product — they are the point of the repo, not incidental.

## Commands

```bash
dotnet run            # interactive colorized menu
dotnet run -- --all   # run every demo top-to-bottom, no key presses (use this to verify output)
dotnet build          # compile
```

Requires the .NET 10 SDK. There is no test project, linter, or CI — verification is running `--all` and reading the output.

## Architecture

- **Data-driven menu.** `Program.cs` (top-level statements) holds a single `IPatternDemo[]` array. Adding a pattern = implement `IPatternDemo` (`Title`, `Summary`, `Run()`) and add one entry to that array; the menu and `--all` runner pick it up automatically. Don't add per-pattern wiring elsewhere.
- **Non-interactive guard.** Demos must run without blocking when stdin is redirected. `Program.cs` runs all demos and exits when `args` contains `--all` **or** `Console.IsInputRedirected`. `ConsoleEx.Pause()` and `ConsoleEx.SafeClear()` already no-op/swallow in that mode — use them rather than calling `Console.ReadKey`/`Console.Clear` directly.
- **All console output goes through `Infrastructure/ConsoleEx.cs`.** Use its semantic helpers (`Good`/`Bad`/`Warn`/`Info`, `Banner`, `Section`, `Bullet`, `Code`) for consistent colorization. No pattern logic lives here.
- **One folder per pattern** under `Patterns/`: the pattern implementation classes plus a `<Pattern>Demo.cs` that narrates them. Implementations stay free of `Console` calls; narration lives only in the demo.

### Singleton has a deliberate two-file split
`SingletonImplementations.cs` holds the canonical, production-shaped singletons (`static` instance fields). But a real singleton's `static` field can only race once per process, so the live menu race can't use them repeatably. `SingletonRaceHarness.cs` mirrors each locking strategy on **per-instance fields** (`SingletonStrategy` subclasses) so `SingletonRace.Run` can re-run the 64-thread race from the menu as many times as wanted. When changing one file's locking logic, keep the other in sync — they intentionally represent the same strategies.

## Conventions

- Nullable reference types and `ImplicitUsings` are enabled; `LangVersion` is `latest`. Match the existing terse, comment-heavy style.
- Demo classes are `sealed`. Pattern base types intended for extension (e.g. `Logistics`, `SingletonStrategy`) are `abstract`.
- UTF-8 box-drawing/glyph characters are used in output; `Program.cs` sets `Console.OutputEncoding` accordingly.
