<template>
  <div class="home">
    <div class="home__header">
      <h1 class="home__title">Доступные тесты</h1>
      <button @click="$emit('navigate', 'create')" class="btn btn-primary">Создать тест</button>
    </div>
    <div v-if="tests.length === 0" class="home__empty">
      <p>Тесты пока не созданы.</p>
    </div>
    <div v-else class="home__grid">
      <div v-for="test in tests" :key="test.id" class="card test-card">
        <div class="test-card__body">
          <h2 class="test-card__title">{{ test.title }}</h2>
          <p class="test-card__desc">{{ test.description }}</p>
          <span class="badge badge-primary">{{ test.questionsCount }} вопросов</span>
        </div>
        <div class="test-card__actions">
          <button @click="$emit('take-test', test.id)" class="btn btn-primary">Пройти</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { getTests } from '../services/api'

const tests = ref([])

onMounted(async () => {
  try {
    tests.value = await getTests()
  } catch (e) {
    console.error(e)
  }
})

defineEmits(['navigate', 'take-test'])
</script>

<style scoped>
.home__header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
}

.home__title {
  color: var(--color-dark-navy);
}

.home__grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 1.5rem;
}

.test-card {
  display: flex;
  flex-direction: column;
  justify-content: space-between;
}

.test-card__body {
  margin-bottom: 1.5rem;
}

.test-card__title {
  font-size: 1.25rem;
  margin-bottom: 0.5rem;
  color: var(--color-dark-navy);
}

.test-card__desc {
  color: var(--text-secondary);
  margin-bottom: 1rem;
}

.test-card__actions {
  display: flex;
  justify-content: flex-end;
}

.home__empty {
  text-align: center;
  padding: 3rem;
  color: var(--text-light);
  background: var(--bg-secondary);
  border-radius: 12px;
}
</style>