# short-lived-chat
Tutorial project for SignalR & Redis practice

Scalable WS chat for small talk:

- created chat lives on only with no less than 2 members
- timeout of user activity in group
- timeout of chat group activity

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
