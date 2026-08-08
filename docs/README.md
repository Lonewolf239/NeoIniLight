[![NuGet](https://img.shields.io/nuget/v/NeoIniLight?style=for-the-badge&logo=nuget&logoColor=FFFFFF)](https://www.nuget.org/packages/NeoIniLight)
[![Downloads](https://img.shields.io/nuget/dt/NeoIniLight?style=for-the-badge&logo=download&logoColor=FFFFFF)](https://www.nuget.org/packages/NeoIniLight)
[![.NET 5+](https://img.shields.io/badge/.NET-5+-2D2D2D?style=for-the-badge&logo=dotnet&logoColor=FFFFFF)](https://dotnet.microsoft.com/)
[![.NET Standard 2.0](https://img.shields.io/badge/.NET%20Standard-2.0-2D2D2D?style=for-the-badge&logo=dotnet&logoColor=FFFFFF)](https://learn.microsoft.com/en-us/dotnet/standard/net-standard?tabs=net-standard-2-0)
[![MIT](https://img.shields.io/badge/License-MIT-2D2D2D?style=for-the-badge&logo=open-source-initiative&logoColor=FFFFFF)](https://github.com/Lonewolf239/NeoIniLight/blob/main/LICENSE)

[![Changelog](https://img.shields.io/badge/CHANGELOG-2D2D2D?style=for-the-badge&logo=history&logoColor=FFFFFF)](./CHANGELOG.md)
[![xUnit Tests](https://img.shields.io/badge/xUnit-Tests-2D2D2D?style=for-the-badge&logo=xunit&logoColor=FFFFFF)](https://lonewolf239.github.io/NeoIniLight/)

### Languages
[![EN](https://img.shields.io/badge/README-EN-2D2D2D?style=for-the-badge&logo=github&logoColor=FFFFFF)](./README.md)
[![RU](https://img.shields.io/badge/README-RU-2D2D2D?style=for-the-badge&logo=google-translate&logoColor=FFFFFF)](./README-RU.md)

# NeoIniLight

**Lightweight, thread-safe configuration framework for .NET with INI-based persistence** — atomic writes, automatic backups, typed access, and full async API.

> ⚠️ **Not a classic INI parser** – NeoIniLight is a **configuration management system** that uses the INI format as its storage layer. Files are managed by the library; manual editing may lead to unexpected behaviour. For hand‑editable INI files, consider using a simple key-value parser instead.

```bash
dotnet add package NeoIniLight
```

- **Package:** [nuget.org/packages/NeoIniLight](https://www.nuget.org/packages/NeoIniLight)
- **Version:** 1.0.5 | **.NET 5+** | **.NET Standard 2.0**
- **Developer:** [Lonewolf239](https://github.com/Lonewolf239)

---

## 🚀 NeoIni vs NeoIniLight

| Feature | NeoIni (Full) | NeoIniLight |
|---------|:-------------:|:------------:|
| **License** | GPLv3 / Commercial | MIT |
| **AES-256 encryption** | ✅ | ❌ |
| **SHA-256 checksum** | ✅ | ❌ |
| **Pluggable providers** | ✅ | ❌ |
| **Hot-reload** | ✅ | ❌ |
| **Human-editable mode** | ✅ | ❌ |
| **Atomic writes & backups** | ✅ | ✅ |
| **Thread-safe** | ✅ | ✅ |
| **Typed Get/Set** | ✅ | ✅ |
| **Full async API** | ✅ | ✅ |
| **Object mapping (source generator)** | ✅ | ✅ |
| **MIT license** | ❌ | ✅ |

> **Need encryption, checksum validation, or advanced features?** Check out the full [NeoIni](https://github.com/Lonewolf239/NeoIni) library.

---

## Features

| | Feature | Details |
|---|---------|---------|
| 🔐 | **Thread-safe** | `AsyncReaderWriterLock` protects all read/write operations under concurrent access with full `async`/`await` support. |
| 📦 | **Typed Get/Set** | Read and write `bool`, `int`, `double`, `DateTime`, `enum`, `string` and more with automatic parsing and defaults. |
| ⚡ | **AutoSave & AutoBackup** | Automatic saving after N operations. Atomic writes via `.tmp` + `.backup` fallback on errors. |
| 🗺️ | **Object mapping** | Source-generated `Get<T>()` / `Set<T>()` for POCO classes via `NeoIniKeyAttribute`. |
| 📡 | **Full async API** | Async versions for all major operations — `CreateAsync`, `GetValueAsync`, `SaveFileAsync`, etc. |
| 🔍 | **Search & TryGet** | Case-insensitive search across keys/values. `TryGetValue<T>` reads without modifying the file. |
| 📢 | **Rich event system** | 14 events: save, load, key/section CRUD, data clearing, autosave, errors, search completion. |
| 📦 | **Black-box design** | Single entrypoint — `NeoIniDocument` owns and manages everything behind a clean public API. |

---

## Quick Start

### Creating an instance

<details>
  <summary>⚙️ Code: NeoIniLight example</summary>

```csharp
using NeoIniLight;

// Plain
NeoIniDocument document = new("config.ini");

// With options
NeoIniDocument doc = new("config.ini", NeoIniOptions.Safe);

// Async
NeoIniDocument document = await NeoIniDocument.CreateAsync("config.ini", cancellationToken: ct);
```

</details>

### Reading & writing values

<details>
  <summary>⚙️ Code: NeoIniLight example</summary>

```csharp
// Write
document.SetValue("Database", "Host", "localhost");
document.SetValue("Database", "Port", 5432);

// Read with typed defaults
string host = document.GetValue("Database", "Host", "127.0.0.1");
int    port = document.GetValue("Database", "Port", 3306);

// Read without side effects (no AutoAdd, no file modification)
if (document.TryGetValue("Game", "Level", out int level))
{
    /* use level */
}
else level = 1;
```

```csharp
// Async
await document.SetValueAsync("Database", "Host", "localhost");
string host = await document.GetValueAsync("Database", "Host", "127.0.0.1", ct);
```

</details>

- Missing sections/keys return `defaultValue`; with `UseAutoAdd` enabled the key is created automatically.
- Supports `enum`, `DateTime`, and any `IConvertible` type via invariant-culture parsing.

### Section & key management

<details>
  <summary>⚙️ Code: NeoIniLight example</summary>

```csharp
document.AddSection("Cache");
document.RemoveKey("Cache", "OldKey");
document.RenameSection("Cache", "AppCache");

string[] sections = document.GetAllSections();
string[] keys     = document.GetAllKeys("AppCache");
bool exists       = document.SectionExists("AppCache");
```

</details>

### Search

<details>
  <summary>⚙️ Code: NeoIniLight example</summary>

```csharp
var results = document.Search("token");
foreach (var r in results)
    Console.WriteLine($"[{r.Section}] {r.Key} = {r.Value}");
```

</details>

### File operations

<details>
  <summary>⚙️ Code: NeoIniLight example</summary>

```csharp
document.SaveFile();
document.Reload();
document.DeleteFile();
document.DeleteFileWithData();
```

</details>

### Options & presets

<details>
  <summary>⚙️ Code: NeoIniLight example</summary>

```csharp
document.UseAutoSave = true;
document.AutoSaveInterval = 3;    // save every 3 writes
document.UseAutoBackup = true;
document.UseAutoAdd = true;
document.SaveOnDispose = true;
document.AllowEmptyValues = true;
```

</details>

Or use built-in presets: `NeoIniOptions.Default`, `Safe`, `Performance`, `BufferedAutoSave(n)`.

### Events

<details>
  <summary>⚙️ Code: NeoIniLight example</summary>

```csharp
document.Saved            += (_, _) => Console.WriteLine("Saved");
document.Loaded           += (_, _) => Console.WriteLine("Loaded");
document.KeyChanged       += (_, e) => Console.WriteLine($"[{e.Section}] {e.Key} → {e.Value}");
document.KeyAdded         += (_, e) => Console.WriteLine($"[{e.Section}] +{e.Key}");
document.Error            += (_, e) => Console.WriteLine($"Error: {e.Exception.Message}");
```

</details>

### Disposal

<details>
  <summary>⚙️ Code: NeoIniLight example</summary>

```csharp
using NeoIniDocument document = new("config.ini");
// SaveFile() is called automatically if SaveOnDispose is true
// After disposal — ObjectDisposedException on any access
```

</details>

---

## Advanced Features

- Attribute-based mapping & source generator — [detailed guide](./ATTRIBUTE-MAPPING.md)

---

## API Reference

Full method, options, and event reference — [API.md](./API.md)

---

## License

NeoIniLight is released under the **MIT** license.

---

## Philosophy

**Black Box Design** — all internal logic is hidden behind the simple public API of `NeoIniDocument`. You work only with methods and events, without worrying about implementation details.

**Why not just an INI parser?** – Classic INI parsers treat files as simple text and preserve user formatting. NeoIniLight, in contrast, **owns** the configuration file: it uses atomic writes (`.tmp` → replace), keeps a `.backup` fallback, and may inject metadata or warning comments to guarantee consistency. This makes it unsuitable for scenarios where users need to edit the file directly. If you need a pure, comment‑preserving INI parser, NeoIniLight is not the right tool.

Thus, NeoIniLight is a **configuration subsystem** that happens to serialize to a human‑readable text format resembling INI — not a replacement for lightweight INI editors.
