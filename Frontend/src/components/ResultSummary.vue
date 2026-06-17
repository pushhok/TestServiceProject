<template>
  <div class="summary">
    <div
      v-for="(item, index) in details"
      :key="item.questionId || index"
      class="summary__item"
      :class="{
        'summary__item--correct': item.isCorrect,
        'summary__item--incorrect': !item.isCorrect
      }"
    >
      <p class="summary__question">{{ index + 1 }}. {{ item.questionText }}</p>
      <div class="summary__answers">
        <p v-if="item.userAnswerText" class="summary__user">
          <span class="summary__label">Ваш ответ:</span> {{ item.userAnswerText }}
        </p>
        <p v-if="item.correctAnswerText" class="summary__correct">
          <span class="summary__label">Правильный ответ:</span> {{ item.correctAnswerText }}
        </p>
      </div>
    </div>
    <div v-if="details.length === 0" class="summary__empty">
      <p>Нет данных для отображения</p>
    </div>
  </div>
</template>

<script setup>
defineProps({
  details: { type: Array, default: () => [] }
})
</script>

<style scoped>
.summary {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.summary__item {
  padding: 1rem;
  border-radius: 8px;
  border-left: 4px solid var(--border-color);
  background: var(--color-lightest);
}

.summary__item--correct {
  border-left-color: var(--color-success);
  background: var(--color-success-light);
}

.summary__item--incorrect {
  border-left-color: var(--color-error);
  background: var(--color-error-light);
}

.summary__question {
  font-weight: 600;
  color: var(--color-dark-navy);
  margin-bottom: 0.5rem;
}

.summary__answers {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
  font-size: 0.95rem;
}

.summary__user {
  color: var(--text-secondary);
}

.summary__correct {
  color: var(--color-success);
  font-weight: 600;
}

.summary__label {
  font-weight: 400;
  opacity: 0.8;
}

.summary__empty {
  text-align: center;
  padding: 2rem;
  color: var(--text-secondary);
}
</style>