[![EN](https://img.shields.io/badge/CHANGELOG-EN-2D2D2D?style=for-the-badge&logo=github&logoColor=FFFFFF)](./CHANGELOG.md)
[![RU](https://img.shields.io/badge/CHANGELOG-RU-2D2D2D?style=for-the-badge&logo=google-translate&logoColor=FFFFFF)](./CHANGELOG-RU.md)

## Changelog · NeoIniLight

<details open>
<summary><strong>1.0.4</strong> — August 8, 2026</summary>

#### List of changes

- **Fixed `API-RU.md`** documenting `UseChecksum`, `UseShielding`, the `ReadOnly` preset, and the `ChecksumMismatch` event — none of these exist in NeoIniLight; the file was a stale copy of the full NeoIni's API reference
- Fixed `ATTRIBUTE-MAPPING.md`/`-RU.md` example using the wrong namespace (`NeoIni.Annotations` instead of `NeoIniLight.Annotations`)
- Corrected the `API.md`/`-RU.md` note about unhandled events — only provider-level errors (via `Error`) and `AddKey`/`RenameKey`/`RenameSection` throw; other events simply do nothing if unhandled
- Fixed `API.md`/`-RU.md` listing `SetValues`/`SetValuesAsync` as generic (`SetValues<T>`) — they take a plain `NeoIniValue[]`
- Updated the stale `1.0.2` version reference in `CONTRIBUTING.md`/`-RU.md`

</details>

<details>
<summary><strong>1.0.3</strong> — August 8, 2026</summary>

#### List of changes

- **Fixed a file-write race condition**: `SaveFile`/`SaveFileAsync` (including the implicit save on `Dispose`/`DisposeAsync`) are now serialized through an internal gate, so concurrent saves can no longer interleave their writes to the same file
- **Fixed silently dropped auto-saves under concurrency**: an auto-save request that arrived while another save was already running is no longer discarded — it is coalesced and retried
- **Fixed `AddKey`/`RenameKey`/`RenameSection`** (and their async counterparts) always throwing after reporting the error via the `Error` event, instead of silently no-oping when a handler is attached
- **Fixed `SetValuesAsync`** not propagating its `cancellationToken` to the per-item write, so cancellation could be ignored mid-batch
- **Fixed a malformed `ArgumentNullException`** thrown by the internal file provider when the file path is `null` (the message was passed as `paramName`)
- **Fixed inconsistent whitespace trimming** between the sync and async file-parsing paths, which could make `GetData()`/`GetDataAsync()` disagree on the same file
- Fixed `Dispose`/`DisposeAsync` silently swallowing a failed final save in Release builds instead of tracing it and reporting it via `Error`
- Corrected the `Saved` event's documentation (fires *after* saving, not before) and `Loaded` (only on `Reload`/`ReloadAsync`, not on construction-time load)
- Removed stale references to checksum validation from `NeoIniOptions` documentation — NeoIniLight has no checksum feature
- Bumped `AsyncReaderWriterLock` dependency to 1.0.3 (fixes a disposed-lock race in the async slow path)

</details>

<details>
<summary><strong>1.0.2</strong> — May 31, 2026</summary>

#### List of changes

- Updated documentation wording: clarified that NeoIniLight is a configuration framework with INI-based persistence, not a classic INI parser
- Added warning to README about library-managed files vs. manual editing
- Corrected NuGet package description and tags

</details>

<details>
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
