[![EN](https://img.shields.io/badge/CHANGELOG-EN-2D2D2D?style=for-the-badge&logo=github&logoColor=FFFFFF)](./CHANGELOG.md)
[![RU](https://img.shields.io/badge/CHANGELOG-RU-2D2D2D?style=for-the-badge&logo=google-translate&logoColor=FFFFFF)](./CHANGELOG-RU.md)

## Changelog · NeoIniLight

<details open>
<summary><strong>1.0.4</strong> — 8 августа 2026</summary>

#### List of changes

- **Исправлен `API-RU.md`**: в нём были задокументированы `UseChecksum`, `UseShielding`, пресет `ReadOnly` и событие `ChecksumMismatch` — ничего из этого в NeoIniLight нет, файл оказался устаревшей копией API-справки полного NeoIni
- Исправлен неверный namespace в примерах `ATTRIBUTE-MAPPING.md`/`-RU.md` (`NeoIni.Annotations` вместо `NeoIniLight.Annotations`)
- Исправлено примечание в `API.md`/`-RU.md` про необработанные события — исключение бросают только ошибки провайдера (через `Error`) и `AddKey`/`RenameKey`/`RenameSection`, остальные события просто ничего не делают, если на них никто не подписан
- Исправлено указание `SetValues`/`SetValuesAsync` как generic-методов (`SetValues<T>`) в `API.md`/`-RU.md` — на деле они принимают обычный `NeoIniValue[]`
- Обновлена устаревшая версия `1.0.2` в `CONTRIBUTING.md`/`-RU.md`

</details>

<details>
<summary><strong>1.0.3</strong> — 8 августа 2026</summary>

#### List of changes

- **Исправлена гонка при записи файла**: `SaveFile`/`SaveFileAsync` (в том числе неявное сохранение при `Dispose`/`DisposeAsync`) теперь сериализуются через внутренний гейт, поэтому параллельные сохранения больше не могут перемешать свои записи в один и тот же файл
- **Исправлена потеря автосохранений при конкурентном доступе**: запрос на автосохранение, пришедший во время уже выполняющегося сохранения, больше не отбрасывается — он объединяется с текущим и повторяется
- **Исправлено поведение `AddKey`/`RenameKey`/`RenameSection`** (и их async-версий): теперь они всегда пробрасывают исключение после уведомления через событие `Error`, а не молча ничего не делают, если на событие есть подписчик
- **Исправлен `SetValuesAsync`**: теперь корректно передаёт `cancellationToken` в запись каждого элемента, раньше отмена могла игнорироваться посреди пакета
- **Исправлено некорректное формирование `ArgumentNullException`** во внутреннем файловом провайдере при `null`-пути к файлу (сообщение ошибочно передавалось как `paramName`)
- **Исправлено несовпадение обрезки пробельных символов** между синхронным и асинхронным путями разбора файла, из-за чего `GetData()`/`GetDataAsync()` могли по-разному разобрать один и тот же файл
- Исправлено молчаливое поглощение ошибки финального сохранения в `Dispose`/`DisposeAsync` в Release-сборках — теперь она трассируется и репортится через `Error`
- Исправлена документация события `Saved` (вызывается *после* сохранения, а не до) и `Loaded` (только при `Reload`/`ReloadAsync`, не при загрузке в конструкторе)
- Убраны устаревшие упоминания проверки контрольной суммы из документации `NeoIniOptions` — в NeoIniLight такой функции нет
- Обновлена зависимость `AsyncReaderWriterLock` до 1.0.3 (исправляет гонку disposed-блокировки в асинхронном медленном пути)

</details>

<details>
<summary><strong>1.0.2</strong> — 31 мая 2026</summary>

#### List of changes

- Исправлено описание: NeoIniLight — это конфигурационный фреймворк с INI-подобной сериализацией, а не классический парсер INI
- Добавлено предупреждение в README о том, что файлами управляет библиотека
- Уточнены теги и описание пакета в .csproj

</details>

<details>
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
- Потокобезопасность с помощью `AsyncReaderWriterLock`

</details>
