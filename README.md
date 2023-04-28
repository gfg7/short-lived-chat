# short-lived-chat 
[![.NET](https://img.shields.io/badge/--512BD4?logo=.net&logoColor=ffffff)](https://dotnet.microsoft.com/)
[![JavaScript](https://img.shields.io/badge/--F7DF1E?logo=javascript&logoColor=000)](https://www.javascript.com/)
[![Docker](https://badgen.net/badge/icon/docker?icon=docker&label)](https://https://docker.com/)
[![Generic badge](https://img.shields.io/badge/SignalR-1.1.0-blue.svg)](https://www.nuget.org/packages/Microsoft.AspNetCore.SignalR.Core)
[![Generic badge](https://img.shields.io/badge/Redis-7.0.4-red.svg)](https://www.nuget.org/packages/Microsoft.Extensions.Caching.StackExchangeRedis)

Проект для практики работы с SignalR и Redis

Масштабируемый онлайн-чат для пустых коротких разговоров:

- Баннер объявлений о чатах - найди собеседника, заинтересовав его темой!
- Таймаут для ожидания собеседника в новосозданном чате - *в следуюший раз ты обязательно дождешься*
- Таймаут групповой активности, если диалог не клеится
- Таймаут активности пользователя в чате - кик всех неактивных

## Запуск

Необходимо: Docker

### Установка:
```
git clone https://github.com/gfg7/short-lived-chat.git
```

Переменные среды для сервера:
```
REDIS_HOST= string
```

Запуск через docker-compose:
```
docker-compose -f docker-compose.yml up -d --build
```

<details>

<summary>EN</summary>

Tutorial project for SignalR & Redis practice

Scalable WS chat for small talk:

Requirments: Docker

Installation:
```
git clone https://github.com/gfg7/short-lived-chat.git
```

Custom environmental variables of server:
```
REDIS_HOST= string
```

Start up via docker-compose file:
```
docker-compose -f docker-compose.yml up -d --build
```
</details>

[![ForTheBadge built-with-love](http://ForTheBadge.com/images/badges/built-with-love.svg)](https://GitHub.com/gfg7/)
