[![EN](https://img.shields.io/badge/CHANGELOG-EN-2D2D2D?style=for-the-badge&logo=github&logoColor=FFFFFF)](./CHANGELOG.md)
[![RU](https://img.shields.io/badge/CHANGELOG-RU-2D2D2D?style=for-the-badge&logo=google-translate&logoColor=FFFFFF)](./CHANGELOG-RU.md)

## Changelog · NeoIniLight

<details open>
<summary><strong>1.0.1</strong> — May 17, 2026</summary>

#### List of changes

- **Fixed DateTime timezone issue**
  - `DateTime` values are now stored in UTC ISO format (`yyyy-MM-ddTHH:mm:ss.fffZ`) and parsed correctly regardless of local timezone.
  - `FormatInvariant` now handles `DateTime` and `DateTimeOffset` with UTC conversion.
  - `TryParseValue` now supports ISO 8601 format for `DateTime` as primary parsing strategy with fallback to invariant culture.

</details>

<details>
<summary><strong>1.0</strong> — May 17, 2026</summary>

#### First release of NeoIniLight

- Lightweight version without encryption, hot reload, and advanced features
- All core INI read/write operations preserved
- Full async API support
- Attribute-based mapping via source generator
- Auto-save and auto-backup functionality
- Thread-safe with `AsyncReaderWriterLock`

</details>
