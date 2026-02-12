<template>
  <div class="product-list">
    <h2>Productos Disponibles</h2>
    
    <div v-if="loading" class="loading">Cargando productos...</div>
    
    <div v-else-if="error" class="error">{{ error }}</div>
    
    <div v-else class="products-grid">
      <div 
        v-for="product in products" 
        :key="product.id" 
        class="product-card"
      >
        <h3>{{ product.name }}</h3>
        <p class="price">${{ product.price.toFixed(2) }}</p>
        <p class="stock">Stock: {{ product.stockQuantity }}</p>
        
        <div class="product-actions">
          <input 
            v-model.number="quantities[product.id]" 
            type="number" 
            min="1" 
            :max="product.stockQuantity"
            placeholder="Cantidad"
            class="quantity-input"
          />
          <button 
            @click="addToCart(product)" 
            :disabled="!quantities[product.id] || quantities[product.id] > product.stockQuantity"
            class="btn btn-primary"
          >
            Agregar al Carrito
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { productsApi } from '../services/api'
import type { Product } from '../types'

const emit = defineEmits<{
  addToCart: [product: Product, quantity: number]
}>()

const products = ref<Product[]>([])
const quantities = ref<Record<string, number>>({})
const loading = ref(false)
const error = ref('')

const fetchProducts = async () => {
  loading.value = true
  error.value = ''
  
  try {
    const response = await productsApi.getAll()
    products.value = response.data
  } catch (err: any) {
    error.value = 'Error al cargar productos: ' + (err.response?.data?.message || err.message)
  } finally {
    loading.value = false
  }
}

const addToCart = (product: Product) => {
  const quantity = quantities.value[product.id]
  if (quantity && quantity > 0 && quantity <= product.stockQuantity) {
    emit('addToCart', product, quantity)
    quantities.value[product.id] = 1
  }
}

onMounted(() => {
  fetchProducts()
})
</script>

<style scoped>
.product-list {
  padding: 20px;
}

h2 {
  color: #2c3e50;
  margin-bottom: 20px;
}

.loading, .error {
  padding: 20px;
  text-align: center;
  font-size: 18px;
}

.error {
  color: #e74c3c;
  background-color: #fadbd8;
  border-radius: 4px;
}

.products-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
  gap: 20px;
  margin-top: 20px;
}

.product-card {
  background: white;
  border: 1px solid #ddd;
  border-radius: 8px;
  padding: 20px;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
  transition: transform 0.2s;
}

.product-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 8px rgba(0,0,0,0.15);
}

.product-card h3 {
  color: #2c3e50;
  margin: 0 0 10px 0;
  font-size: 18px;
}

.price {
  color: #27ae60;
  font-size: 24px;
  font-weight: bold;
  margin: 10px 0;
}

.stock {
  color: #7f8c8d;
  font-size: 14px;
  margin-bottom: 15px;
}

.product-actions {
  display: flex;
  gap: 10px;
  align-items: center;
}

.quantity-input {
  width: 80px;
  padding: 8px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 14px;
}

.btn {
  padding: 8px 16px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
  transition: background-color 0.2s;
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
