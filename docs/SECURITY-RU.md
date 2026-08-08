[![EN](https://img.shields.io/badge/SECURITY-EN-2D2D2D?style=for-the-badge&logo=github&logoColor=FFFFFF)](https://github.com/Lonewolf239/NeoIniLight/blob/main/docs/SECURITY.md)
[![RU](https://img.shields.io/badge/SECURITY-RU-2D2D2D?style=for-the-badge&logo=google-translate&logoColor=FFFFFF)](https://github.com/Lonewolf239/NeoIniLight/blob/main/docs/SECURITY-RU.md)

## Security policy · NeoIniLight

Сообщайте об уязвимостях ответственно, чтобы они были исправлены до публичного раскрытия.

---

### Supported versions

NeoIniLight использует многоуровневую модель поддержки:

| Abbreviation | Meaning | Scope |
|--------------|---------|-------|
| **CSR** | Current Stable Release | Полные security-патчи и исправления багов |
| **LSR** | Long-term Stable Release | Без патчей — доступны только для скачивания |
| **DSR** | Deprecated Stable Release | Без патчей — настоятельно рекомендуется обновление |

| Version | Status | Support |
|---------|--------|---------|
| 1.0.5 | **CSR** | Security-патчи + исправления багов |
| 1.0.4 | LSR | Доступна для скачивания |
| 1.0.3 | **LSR** | Доступна для скачивания |
| 1.0.2 | **LSR** | Доступна для скачивания |
| 1.0.1 | **LSR** | Доступна для скачивания |
| 1.0 | **DSR** | Неполные, багованные или устаревшие версии |

---

### Reporting a vulnerability

**Не создавайте публичный issue для уязвимостей безопасности.**

Свяжитесь с мейнтейнером напрямую:

- **Telegram:** [@an1onime](https://t.me/an1onime)

Укажите в отчёте:

1. **Затронутые версии.**
2. **Описание** уязвимости и её влияния.
3. **Шаги воспроизведения** — минимальный код или конфигурация для воспроизведения проблемы.
4. **Предлагаемое исправление** (если есть).

Вы получите подтверждение в течение **48 часов**. Исправление будет разработано приватно и выпущено как патч до публичного раскрытия.

---

### Scope

Следующие области входят в scope security-репортов:

- Race conditions файлового I/O (атомарные записи, временные файлы).
- Проблемы потокобезопасности при конкурентном доступе.

Вне scope: denial-of-service через намеренно некорректные INI-файлы экстремальных размеров.
