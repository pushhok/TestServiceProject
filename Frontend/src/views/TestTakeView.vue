<template>
  <div class="test-take">
    <div v-if="loading" class="test-take__loading">Загрузка теста...</div>
    <div v-else-if="error" class="test-take__error">{{ error }}</div>
    <div v-else class="test-take__content">
      <h1 class="test-take__title">{{ test.title }}</h1>
      <p class="test-take__desc">{{ test.description }}</p>
      
      <div v-if="test.totalTimeLimit > 0" class="test-take__global-timer">
        Общее время: {{ formatTime(globalTimeLeft) }}
      </div>

      <form @submit.prevent="handleSubmit" class="test-take__form">
        <QuestionCard
          v-for="q in test.questions"
          :key="q.id"
          :question="q"
          v-model="answers[q.id]"
          @time-up="handleTimeUp"
        />
        <button type="submit" :disabled="isSubmitting" class="btn btn-primary test-take__submit">
          {{ isSubmitting ? 'Проверка...' : 'Завершить тест' }}
        </button>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted, onUnmounted } from 'vue'
import { getTestById } from '../services/api'
import { useTestSubmit } from '../composables/useTestSubmit'
import QuestionCard from '../components/QuestionCard.vue'

const props = defineProps({
  testId: { type: [Number, String], required: true }
})

const emit = defineEmits(['test-completed'])

const { isSubmitting, submitTest } = useTestSubmit()

const test = reactive({ title: '', description: '', totalTimeLimit: 0, questions: [] })
const answers = ref({})
const loading = ref(true)
const error = ref(null)
const globalTimeLeft = ref(0)
let globalTimer = null

onMounted(async () => {
  try {
    const data = await getTestById(props.testId)
    Object.assign(test, data)
    
    if (test.totalTimeLimit > 0) {
      globalTimeLeft.value = test.totalTimeLimit * 60
      globalTimer = setInterval(() => {
        globalTimeLeft.value--
        if (globalTimeLeft.value <= 0) {
          clearInterval(globalTimer)
          handleSubmit()
        }
      }, 1000)
    }
  } catch (e) {
    error.value = 'Не удалось загрузить тест'
  } finally {
    loading.value = false
  }
})

onUnmounted(() => {
  if (globalTimer) clearInterval(globalTimer)
})

const formatTime = (seconds) => {
  const mins = Math.floor(seconds / 60)
  const secs = seconds % 60
  return `${mins}:${secs.toString().padStart(2, '0')}`
}

const handleTimeUp = (questionId) => {
  if (!answers.value[questionId]) {
    answers.value[questionId] = []
  }
}

const handleSubmit = async () => {
  if (globalTimer) clearInterval(globalTimer)
  
  try {
    const result = await submitTest(props.testId, answers.value)
    emit('test-completed', result)
  } catch (e) {
    error.value = 'Ошибка при отправке ответов'
  }
}
</script>

<style scoped>
.test-take__title {
  color: var(--color-dark-navy);
  margin-bottom: 0.5rem;
}

.test-take__desc {
  color: var(--text-secondary);
  margin-bottom: 2rem;
}

.test-take__global-timer {
  background: var(--color-error-light);
  color: var(--color-error);
  padding: 1rem;
  border-radius: 8px;
  font-weight: 600;
  font-size: 1.2rem;
  text-align: center;
  margin-bottom: 2rem;
}

.test-take__form {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.test-take__submit {
  width: 100%;
  padding: 1rem;
  font-size: 1.1rem;
}

.test-take__loading,
.test-take__error {
  text-align: center;
  padding: 3rem;
  color: var(--text-secondary);
  background: var(--bg-secondary);
  border-radius: 12px;
}

.test-take__error {
  color: var(--color-error);
}
</style>