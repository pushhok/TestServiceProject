<template>
  <div class="results">
    <h1 class="results__title">Результаты тестов</h1>
    <div v-if="results.length === 0" class="results__empty">
      <p>Вы еще не проходили тесты</p>
    </div>
    <div v-else class="results__list">
      <div v-for="result in results" :key="result.id" class="card result-card">
        <div class="result-card__header">
          <h2 class="result-card__title">{{ result.testName }}</h2>
          <div class="result-card__score">
            <span class="result-card__score-value">{{ result.score }} / {{ result.totalQuestions }}</span>
            <span class="result-card__score-percent">{{ Math.round((result.score / result.totalQuestions) * 100) }}%</span>
          </div>
          <p class="result-card__date">{{ formatDate(result.completedAt) }}</p>
        </div>
        <div v-if="result.details && result.details.length > 0" class="result-card__details">
          <h3 class="result-card__subtitle">Разбор ответов</h3>
          <div class="result-card__answers">
            <div
              v-for="(detail, index) in result.details"
              :key="index"
              class="answer-item"
              :class="{'answer-item--correct': detail.isCorrect, 'answer-item--incorrect': !detail.isCorrect}"
            >
              <div class="answer-item__header">
                <p class="answer-item__question">{{ index + 1 }}. {{ detail.questionText }}</p>
                <span class="answer-item__score">{{ detail.isCorrect ? '1/1' : '0/1' }}</span>
              </div>
              <div class="answer-item__answers">
                <p class="answer-item__user" v-if="detail.userAnswerText">
                  <span>Ваш ответ:</span> {{ detail.userAnswerText }}
                </p>
                <p class="answer-item__correct" v-if="!detail.isCorrect && detail.correctAnswerText">
                  <span>Правильный ответ:</span> {{ detail.correctAnswerText }}
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'

const results = ref([])

onMounted(async () => {
  try {
    const response = await fetch('/api/results')
    if (response.ok) {
      const data = await response.json()
      console.log('Loaded results:', data)
      results.value = data
    }
  } catch (e) {
    console.error('Error loading results:', e)
  }
})

const formatDate = (dateStr) => {
  const date = new Date(dateStr)
  return date.toLocaleString('ru-RU')
}
</script>

<style scoped>
.results__title {
  color: var(--color-dark-navy);
  margin-bottom: 2rem;
}

.results__list {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.result-card {
  padding: 1.5rem;
}

.result-card__header {
  margin-bottom: 1.5rem;
  padding-bottom: 1rem;
  border-bottom: 2px solid var(--border-light);
}

.result-card__title {
  font-size: 1.25rem;
  margin-bottom: 0.5rem;
  color: var(--color-dark-navy);
}

.result-card__score {
  display: flex;
  align-items: baseline;
  gap: 1rem;
  margin-bottom: 0.5rem;
}

.result-card__score-value {
  font-size: 1.5rem;
  font-weight: 700;
  color: var(--color-primary);
}

.result-card__score-percent {
  font-size: 1.1rem;
  color: var(--text-secondary);
}

.result-card__date {
  color: var(--text-light);
  font-size: 0.9rem;
}

.result-card__subtitle {
  font-size: 1.1rem;
  color: var(--color-dark-navy);
  margin-bottom: 1rem;
}

.result-card__answers {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.answer-item {
  padding: 1rem;
  border-radius: 8px;
  border-left: 4px solid var(--border-color);
  background: var(--color-lightest);
}

.answer-item--correct {
  border-left-color: var(--color-success);
  background: var(--color-success-light);
}

.answer-item--incorrect {
  border-left-color: var(--color-error);
  background: var(--color-error-light);
}

.answer-item__header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 0.5rem;
}

.answer-item__question {
  font-weight: 600;
  color: var(--color-dark-navy);
  margin: 0;
  flex: 1;
}

.answer-item__score {
  font-weight: 700;
  font-size: 1.1rem;
  color: var(--color-dark-navy);
  margin-left: 1rem;
}

.answer-item--correct .answer-item__score {
  color: var(--color-success);
}

.answer-item--incorrect .answer-item__score {
  color: var(--color-error);
}

.answer-item__answers {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
  font-size: 0.95rem;
}

.answer-item__user {
  color: var(--text-secondary);
  margin: 0;
}

.answer-item__correct {
  color: var(--color-success);
  font-weight: 600;
  margin: 0;
}

.answer-item__user span,
.answer-item__correct span {
  font-weight: 400;
  opacity: 0.8;
}

.results__empty {
  text-align: center;
  padding: 3rem;
  color: var(--text-secondary);
  background: var(--bg-secondary);
  border-radius: 12px;
}
</style>