[![EN](https://img.shields.io/badge/CHANGELOG-EN-2D2D2D?style=for-the-badge&logo=github&logoColor=FFFFFF)](./CHANGELOG.md)
[![RU](https://img.shields.io/badge/CHANGELOG-RU-2D2D2D?style=for-the-badge&logo=google-translate&logoColor=FFFFFF)](./CHANGELOG-RU.md)

## Changelog · NeoIniLight

<details open>
<summary><strong>1.0.1</strong> — 17 мая 2026</summary>

#### List of changes

- **Исправлена проблема с часовым поясом DateTime**
  - Значения `DateTime` теперь сохраняются в UTC ISO формате (`yyyy-MM-ddTHH:mm:ss.fffZ`) и корректно парсятся независимо от локального часового пояса.
  - `FormatInvariant` теперь обрабатывает `DateTime` и `DateTimeOffset` с конвертацией в UTC.
  - `TryParseValue` теперь поддерживает ISO 8601 формат для `DateTime` как основную стратегию парсинга с fallback на инвариантную культуру.

</details>

<details>
<summary><strong>1.0</strong> — 17 мая 2026</summary>

#### First release of NeoIniLight

- Облегчённая версия без шифрования, горячей перезагрузки и расширенных функций
- Сохранены все основные операции чтения/записи INI
- Полная поддержка асинхронного API
- Маппинг на основе атрибутов через source generator
- Функции автосохранения и автоматического резервного копирования
- Потокобезопасность с помощью AsyncReaderWriterLock

</details>
