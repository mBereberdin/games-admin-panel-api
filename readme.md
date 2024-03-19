# Games admin panel

<!-- Актуализировать после импорта -->

[![Build](https://github.com/mBereberdin/games-admin-panel-api/actions/workflows/BuildAndTest.yml/badge.svg)](https://github.com/mBereberdin/games-admin-panel-api/actions/workflows/BuildAndTest.yml)

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

### Собрать приложение в docker образ

1. Перейти в директорию: `games-admin-panel-api/`;
2. Выполнить команду:

```powershell
docker build -t games-admin-panel:latest .
```

#### Собрать контейнер образа

2. Выполнить команду:

```powershell
docker create --name LOCAL-games-admin-panel games-admin-panel:latest
```

## Дополнительные пояснения

### Data annotations vs fluent api

В рамках данного проекта (как минимум - пока что) используются аннотации, потому что:

1. Так удобнее видеть логику непосредственно в моделях, при взаимодействии с ними;
2. Вряд ли модели этого проекта будут использоваться в каком-либо еще, не смотря на то, что они были удобно вынесены в отдельный проект (для возможного переиспользования и "чистой архитектуры").
   При внедрении аннотации данных эти модели стали более жестко привязаны к этому проекту. В будущем, если потребуется переход на fluent api или переиспользования моделей - удастся придерживаться
   более "гибкой" конструкции, когда в модели не намешивается бизнес логика, а вся она находится непосредственно в контексте базы данных. Сейчас это не критично, проще и быстрее, поэтому - так.

## Используемые технологии

### Общая часть

- .net core 7

### Database

- EntityFrameworkCore
- EntityFrameworkCore.PostgreSQL
- Serilog

### Infrastructure

- Serilog
- Mapster

### Tests

- xUnit
- FluentAssertions
- EntityFrameworkCore.Sqlite
- Moq
- Moq.EntityFrameworkCore
- coverlet.collector

### WebApi

- Mapster
- Serilog
- Swashbuckle.AspNetCore
