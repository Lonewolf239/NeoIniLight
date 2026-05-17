📖 **[Documentation on GitHub](https://github.com/Lonewolf239/NeoIniLight#readme)** — detailed guides, API reference, and examples.

# NeoIniLight

Lightweight, thread-safe INI configuration library for .NET with atomic writes and automatic backup support.

```bash
dotnet add package NeoIniLight
```

- **Version:** 1.0.1 | **.NET 5+** | **.NET Standard 2.0**
- **Developer:** [Lonewolf239](https://github.com/Lonewolf239)

---

## Features

- 🔐 **Thread-safe** — `AsyncReaderWriterLock` protects all read/write operations
- 📦 **Typed Get/Set** — Read/write `bool`, `int`, `double`, `DateTime`, `enum`, `string` with defaults
- ⚡ **AutoSave & AutoBackup** — Automatic saving after N operations, atomic `.tmp` writes, `.backup` fallback
- 🗺️ **Object mapping** — Source-generated `Get<T>()` / `Set<T>()` via `NeoIniKeyAttribute`
- 📡 **Full async API** — `CreateAsync`, `GetValueAsync`, `SaveFileAsync`, etc.
- 🔍 **Search & TryGet** — Case-insensitive search, side-effect-free reads
- 📢 **Rich event system** — 12 events for save, load, CRUD, errors, search completion

---

## Quick Example

```csharp
using NeoIniLight;

var doc = new NeoIniDocument("config.ini");

doc.SetValue("Database", "Host", "localhost");
doc.SetValue("Database", "Port", 5432);

string host = doc.GetValue("Database", "Host", "127.0.0.1");
int port = doc.GetValue("Database", "Port", 3306);
```

---

## NeoIni vs NeoIniLight

| Feature | NeoIni (Full) | NeoIniLight |
|---------|:-------------:|:-----------:|
| License | GPLv3 / Commercial | MIT |
| AES-256 encryption | ✅ | ❌ |
| SHA-256 checksum | ✅ | ❌ |
| Pluggable providers | ✅ | ❌ |
| Hot-reload | ✅ | ❌ |
| All NeoIniLight features | ✅ | ✅ |

> **Need encryption or advanced features?** Check out [NeoIni](https://github.com/Lonewolf239/NeoIni).
