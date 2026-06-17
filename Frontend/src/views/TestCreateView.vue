<template>
  <div class="create">
    <h1 class="create__title">Создание теста</h1>
    <form @submit.prevent="submitTest" class="card create__form">
      <div class="form-group">
        <label class="form-label">Название теста</label>
        <input v-model="form.title" type="text" class="form-input" required />
      </div>
      <div class="form-group">
        <label class="form-label">Описание</label>
        <textarea v-model="form.description" class="form-textarea"></textarea>
      </div>
      <div class="form-group">
        <label class="form-label">
          <input type="checkbox" v-model="form.showCorrectAnswers" />
          Показывать правильные ответы после прохождения
        </label>
      </div>
      <div class="form-group">
        <label class="form-label">Общий таймер теста (минут, 0 = без ограничения)</label>
        <input v-model.number="form.totalTimeLimit" type="number" min="0" class="form-input" />
      </div>

      <div class="create__questions">
        <h2 class="create__subtitle">Вопросы</h2>
        <div v-for="(q, qIndex) in form.questions" :key="qIndex" class="card create__question">
          <div class="form-group">
            <label class="form-label">Текст вопроса {{ qIndex + 1 }}</label>
            <input v-model="q.text" type="text" class="form-input" required />
          </div>
          <div class="form-group">
            <label class="form-label">Тип вопроса</label>
            <select v-model="q.questionType" class="form-input">
              <option value="single">Один правильный ответ</option>
              <option value="multiple">Несколько правильных ответов</option>
            </select>
          </div>
          <div class="form-group">
            <label class="form-label">Таймер на вопрос (секунд, 0 = без ограничения)</label>
            <input v-model.number="q.timeLimit" type="number" min="0" class="form-input" />
          </div>
          <div class="create__answers">
            <div v-for="(a, aIndex) in q.answers" :key="aIndex" class="create__answer">
              <input v-model="a.text" type="text" class="form-input" placeholder="Вариант ответа" required />
              <label class="create__checkbox">
                <input type="checkbox" v-model="a.isCorrect" />
                <span>Правильный</span>
              </label>
              <button type="button" @click="removeAnswer(qIndex, aIndex)" class="btn btn-danger btn-sm">Удалить</button>
            </div>
            <button type="button" @click="addAnswer(qIndex)" class="btn btn-outline btn-sm">Добавить вариант</button>
          </div>
          <button type="button" @click="removeQuestion(qIndex)" class="btn btn-danger">Удалить вопрос</button>
        </div>
        <button type="button" @click="addQuestion" class="btn btn-secondary">Добавить вопрос</button>
      </div>

      <button type="submit" class="btn btn-primary create__submit">Сохранить тест</button>
    </form>
  </div>
</template>

<script setup>
import { reactive } from 'vue'

const emit = defineEmits(['test-created', 'navigate'])

const form = reactive({
  title: '',
  description: '',
  showCorrectAnswers: true,
  totalTimeLimit: 0,
  questions: [
    {
      text: '',
      questionType: 'single',
      timeLimit: 0,
      answers: [{ text: '', isCorrect: false }]
    }
  ]
})

const addQuestion = () => {
  form.questions.push({
    text: '',
    questionType: 'single',
    timeLimit: 0,
    answers: [{ text: '', isCorrect: false }]
  })
}

const removeQuestion = (index) => {
  form.questions.splice(index, 1)
}

const addAnswer = (qIndex) => {
  form.questions[qIndex].answers.push({ text: '', isCorrect: false })
}

const removeAnswer = (qIndex, aIndex) => {
  form.questions[qIndex].answers.splice(aIndex, 1)
}

const submitTest = async () => {
  for (let i = 0; i < form.questions.length; i++) {
    const question = form.questions[i]
    const hasCorrectAnswer = question.answers.some(a => a.isCorrect)
    
    if (!hasCorrectAnswer) {
      alert(`В вопросе ${i + 1} не выбран правильный ответ! Отметьте хотя бы один вариант как правильный.`)
      return
    }
  }

  try {
    const response = await fetch('/api/tests', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(form)
    })
    
    if (!response.ok) {
      const errorData = await response.json()
      throw new Error(errorData.message || 'Не удалось создать тест')
    }
    
    form.title = ''
    form.description = ''
    form.showCorrectAnswers = true
    form.totalTimeLimit = 0
    form.questions = [{
      text: '',
      questionType: 'single',
      timeLimit: 0,
      answers: [{ text: '', isCorrect: false }]
    }]
    
    emit('test-created')
    alert('Тест успешно создан!')
  } catch (e) {
    console.error(e)
    alert('Ошибка: ' + e.message)
  }
}
</script>

<style scoped>
.create__title {
  color: var(--color-dark-navy);
  margin-bottom: 1.5rem;
}

.create__subtitle {
  font-size: 1.25rem;
  margin-bottom: 1rem;
  color: var(--color-dark-navy);
}

.create__questions {
  margin-bottom: 2rem;
}

.create__question {
  margin-bottom: 1rem;
  border-left: 4px solid var(--color-primary);
}

.create__answers {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
  margin-bottom: 1rem;
}

.create__answer {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.create__answer .form-input {
  flex: 1;
}

.create__checkbox {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  cursor: pointer;
  white-space: nowrap;
}

.create__submit {
  width: 100%;
  margin-top: 1rem;
}

.btn-sm {
  padding: 0.4rem 0.8rem;
  font-size: 0.875rem;
}
</style>