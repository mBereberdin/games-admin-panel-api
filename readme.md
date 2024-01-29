# Games admin panel

<!-- Актуализировать после импорта -->
[![Build](https://github.com/mBereberdin/games-admin-panel-api/actions/workflows/Build.yml/badge.svg)](https://github.com/mBereberdin/games-admin-panel-api/actions/workflows/Build.yml)

## Описание проекта

Backend приложение для администрирования игр.

Основной функционал:

- регистрация пользователей;
- выдача пользователям токенов для взаимодействия с играми;
- взаимодействие с правами игр (создание, удаление, назначение).

## How to

### Добавить миграцию

1. Перейти в директорию: `src/Database/`;
2. Выполнить команду:

```powershell
dotnet ef migrations add <MigrationName> -s ../WebApi 
```

## Используемые технологии

- .net core 7
- swagger
- serilog
- mapster
- ef core
