<template>
    <div class="container">
    <h1>💰 Expense Manager</h1>

    <div class="form">
      <input v-model="description" placeholder="Description" />

      <input
        v-model.number="amount"
        type="number"
        step="0.01"
        placeholder="Amount"
      />

      <input v-model="category" placeholder="Category" />

      <input v-model="date" type="date" />

      <button @click="addExpense">Add</button>
    </div>

    <div class = "table-wrapper" v-if="expenses.length">
      <table>
        <thead>
          <tr>
            <th>Description</th>
            <th>Amount</th>
            <th>Category</th>
            <th>Date</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="expense in expenses" :key="expense.id">
            <!-- Description -->
            <td v-if="editingId !== expense.id">
              {{ expense.description }}
            </td>
            <td v-else>
              <input v-model="expense.description" />
            </td>

            <!-- Amount -->
            <td v-if="editingId !== expense.id">
              ${{ Number(expense.amount).toFixed(2) }}
            </td>
            <td v-else>
              <input v-model.number="expense.amount" type="number" step="0.01" />
            </td>

            <!-- Category -->
            <td v-if="editingId !== expense.id">
              {{ expense.category }}
            </td>
            <td v-else>
              <input v-model="expense.category" />
            </td>

            <!-- Date -->
            <td v-if="editingId !== expense.id">
              {{ formatDate(expense.date) }}
            </td>
            <td v-else>
              <input v-model="expense.date" type="date" />
            </td>

            <!-- Actions -->
            <td>
              <div class="actions">
                <button
                  v-if="editingId !== expense.id"
                  @click="startEdit(expense.id)"
                >
                  Edit
                </button>

                <button
                  v-else
                  @click="updateExpense(expense)"
                >
                  Save
                </button>

                <button
                  class="delete"
                  @click="deleteExpense(expense.id)"
                >
                  Delete
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <div class="total" v-if="expenses.length">
      Total: ${{ totalAmount.toFixed(2) }}
    </div>

    <p v-else>No expenses yet.</p>

    <!-- Toast -->
    <div v-if="toast.show" :class="['toast', toast.type]">
      {{ toast.message }}
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'

const API_URL =
  import.meta.env.VITE_API_URL ||
  'http://localhost:5246/api/expense'

/* ===========================
   State
   =========================== */
const expenses = ref([])
const description = ref('')
const amount = ref(null)
const category = ref('')
const date = ref('')
const editingId = ref(null)

/* Toast state */
const toast = ref({
  show: false,
  message: '',
  type: 'success'
})

/* ===========================
   Computed
   =========================== */
const totalAmount = computed(() =>
  expenses.value.reduce((sum, expense) => {
    return sum + Number(expense.amount)
  }, 0)
)

/* ===========================
   Helpers
   =========================== */
function showToast(message, type = 'success') {
  toast.value = { show: true, message, type }

  setTimeout(() => {
    toast.value.show = false
  }, 2500)
}

function formatDate(dateString) {
  return new Date(dateString).toLocaleDateString()
}

/* ===========================
   API Calls
   =========================== */
async function loadExpenses() {
  const res = await fetch(API_URL)
  expenses.value = await res.json()
}

async function addExpense() {
  if (!description.value || !amount.value || !category.value || !date.value) {
    showToast('Please fill out all fields', 'error')
    return
  }

  const response = await fetch(API_URL, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
      description: description.value,
      amount: Number(amount.value),
      category: category.value,
      date: date.value
    })
  })

  if (!response.ok) {
    showToast('Failed to add expense', 'error')
    return
  }

  description.value = ''
  amount.value = null
  category.value = ''
  date.value = ''

  await loadExpenses()
  showToast('Expense added!')
}

async function deleteExpense(id) {
  await fetch(`${API_URL}/${id}`, {
    method: 'DELETE'
  })

  await loadExpenses()
  showToast('Expense deleted!', 'error')
}

function startEdit(id) {
  editingId.value = id
}

async function updateExpense(expense) {
  await fetch(`${API_URL}/${expense.id}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(expense)
  })

  editingId.value = null
  await loadExpenses()
  showToast('Expense updated!')
}

onMounted(loadExpenses)
</script>

<style>
body {
  font-family: Arial, sans-serif;
  background-color: #f4f6f8;
  margin: 0;
}

.container {
  max-width: 700px;
  margin: 50px auto;
  background: white;
  padding: 30px;
  border-radius: 10px;
  box-shadow: 0 8px 20px rgba(0, 0, 0, 0.08);
}

h1 {
  text-align: center;
  margin-bottom: 20px;
}

.form {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 10px;
  margin-bottom: 20px;
}

.form input {
  flex: 1 1 150px;
}

.form button {
  grid-column: 1 / -1; /* Span all columns */
  justify-self: center;
  width: 200px;
}

input {
  flex: 1;
  padding: 8px;
  border-radius: 6px;
  border: 1px solid #ccc;
}

button {
  padding: 6px 12px;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-weight: 500;
  font-size: 14px;
  min-width: 70px;
}

button:hover {
  opacity: 0.9;
}

button:not(.delete) {
  background-color: #4caf50;
  color: white;
}

.delete {
  background-color: #e74c3c;
  color: white;
}

.table-wrapper {
  overflow-x: auto;
}

.table-wrapper table {
  min-width: 700px;
}

table {
  width: 100%;
  border-collapse: collapse;
}

th, td {
  padding: 12px 8px;
  text-align: left;
  vertical-align: middle;
}

td input {
  width: 100%;
  padding: 6px 8px;
  font-size: 14px;
  box-sizing: border-box;
}

td :last-child {
  white-space: nowrap;
  width: 160px;
}

td button {
  margin-right: 5px;
  padding: 6px 10px;
  font-size: 13px;
}

thead {
  background-color: #f3f4f6;
}

tbody tr:hover {
  background-color: #f9fafb;
}

.actions {
  display: flex;
  gap: 6px;
}

.total {
  margin-top: 20px;
  font-size: 18px;
  font-weight: bold;
  text-align: right;
}

.toast {
  position: fixed;
  bottom: 20px;
  right: 20px;
  padding: 12px 18px;
  border-radius: 8px;
  color: white;
  font-weight: bold;
  box-shadow: 0 6px 15px rgba(0, 0, 0, 0.15);
  animation: fadeIn 0.3s ease;
}

.toast.success {
  background-color: #4caf50;
}

.toast.error {
  background-color: #e74c3c;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

@media (max-width: 768px) {
  .container {
    margin: 20px;
    padding: 20px;
  }
  
  .form {
    grid-template-columns: 1fr;
  }

  .form button {
    width: 100%;
  }

  /* Transform table into mobile cards */
  table,
  thead,
  tbody,
  th,
  td,
  tr {
    display: block;
  }

  thead {
    display: none;
  }

  tr {
    background: #f9fafb;
    margin-bottom: 15px;
    padding: 15px;
    border-radius: 8px;
  }

  td {
    padding: 8px 0;
    border: none;
  }

  td::before {
    font-weight: bold;
    display: block;
    margin-bottom: 3px;
    color: #555;
  }

  td:nth-of-type(1)::before { content: "Description"; }
  td:nth-of-type(2)::before { content: "Amount"; }
  td:nth-of-type(3)::before { content: "Category"; }
  td:nth-of-type(4)::before { content: "Date"; }
  td:nth-of-type(5)::before { content: "Actions"; }

  .actions {
    margin-top: 8px;
  }

}
</style>