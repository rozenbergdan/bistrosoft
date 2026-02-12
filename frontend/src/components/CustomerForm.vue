<template>
  <div class="customer-form">
    <h2>Información del Cliente</h2>
    
    <form @submit.prevent="handleSubmit">
      <div class="form-group">
        <label for="name">Nombre:</label>
        <input 
          v-model="form.name" 
          type="text" 
          id="name" 
          required 
          class="form-control"
          placeholder="Ingrese su nombre"
        />
      </div>
      
      <div class="form-group">
        <label for="email">Email:</label>
        <input 
          v-model="form.email" 
          type="email" 
          id="email" 
          required 
          class="form-control"
          placeholder="correo@ejemplo.com"
        />
      </div>
      
      <div class="form-group">
        <label for="phone">Teléfono:</label>
        <input 
          v-model="form.phoneNumber" 
          type="tel" 
          id="phone" 
          class="form-control"
          placeholder="+54 11 1234-5678"
        />
      </div>
      
      <div v-if="error" class="error-message">
        {{ error }}
      </div>
      
      <div v-if="success" class="success-message">
        Cliente creado exitosamente: {{ success }}
      </div>
      
      <button 
        type="submit" 
        class="btn btn-primary btn-block"
        :disabled="loading"
      >
        {{ loading ? 'Creando...' : 'Crear Cliente' }}
      </button>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import { customersApi } from '../services/api'
import type { CreateCustomerRequest } from '../types'

const emit = defineEmits<{
  customerCreated: [customerId: string, customerName: string]
}>()

const form = reactive<CreateCustomerRequest>({
  name: '',
  email: '',
  phoneNumber: ''
})

const loading = ref(false)
const error = ref('')
const success = ref('')

const handleSubmit = async () => {
  loading.value = true
  error.value = ''
  success.value = ''
  
  try {
    const response = await customersApi.create(form)
    success.value = response.data.name
    
    emit('customerCreated', response.data.id, response.data.name)
    
    // Reset form
    form.name = ''
    form.email = ''
    form.phoneNumber = ''
    
    setTimeout(() => {
      success.value = ''
    }, 3000)
  } catch (err: any) {
    error.value = err.response?.data?.message || 'Error al crear cliente'
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.customer-form {
  background: white;
  border: 1px solid #ddd;
  border-radius: 8px;
  padding: 20px;
  margin: 20px;
}

h2 {
  color: #2c3e50;
  margin-bottom: 20px;
}

.form-group {
  margin-bottom: 15px;
}

label {
  display: block;
  margin-bottom: 5px;
  color: #2c3e50;
  font-weight: 500;
}

.form-control {
  width: 100%;
  padding: 10px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 14px;
  box-sizing: border-box;
}

.form-control:focus {
  outline: none;
  border-color: #3498db;
}

.error-message {
  background-color: #fadbd8;
  color: #e74c3c;
  padding: 10px;
  border-radius: 4px;
  margin-bottom: 15px;
}

.success-message {
  background-color: #d4edda;
  color: #155724;
  padding: 10px;
  border-radius: 4px;
  margin-bottom: 15px;
}

.btn {
  padding: 10px 20px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 16px;
  transition: background-color 0.2s;
}

.btn-block {
  width: 100%;
}

.btn-primary {
  background-color: #3498db;
  color: white;
}

.btn-primary:hover:not(:disabled) {
  background-color: #2980b9;
}

.btn:disabled {
  background-color: #bdc3c7;
  cursor: not-allowed;
}
</style>
