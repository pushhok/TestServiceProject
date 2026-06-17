<template>
  <nav class="nav">
    <div class="nav__container">
      <span class="nav__brand">TestService</span>
      <ul class="nav__list">
        <li class="nav__item">
          <button @click="$emit('navigate', 'home')" class="nav__button">Тесты</button>
        </li>
        <li class="nav__item" v-if="isAuthenticated()">
          <button @click="$emit('navigate', 'my-tests')" class="nav__button">Мои тесты</button>
        </li>
        <li class="nav__item" v-if="isAuthenticated()">
          <button @click="$emit('navigate', 'create')" class="nav__button">Создать</button>
        </li>
        <li class="nav__item" v-if="isAuthenticated()">
          <button @click="$emit('navigate', 'results')" class="nav__button">Результаты</button>
        </li>
        <li class="nav__item" v-if="!isAuthenticated()">
          <button @click="$emit('navigate', 'login')" class="nav__button">Войти</button>
        </li>
        <li class="nav__item" v-if="!isAuthenticated()">
          <button @click="$emit('navigate', 'register')" class="nav__button nav__button--primary">Регистрация</button>
        </li>
        <li class="nav__item nav__user" v-if="isAuthenticated()">
          <span class="nav__user-name">{{ user?.fullName || user?.email }}</span>
          <button @click="handleLogout" class="nav__button nav__button--danger">Выйти</button>
        </li>
      </ul>
    </div>
  </nav>
</template>

<script setup>
import { computed } from 'vue'
import { getUser, isAuthenticated, logout } from '../services/auth'

const user = computed(() => getUser())

const handleLogout = () => {
  logout()
  window.location.href = '/'
}

defineEmits(['navigate'])
</script>

<style scoped>
.nav {
  background: var(--color-dark-navy);
  padding: 1rem 2rem;
  box-shadow: 0 4px 6px rgba(6, 20, 27, 0.2);
  border-radius: 12px;
  margin-bottom: 2rem;
}

.nav__container {
  max-width: 1200px;
  margin: 0 auto;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.nav__brand {
  font-size: 1.5rem;
  font-weight: 700;
  color: white;
}

.nav__list {
  display: flex;
  gap: 1.5rem;
  list-style: none;
  margin: 0;
  padding: 0;
  align-items: center;
}

.nav__button {
  background: none;
  border: none;
  color: var(--color-light-gray);
  font-weight: 500;
  cursor: pointer;
  transition: color 0.2s ease;
  font-size: 1rem;
}

.nav__button:hover {
  color: white;
}

.nav__button--primary {
  background: var(--color-primary);
  color: white !important;
  padding: 0.5rem 1rem;
  border-radius: 6px;
}

.nav__button--primary:hover {
  background: var(--color-primary-hover);
}

.nav__button--danger {
  color: var(--color-error) !important;
}

.nav__user {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.nav__user-name {
  color: white;
  font-size: 0.9rem;
}

@media (max-width: 768px) {
  .nav__container {
    flex-direction: column;
    gap: 1rem;
  }
  
  .nav__list {
    flex-direction: column;
    text-align: center;
    gap: 0.75rem;
  }
}
</style>