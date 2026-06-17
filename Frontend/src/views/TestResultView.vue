<template>
  <div class="result">
    <div v-if="!result" class="result__empty">
      <p>Результаты не найдены.</p>
      <button @click="$emit('navigate', 'home')" class="btn btn-primary">На главную</button>
    </div>
    <div v-else class="result__content">
      <div class="card result__summary">
        <h1 class="result__title">Результаты теста</h1>
        
        <div class="result__score">
          <span class="result__score-value">{{ result.score }} / {{ result.totalQuestions }}</span>
          <span class="result__score-percent">{{ result.percentage }}%</span>
        </div>

        <div v-if="result.showCorrectAnswers && result.details && result.details.length > 0" class="result__details">
          <h2 class="result__subtitle">Разбор ответов</h2>
          <ResultSummary :details="result.details" />
        </div>
        <div v-else-if="result.showCorrectAnswers" class="result__no-details">
          <p>Разбор ответов недоступен</p>
        </div>
        <div v-else class="result__no-details">
          <p>Правильные ответы скрыты создателем теста</p>
        </div>

        <button @click="$emit('navigate', 'home')" class="btn btn-primary result__back">К списку тестов</button>
      </div>
    </div>
  </div>
</template>

<script setup>
import ResultSummary from '../components/ResultSummary.vue'

defineProps({
  result: { type: Object, default: null }
})

defineEmits(['navigate'])
</script>

<style scoped>
.result__title {
  color: var(--color-dark-navy);
  margin-bottom: 1.5rem;
  text-align: center;
}

.result__score {
  display: flex;
  flex-direction: column;
  align-items: center;
  margin: 2rem 0;
  padding: 1.5rem;
  background: var(--color-lightest);
  border-radius: 12px;
}

.result__score-value {
  font-size: 2.5rem;
  font-weight: 700;
  color: var(--color-primary);
}

.result__score-percent {
  font-size: 1.25rem;
  color: var(--text-secondary);
}

.result__subtitle {
  font-size: 1.25rem;
  color: var(--color-dark-navy);
  margin-bottom: 1rem;
}

.result__no-details {
  text-align: center;
  padding: 2rem;
  color: var(--text-secondary);
  background: var(--bg-secondary);
  border-radius: 8px;
  margin: 1.5rem 0;
}

.result__back {
  display: block;
  width: 100%;
  text-align: center;
  margin-top: 1.5rem;
}

.result__empty {
  text-align: center;
  padding: 3rem;
}
</style>