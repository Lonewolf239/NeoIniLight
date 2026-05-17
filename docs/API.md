[![EN](https://img.shields.io/badge/API-EN-2D2D2D?style=for-the-badge&logo=github&logoColor=FFFFFF)](./API.md)
[![RU](https://img.shields.io/badge/API-RU-2D2D2D?style=for-the-badge&logo=google-translate&logoColor=FFFFFF)](./API-RU.md)

## API Reference · NeoIniLight

Complete reference for all public methods, options, and events exposed by `NeoIniDocument`.

---

### Core Methods

| Method | Description | Async Version |
|--------|-------------|---------------|
| `GetValue<T>` | Read typed value with default fallback (optionally auto-adding) | `GetValueAsync<T>` |
| `GetValueClamped<T>` | Read typed value and clamp it between min/max | `GetValueClampedAsync<T>` |
| `TryGetValue<T>` | Read typed value without modifying the file and without AutoAdd | – |
| `SetValue<T>` | Set/create key-value | `SetValueAsync<T>` |
| `SetValues<T>` | Set/create multiple key-value pairs (bulk) | `SetValuesAsync<T>` |
| `SetValueClamped<T>` | Set/create key-value and clamp it within range | `SetValueClampedAsync<T>` |
| `AddSection` | Create section if missing | `AddSectionAsync` |
| `AddKey<T>` | Add unique key-value | `AddKeyAsync<T>` |
| `AddKeyClamped<T>` | Add unique key-value and clamp it within range | `AddKeyClampedAsync<T>` |
| `RemoveKey` | Delete specific key | `RemoveKeyAsync` |
| `RemoveSection` | Delete entire section | `RemoveSectionAsync` |
| `ClearSection` | Remove all keys from section | `ClearSectionAsync` |
| `RenameKey` | Rename key in section | `RenameKeyAsync` |
| `RenameSection` | Rename entire section | `RenameSectionAsync` |
| `Search` | Search keys/values by pattern | – |
| `FindKey` | Search a key across all sections | – |
| `GetAllSections` | List all sections | – |
| `GetAllKeys` | List keys in section | – |
| `GetSection` | Get all key-value pairs in section | – |
| `SectionExists` | Check if section exists | – |
| `KeyExists` | Check if key exists in section | – |
| `SaveFile` | Save data to storage | `SaveFileAsync` |
| `ToString` | Serialize INI data to formatted string (as in file) | – |
| `Reload` | Reload data from storage | `ReloadAsync` |
| `DeleteFile` | Delete file from disk | – |
| `DeleteFileWithData` | Delete file and clear data | – |
| `DeleteBackup` | Delete the backup file from disk | – |
| `Clear` | Clear internal data structure completely | – |
| `CreateAsync` | Asynchronously create and initialize reader (static factory) | – |

---

### Options (NeoIniOptions)

| Option | Description | Default |
|--------|-------------|---------|
| `UseAutoSave` | Automatically saves changes to storage after modifications | `true` |
| `AutoSaveInterval` | Number of operations between automatic saves when AutoSave is enabled | `0` (every change) |
| `UseAutoBackup` | Creates `.backup` files during save operations for safety | `true` |
| `UseAutoAdd` | Automatically creates missing sections/keys with default values when reading via `GetValue<T>` | `true` |
| `SaveOnDispose` | Automatically saves the configuration when the instance is disposed | `true` |
| `AllowEmptyValues` | Permits configuration keys to be saved with empty or null values | `true` |

**Built-in presets:** `Default`, `Safe`, `Performance`, `BufferedAutoSave(interval)`.

---

### Events

| Event | Description |
|-------|-------------|
| `Saved` | Called after saving data to storage |
| `Loaded` | Called after successfully loading data or reloading |
| `KeyChanged` | Called when the value of an existing key changes |
| `KeyRenamed` | Called when a key is renamed within a section |
| `KeyAdded` | Called when a new key is added to a section |
| `KeyRemoved` | Called when a key is removed from a section |
| `SectionChanged` | Called whenever a section changes (keys changed/added/removed) |
| `SectionRenamed` | Called when a section is renamed |
| `SectionAdded` | Called when a new section is added |
| `SectionRemoved` | Called when a section is deleted |
| `DataCleared` | Called when data is completely cleared |
| `AutoSave` | Called before automatic saving |
| `SearchCompleted` | Called after each search with the pattern and match count |
| `Error` | Called when errors occur (parsing, saving, reading, etc.) |

> **Note:** If no handlers are subscribed to an event, the provider (by default) throws an exception. For silent handling, always subscribe to `Error`.
