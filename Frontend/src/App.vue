<template>
  <div id="app">
    <NavBar @navigate="navigate" />
    <main class="main">
      <HomeView v-if="currentView === 'home'" @navigate="navigate" @take-test="openTest" />
      <MyTestsView v-if="currentView === 'my-tests'" ref="myTestsRef" @navigate="navigate" @edit-test="openEditTest" />
      <TestCreateView v-if="currentView === 'create'" @navigate="navigate" @test-created="onTestCreated" />
      <TestEditView v-if="currentView === 'edit'" :test-id="editingTestId" @navigate="navigate" />
      <TestTakeView v-if="currentView === 'take'" :test-id="selectedTestId" @test-completed="showResult" />
      <TestResultView v-if="currentView === 'result'" :result="testResult" @navigate="navigate" />
      <ResultsView v-if="currentView === 'results'" />
      <LoginView v-if="currentView === 'login'" @navigate="navigate" />
      <RegisterView v-if="currentView === 'register'" @navigate="navigate" />
    </main>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { isAuthenticated } from './services/auth'
import NavBar from './components/NavBar.vue'
import HomeView from './views/HomeView.vue'
import MyTestsView from './views/MyTestsView.vue'
import TestCreateView from './views/TestCreateView.vue'
import TestEditView from './views/TestEditView.vue'
import TestTakeView from './views/TestTakeView.vue'
import TestResultView from './views/TestResultView.vue'
import ResultsView from './views/ResultsView.vue'
import LoginView from './views/LoginView.vue'
import RegisterView from './views/RegisterView.vue'

const currentView = ref('home')
const selectedTestId = ref(null)
const editingTestId = ref(null)
const testResult = ref(null)
const myTestsRef = ref(null)

const navigate = (view) => {
  // Проверка авторизации для защищённых страниц
  const protectedViews = ['my-tests', 'create', 'edit', 'results']
  if (protectedViews.includes(view) && !isAuthenticated()) {
    currentView.value = 'login'
    return
  }
  
  currentView.value = view
}

const openTest = (testId) => {
  selectedTestId.value = testId
  currentView.value = 'take'
}

const openEditTest = (testId) => {
  editingTestId.value = testId
  currentView.value = 'edit'
}

const showResult = (result) => {
  testResult.value = result
  currentView.value = 'result'
}

const onTestCreated = () => {
  if (currentView.value === 'my-tests' && myTestsRef.value) {
    myTestsRef.value.refreshTests()
  }
  currentView.value = 'home'
}
</script>

<style scoped>
#app {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

.main {
  flex: 1;
  max-width: 1200px;
  margin: 0 auto;
  padding: 2rem 1rem;
  width: 100%;
}
</style>