<template>
  <div class="edit">
    <h1 class="edit__title">Редактирование теста</h1>
    <div v-if="loading" class="edit__loading">Загрузка...</div>
    <form v-else @submit.prevent="saveTest" class="card edit__form">
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

      <div class="edit__questions">
        <h2 class="edit__subtitle">Вопросы</h2>
        <div v-for="(q, qIndex) in form.questions" :key="qIndex" class="card edit__question">
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
          <div class="edit__answers">
            <div v-for="(a, aIndex) in q.answers" :key="aIndex" class="edit__answer">
              <input v-model="a.text" type="text" class="form-input" placeholder="Вариант ответа" required />
              <label class="edit__checkbox">
                <input type="checkbox" v-model="a.isCorrect" />
                <span>Правильный</span>
              </label>
              <button type="button" @click="removeAnswer(qIndex, aIndex)" class="btn btn-danger btn-sm">
                Удалить
              </button>
            </div>
            <button type="button" @click="addAnswer(qIndex)" class="btn btn-outline btn-sm">
              Добавить вариант
            </button>
          </div>
          <button type="button" @click="removeQuestion(qIndex)" class="btn btn-danger">
            Удалить вопрос
          </button>
        </div>
        <button type="button" @click="addQuestion" class="btn btn-secondary">
          Добавить вопрос
        </button>
      </div>

      <button type="submit" class="btn btn-primary edit__submit">
        Сохранить изменения
      </button>
    </form>
  </div>
</template>

<script setup>
import { reactive, ref, onMounted } from 'vue'

const props = defineProps({
  testId: { type: [Number, String], required: true }
})

const emit = defineEmits(['navigate'])

const loading = ref(true)

const form = reactive({
  title: '',
  description: '',
  showCorrectAnswers: true,
  totalTimeLimit: 0,
  questions: []
})

onMounted(async () => {
  try {
    const response = await fetch(`/api/tests/${props.testId}`)
    if (response.ok) {
      const test = await response.json()
      form.title = test.title
      form.description = test.description || ''
      form.showCorrectAnswers = test.showCorrectAnswers !== false
      form.totalTimeLimit = test.totalTimeLimit || 0
      form.questions = (test.questions || []).map(q => ({
        text: q.text,
        questionType: q.questionType || 'single',
        timeLimit: q.timeLimit || 0,
        answers: (q.options || []).map(a => ({
          text: a.text,
          isCorrect: a.isCorrect || false
        }))
      }))
    }
  } catch (e) {
    console.error(e)
  } finally {
    loading.value = false
  }
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

const saveTest = async () => {
  for (let i = 0; i < form.questions.length; i++) {
    const question = form.questions[i]
    const hasCorrectAnswer = question.answers.some(a => a.isCorrect)
    
    if (!hasCorrectAnswer) {
      alert(`В вопросе ${i + 1} не выбран правильный ответ! Отметьте хотя бы один вариант как правильный.`)
      return
    }
  }

  try {
    const response = await fetch(`/api/tests/${props.testId}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(form)
    })
    if (response.ok) {
      alert('Тест успешно обновлён!')
      emit('navigate', 'my-tests')
    } else {
      const errorData = await response.json()
      throw new Error(errorData.message || 'Не удалось сохранить тест')
    }
  } catch (e) {
    console.error(e)
    alert('Ошибка: ' + e.message)
  }
}
</script>

<style scoped>
.edit__title {
  color: var(--color-dark-navy);
  margin-bottom: 1.5rem;
}

.edit__loading {
  text-align: center;
  padding: 2rem;
  color: var(--text-secondary);
}

.edit__subtitle {
  font-size: 1.25rem;
  margin-bottom: 1rem;
  color: var(--color-dark-navy);
}

.edit__questions {
  margin-bottom: 2rem;
}

.edit__question {
  margin-bottom: 1rem;
  border-left: 4px solid var(--color-primary);
}

.edit__answers {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
  margin-bottom: 1rem;
}

.edit__answer {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.edit__answer .form-input {
  flex: 1;
}

.edit__checkbox {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  cursor: pointer;
  white-space: nowrap;
}

.edit__submit {
  width: 100%;
  margin-top: 1rem;
}

.btn-sm {
  padding: 0.4rem 0.8rem;
  font-size: 0.875rem;
}
</style>