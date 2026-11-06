# WebApplicationStas — слоистая архитектура (Практическая работа №10)

ASP.NET Core Web API для предметной области «Магазин» (SHOPDB), переведённый на слоистую архитектуру Repository / Service / Controller с AutoMapper.

## Структура слоёв

```
Controllers  — тонкие контроллеры: вызывают сервис, переводят результат в HTTP-статус
Services     — бизнес-логика + AutoMapper; никаких HTTP-типов и DbContext
Repositories — доступ к данным (единственное место, где живёт ShopdbContext)
Mappings     — MappingProfile (AutoMapper)
DTOs         — DTO для чтения (с вложенными данными) и записи (с Id связанных сущностей)
Exceptions   — NotFoundException, BusinessRuleException + глобальный обработчик
Models       — EF-сущности (сгенерированы из БД в ПР №1, дополнены ProductReservation)
```

## Связь сущностей

Один-ко-многим: `Category → Products`, `User → Orders`, `Order → OrderItems`, `User → Carts`, `Cart → CartItems`, `User → ProductReservations`, `Product → ProductReservations`.

## Бизнес-операции (ReservationsController)

- **`POST /api/reservations`** — резервирование товара: уменьшает `Product.StockQuantity` и создаёт запись `ProductReservation` (кто — UserId, что — ProductId + Quantity, когда — ReservedAt). Проверки: количество > 0 (409), пользователь/товар существуют (404), достаточный остаток (409 с понятным сообщением). Оба изменения сохраняются **одним вызовом `SaveChangesAsync`** — EF Core сам оборачивает его в транзакцию.
- **`POST /api/reservations/{id}/release`** — обратная операция: возвращает quantity на склад и закрывает запись (`ReleasedAt`). Если активной записи нет (не существует или уже закрыта) — 404.

## Время жизни сервисов — Scoped

`ShopdbContext` зарегистрирован как Scoped и не потокобезопасен. Все репозитории и сервисы — Scoped, поэтому в пределах одного HTTP-запроса они разделяют один экземпляр контекста:

- `Singleton` нельзя: контекст использовался бы параллельно несколькими запросами — гонки и повреждение состояния.
- `Transient` сломал бы бизнес-операцию: у сервиса резервирований и сервиса товаров оказались бы разные контексты, и один `SaveChangesAsync` не зафиксировал бы оба изменения атомарно.
- `Scoped` даёт одну единицу работы на запрос без отдельного UnitOfWork.

## HTTP-поведение

- Списки: 200 + заголовок `X-Total-Count` (пагинация `?page=&pageSize=`)
- Получение по id: 200 / 404
- Создание: 201 + Location
- PUT/PATCH: 204 / 404
- DELETE: 204 / 404
- Нарушение бизнес-правил: 409 (ProblemDetails), отсутствие ресурса: 404 — через `GlobalExceptionHandler` (`IExceptionHandler`)

