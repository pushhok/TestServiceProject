# TestService

Веб-приложение для создания и прохождения тестов.

## Стек технологий

**Backend:**
- ASP.NET Core 8 (Minimal API)
- Entity Framework Core + SQLite
- JWT аутентификация
- BCrypt для хеширования паролей

**Frontend:**
- Vue 3
- Vite

## Возможности

- Регистрация и авторизация (JWT)
- Создание тестов с вопросами
- Одиночный и множественный выбор ответов
- Таймер на тест и на каждый вопрос
- Автопроверка результатов
- Разбор ответов
- Разделение прав (создатель/проходящий)

## Запуск

### Backend
cd Backend
dotnet run

### Frontend
cd Frontend
npm install
npm run dev
