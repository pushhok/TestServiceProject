import { ref } from 'vue'

export function useTestSubmit() {
  const isSubmitting = ref(false)
  const error = ref(null)
  const result = ref(null)

  async function submitTest(testId, answers) {
    isSubmitting.value = true
    error.value = null
    result.value = null

    try {
      const response = await fetch(`/api/tests/${testId}/submit`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({ answers })
      })

      if (!response.ok) {
        throw new Error('Failed to submit test')
      }

      result.value = await response.json()
      return result.value
    } catch (err) {
      error.value = err.message
      throw err
    } finally {
      isSubmitting.value = false
    }
  }

  return {
    isSubmitting,
    error,
    result,
    submitTest
  }
}