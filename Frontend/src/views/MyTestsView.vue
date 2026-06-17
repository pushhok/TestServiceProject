<template>
  <div class="my-tests">
    <h1 class="my-tests__title">Мои тесты</h1>
    <div v-if="tests.length === 0" class="my-tests__empty">
      <p>У вас пока нет созданных тестов</p>
      <button @click="$emit('navigate', 'create')" class="btn btn-primary">
        Создать первый тест
      </button>
    </div>
    <div v-else class="my-tests__grid">
      <div v-for="test in tests" :key="test.id" class="card test-card">
        <div class="test-card__body">
          <h2 class="test-card__title">{{ test.title }}</h2>
          <p class="test-card__desc">{{ test.description }}</p>
          <span class="badge badge-primary">{{ test.questionsCount }} вопросов</span>
        </div>
        <div class="test-card__actions">
          <button @click="editTest(test.id)" class="btn btn-secondary btn-sm">
            Редактировать
          </button>
          <button @click="deleteTest(test.id)" class="btn btn-danger btn-sm">
            Удалить
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { getMyTests, deleteTest as deleteTestApi } from '../services/api'

const tests = ref([])
const emit = defineEmits(['navigate', 'edit-test'])

const loadTests = async () => {
  try {
    tests.value = await getMyTests()
    console.log('Loaded tests:', tests.value)
  } catch (e) {
    console.error('Error loading tests:', e)
  }
}

onMounted(() => {
  loadTests()
})

const editTest = (testId) => {
  emit('edit-test', testId)
}

const deleteTest = async (testId) => {
  if (!confirm('Вы уверены, что хотите удалить этот тест?')) return
  
  try {
    await deleteTestApi(testId)
    tests.value = tests.value.filter(t => t.id !== testId)
  } catch (e) {
    console.error('Error deleting test:', e)
  }
}

// Экспортируем функцию для обновления списка
defineExpose({
  refreshTests: loadTests
})
</script>

<style scoped>
.my-tests__title {
  color: var(--color-dark-navy);
  margin-bottom: 2rem;
}

.my-tests__grid {
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
  gap: 0.75rem;
  justify-content: flex-end;
}

.my-tests__empty {
  text-align: center;
  padding: 3rem;
  background: var(--bg-secondary);
  border-radius: 12px;
}

.my-tests__empty p {
  margin-bottom: 1.5rem;
  color: var(--text-secondary);
}
</style>