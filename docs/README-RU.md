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

**Лёгкий, потокобезопасный конфигурационный фреймворк для .NET с INI-подобным хранением** — атомарная запись, автоматические бэкапы, типизированный доступ и полный async API.

> ⚠️ **Не классический парсер INI** – NeoIniLight — это **система управления конфигурацией**, которая использует INI-формат как слой сериализации. Файлы находятся под управлением библиотеки; ручное редактирование может привести к неожиданному поведению.

```bash
dotnet add package NeoIniLight
```

- **Пакет:** [nuget.org/packages/NeoIniLight](https://www.nuget.org/packages/NeoIniLight)
- **Версия:** 1.0.2 | **.NET 5+** | **.NET Standard 2.0**
- **Разработчик:** [Lonewolf239](https://github.com/Lonewolf239)

---

## 🚀 NeoIni vs NeoIniLight

| Feature | NeoIni (Полная) | NeoIniLight |
|---------|:---------------:|:-----------:|
| **Лицензия** | GPLv3 / Commercial | MIT |
| **AES-256 шифрование** | ✅ | ❌ |
| **SHA-256 контрольная сумма** | ✅ | ❌ |
| **Подключаемые провайдеры** | ✅ | ❌ |
| **Hot-reload** | ✅ | ❌ |
| **Human-editable режим** | ✅ | ❌ |
| **Атомарная запись и бэкапы** | ✅ | ✅ |
| **Потокобезопасность** | ✅ | ✅ |
| **Типизированные Get/Set** | ✅ | ✅ |
| **Полный async API** | ✅ | ✅ |
| **Object mapping (source generator)** | ✅ | ✅ |
| **MIT лицензия** | ❌ | ✅ |

> **Нужно шифрование, проверка целостности или продвинутые функции?** Обратите внимание на полную версию [NeoIni](https://github.com/Lonewolf239/NeoIni).

---

## Features

| | Feature | Details |
|---|---------|---------|
| 🔐 | **Thread-safe** | `AsyncReaderWriterLock` защищает все операции чтения и записи при конкурентном доступе и полностью поддерживает `async`/`await`. |
| 📦 | **Typed Get/Set** | Чтение и запись `bool`, `int`, `double`, `DateTime`, `enum`, `string` и других типов с автоматическим парсингом и значениями по умолчанию. |
| ⚡ | **AutoSave & AutoBackup** | Автоматическое сохранение после N операций. Атомарная запись через `.tmp` + откат на `.backup` при ошибках. |
| 🗺️ | **Object mapping** | Source-generated `Get<T>()` / `Set<T>()` для POCO-классов через `NeoIniKeyAttribute`. |
| 📡 | **Full async API** | Асинхронные версии всех основных операций — `CreateAsync`, `GetValueAsync`, `SaveFileAsync` и т.д. |
| 🔍 | **Search & TryGet** | Регистронезависимый поиск по ключам/значениям. `TryGetValue<T>` читает без модификации файла. |
| 📢 | **Rich event system** | 12 событий: сохранение, загрузка, CRUD ключей/секций, autosave, ошибки, завершение поиска. |
| 📦 | **Black-box design** | Единая точка входа — `NeoIniDocument` владеет и управляет всем за чистым публичным API. |

---

## Quick Start

### Creating an instance

<details>
  <summary>⚙️ Код: пример NeoIniLight</summary>

```csharp
using NeoIniLight;

// Без шифрования
NeoIniDocument document = new("config.ini");

// С опциями
NeoIniDocument doc = new("config.ini", NeoIniOptions.Safe);

// Асинхронно
NeoIniDocument document = await NeoIniDocument.CreateAsync("config.ini", cancellationToken: ct);
```

</details>

### Reading & writing values

<details>
  <summary>⚙️ Код: пример NeoIniLight</summary>

```csharp
// Запись
document.SetValue("Database", "Host", "localhost");
document.SetValue("Database", "Port", 5432);

// Чтение с типизированными значениями по умолчанию
string host = document.GetValue("Database", "Host", "127.0.0.1");
int    port = document.GetValue("Database", "Port", 3306);

// Чтение без побочных эффектов (без AutoAdd, без модификации файла)
if (document.TryGetValue("Game", "Level", out int level))
{
    /* use level */
}
else level = 1;
```

```csharp
// Асинхронно
await document.SetValueAsync("Database", "Host", "localhost");
string host = await document.GetValueAsync("Database", "Host", "127.0.0.1", ct);
```

</details>

- Отсутствующие секции/ключи возвращают `defaultValue`; при включённом `UseAutoAdd` ключ создаётся автоматически.
- Поддерживаются `enum`, `DateTime` и любые `IConvertible` типы через invariant-culture парсинг.

### Section & key management

<details>
  <summary>⚙️ Код: пример NeoIniLight</summary>

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
  <summary>⚙️ Код: пример NeoIniLight</summary>

```csharp
var results = document.Search("token");
foreach (var r in results)
    Console.WriteLine($"[{r.Section}] {r.Key} = {r.Value}");
```

</details>

### File operations

<details>
  <summary>⚙️ Код: пример NeoIniLight</summary>

```csharp
document.SaveFile();
document.Reload();
document.DeleteFile();
document.DeleteFileWithData();
```

</details>

### Options & presets

<details>
  <summary>⚙️ Код: пример NeoIniLight</summary>

```csharp
document.UseAutoSave = true;
document.AutoSaveInterval = 3;    // сохранять каждые 3 записи
document.UseAutoBackup = true;
document.UseAutoAdd = true;
document.SaveOnDispose = true;
document.AllowEmptyValues = true;
```

</details>

Или используйте встроенные пресеты: `NeoIniOptions.Default`, `Safe`, `Performance`, `BufferedAutoSave(n)`.

### Events

<details>
  <summary>⚙️ Код: пример NeoIniLight</summary>

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
  <summary>⚙️ Код: пример NeoIniLight</summary>

```csharp
using NeoIniDocument document = new("config.ini");
// SaveFile() вызывается автоматически, если SaveOnDispose = true
// После Dispose — ObjectDisposedException при любом обращении
```

</details>

---

## Advanced Features

- Attribute-based mapping & source generator — [подробный гайд](./ATTRIBUTE-MAPPING-RU.md)

---

## API Reference

Полная справка по методам, опциям и событиям — [API-RU.md](./API-RU.md)

---

## License

NeoIniLight распространяется под лицензией **MIT**.

---

## Philosophy (Философия)

**Black Box Design (Чёрный ящик)** — вся внутренняя логика скрыта за простым публичным API класса `NeoIniDocument`. Вы работаете только с методами и событиями, не задумываясь о деталях реализации.

**Почему не просто парсер INI?** – Классические парсеры INI относятся к файлам как к простому тексту и сохраняют пользовательское форматирование. NeoIniLight, напротив, **полностью управляет** конфигурационным файлом: использует атомарную запись (`.tmp` → замена), ведёт `.backup` и может вставлять служебные комментарии для обеспечения согласованности. Это делает его непригодным для сценариев, где пользователи правят файл вручную. Если вам нужен чистый парсер INI с сохранением комментариев, NeoIniLight не подходит.

Таким образом, NeoIniLight — это **конфигурационная подсистема**, которая сериализует данные в человекочитаемый текстовый формат, напоминающий INI, а не замена легковесным редакторам INI.
