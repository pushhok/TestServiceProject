<template>
  <div class="auth">
    <div class="auth__card">
      <h1 class="auth__title">Регистрация</h1>
      
      <form @submit.prevent="handleRegister" class="auth__form">
        <div class="form-group">
          <label class="form-label">Email</label>
          <input 
            v-model="form.email" 
            type="email" 
            class="form-input" 
            required 
            placeholder="your@email.com"
          />
        </div>

        <div class="form-group">
          <label class="form-label">Имя (необязательно)</label>
          <input 
            v-model="form.fullName" 
            type="text" 
            class="form-input" 
            placeholder="Иван Иванов"
          />
        </div>

        <div class="form-group">
          <label class="form-label">Пароль</label>
          <input 
            v-model="form.password" 
            type="password" 
            class="form-input" 
            required 
            placeholder="Минимум 6 символов"
            minlength="6"
          />
        </div>

        <div v-if="error" class="auth__error">
          {{ error }}
        </div>

        <button type="submit" :disabled="loading" class="btn btn-primary auth__button">
          {{ loading ? 'Регистрация...' : 'Зарегистрироваться' }}
        </button>

        <p class="auth__text">
          Уже есть аккаунт? 
          <router-link to="/login" class="auth__link">Войти</router-link>
        </p>
      </form>
    </div>
  </div>
</template>

<script setup>
import { reactive, ref } from 'vue'
import { register, setToken, setUser } from '../services/auth'

const form = reactive({
  email: '',
  fullName: '',
  password: ''
})

const loading = ref(false)
const error = ref(null)

const handleRegister = async () => {
  loading.value = true
  error.value = null

  try {
    const data = await register(form.email, form.password, form.fullName)
    setToken(data.token)
    setUser({ id: data.id, email: data.email, fullName: data.fullName })
    
    // Перенаправляем на главную
    window.location.href = '/'
  } catch (e) {
    error.value = e.message
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.auth {
  min-height: 80vh;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 2rem 1rem;
}

.auth__card {
  background: var(--bg-secondary);
  padding: 2.5rem;
  border-radius: 12px;
  box-shadow: 0 4px 6px rgba(6, 20, 27, 0.1);
  max-width: 400px;
  width: 100%;
}

.auth__title {
  color: var(--color-dark-navy);
  margin-bottom: 2rem;
  text-align: center;
}

.auth__form {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.auth__error {
  background: var(--color-error-light);
  color: var(--color-error);
  padding: 0.75rem;
  border-radius: 8px;
  font-size: 0.9rem;
  text-align: center;
}

.auth__button {
  width: 100%;
  padding: 0.875rem;
  font-size: 1rem;
}

.auth__text {
  text-align: center;
  color: var(--text-secondary);
  margin: 0;
}

.auth__link {
  color: var(--color-primary);
  text-decoration: none;
  font-weight: 600;
}

.auth__link:hover {
  text-decoration: underline;
}
</style>