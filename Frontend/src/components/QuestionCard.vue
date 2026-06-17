<template>
  <div class="question-card">
    <h3 class="question-card__title">{{ question.text }}</h3>
    <div v-if="question.timeLimit > 0" class="question-card__timer">
      Осталось: {{ formatTime(timeLeft) }}
    </div>
    <div class="question-card__options">
      <label
        v-for="option in question.options"
        :key="option.id"
        class="question-card__option"
        :class="{ 'question-card__option--selected': isSelected(option.id) }"
      >
        <input
          :type="question.questionType === 'multiple' ? 'checkbox' : 'radio'"
          :name="`q-${question.id}`"
          :value="option.id"
          :checked="isSelected(option.id)"
          @change="handleSelect(option.id)"
          class="question-card__input"
        />
        <span class="question-card__label">{{ option.text }}</span>
      </label>
    </div>
  </div>
</template>

<script setup>
import { ref, watch, onUnmounted } from 'vue'

const props = defineProps({
  question: { type: Object, required: true },
  modelValue: { type: [Number, Array], default: null }
})

const emit = defineEmits(['update:modelValue'])

const timeLeft = ref(props.question.timeLimit || 0)
let timer = null

const isSelected = (optionId) => {
  if (props.question.questionType === 'multiple') {
    return Array.isArray(props.modelValue) && props.modelValue.includes(optionId)
  }
  return props.modelValue === optionId
}

const handleSelect = (optionId) => {
  if (props.question.questionType === 'multiple') {
    const current = Array.isArray(props.modelValue) ? [...props.modelValue] : []
    const index = current.indexOf(optionId)
    if (index > -1) {
      current.splice(index, 1)
    } else {
      current.push(optionId)
    }
    emit('update:modelValue', current)
  } else {
    emit('update:modelValue', optionId)
  }
}

const formatTime = (seconds) => {
  const mins = Math.floor(seconds / 60)
  const secs = seconds % 60
  return `${mins}:${secs.toString().padStart(2, '0')}`
}

if (props.question.timeLimit > 0) {
  timer = setInterval(() => {
    timeLeft.value--
    if (timeLeft.value <= 0) {
      clearInterval(timer)
      emit('time-up', props.question.id)
    }
  }, 1000)
}

onUnmounted(() => {
  if (timer) clearInterval(timer)
})
</script>

<style scoped>
.question-card {
  background: var(--bg-secondary);
  padding: 1.5rem;
  border-radius: 12px;
  box-shadow: 0 2px 4px rgba(6, 20, 27, 0.05);
}

.question-card__title {
  font-size: 1.1rem;
  color: var(--color-dark-navy);
  margin-bottom: 1rem;
}

.question-card__timer {
  background: var(--color-error-light);
  color: var(--color-error);
  padding: 0.5rem 1rem;
  border-radius: 6px;
  font-weight: 600;
  margin-bottom: 1rem;
  display: inline-block;
}

.question-card__options {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.question-card__option {
  display: flex;
  align-items: center;
  padding: 0.75rem 1rem;
  border: 2px solid var(--border-color);
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.2s ease;
}

.question-card__option:hover {
  border-color: var(--color-gray-blue);
}

.question-card__option--selected {
  border-color: var(--color-primary);
  background: rgba(37, 55, 69, 0.05);
}

.question-card__input {
  margin-right: 1rem;
  accent-color: var(--color-primary);
  width: 18px;
  height: 18px;
}

.question-card__label {
  color: var(--text-primary);
}
</style>