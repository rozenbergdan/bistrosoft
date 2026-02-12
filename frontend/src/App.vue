<template>
  <div id="app">
    <header>
      <h1>🛒 Tienda Online - Sistema de Pedidos</h1>
    </header>
    
    <div class="container">
      <div class="row">
        <div class="col-md-6">
          <CustomerForm @customer-created="handleCustomerCreated" />
          
          <div v-if="currentCustomer" class="current-customer">
            <h3>Cliente Actual</h3>
            <p><strong>{{ currentCustomer.name }}</strong></p>
            <p>{{ currentCustomer.email }}</p>
            <button @click="clearCustomer" class="btn btn-secondary btn-sm">
              Cambiar Cliente
            </button>
          </div>
        </div>
        
        <div class="col-md-6">
          <ShoppingCart 
            :cart-items="cartItems"
            :customer-id="currentCustomerId"
            @remove-item="removeFromCart"
            @checkout="handleCheckout"
          />
        </div>
      </div>
      
      <ProductList @add-to-cart="addToCart" />
      
      <div v-if="orderSuccess" class="order-success">
        <h2>✓ Pedido creado exitosamente</h2>
        <p>Número de pedido: {{ orderSuccess.id }}</p>
        <p>Total: ${{ orderSuccess.totalAmount.toFixed(2) }}</p>
        <p>Estado: {{ getStatusLabel(orderSuccess.status) }}</p>
        <button @click="orderSuccess = null" class="btn btn-primary">
          Nuevo Pedido
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import ProductList from './components/ProductList.vue'
import ShoppingCart from './components/ShoppingCart.vue'
import CustomerForm from './components/CustomerForm.vue'
import { ordersApi } from './services/api'
import type { Product, Order, OrderStatus } from './types'

interface CartItem {
  productId: string
  productName: string
  quantity: number
  unitPrice: number
}

interface CustomerInfo {
  id: string
  name: string
  email: string
}

const currentCustomer = ref<CustomerInfo | null>(null)
const cartItems = ref<CartItem[]>([])
const orderSuccess = ref<Order | null>(null)

const currentCustomerId = computed(() => currentCustomer.value?.id || null)

const handleCustomerCreated = (customerId: string, customerName: string) => {
  currentCustomer.value = {
    id: customerId,
    name: customerName,
    email: ''
  }
}

const clearCustomer = () => {
  currentCustomer.value = null
  cartItems.value = []
}

const addToCart = (product: Product, quantity: number) => {
  const existingItem = cartItems.value.find(item => item.productId === product.id)
  
  if (existingItem) {
    existingItem.quantity += quantity
  } else {
    cartItems.value.push({
      productId: product.id,
      productName: product.name,
      quantity: quantity,
      unitPrice: product.price
    })
  }
}

const removeFromCart = (productId: string) => {
  cartItems.value = cartItems.value.filter(item => item.productId !== productId)
}

const handleCheckout = async () => {
  if (!currentCustomerId.value || cartItems.value.length === 0) {
    return
  }
  
  try {
    const orderRequest = {
      customerId: currentCustomerId.value,
      items: cartItems.value.map(item => ({
        productId: item.productId,
        quantity: item.quantity
      }))
    }
    
    const response = await ordersApi.create(orderRequest)
    orderSuccess.value = response.data
    
    // Limpiar carrito
    cartItems.value = []
  } catch (err: any) {
    alert('Error al crear el pedido: ' + (err.response?.data?.message || err.message))
  }
}

const getStatusLabel = (status: OrderStatus): string => {
  const labels: Record<OrderStatus, string> = {
    [0]: 'Pendiente',
    [1]: 'Pagado',
    [2]: 'Enviado',
    [3]: 'Entregado',
    [4]: 'Cancelado'
  }
  return labels[status] || 'Desconocido'
}
</script>

<style>
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

body {
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;
  background-color: #f5f6fa;
  color: #2c3e50;
}

#app {
  min-height: 100vh;
}

header {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 30px 20px;
  text-align: center;
  box-shadow: 0 2px 10px rgba(0,0,0,0.1);
}

header h1 {
  margin: 0;
  font-size: 32px;
  font-weight: 600;
}

.container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 20px;
}

.row {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 20px;
  margin-bottom: 30px;
}

.current-customer {
  background: white;
  border: 1px solid #ddd;
  border-radius: 8px;
  padding: 20px;
  margin: 20px;
}

.current-customer h3 {
  color: #2c3e50;
  margin-bottom: 10px;
}

.current-customer p {
  margin: 5px 0;
  color: #7f8c8d;
}

.order-success {
  background: linear-gradient(135deg, #11998e 0%, #38ef7d 100%);
  color: white;
  border-radius: 8px;
  padding: 30px;
  text-align: center;
  margin: 20px;
  box-shadow: 0 4px 15px rgba(0,0,0,0.2);
}

.order-success h2 {
  margin-bottom: 20px;
  font-size: 28px;
}

.order-success p {
  font-size: 18px;
  margin: 10px 0;
}

.btn {
  padding: 10px 20px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 16px;
  transition: all 0.2s;
  font-weight: 500;
}

.btn-primary {
  background-color: #3498db;
  color: white;
}

.btn-secondary {
  background-color: #95a5a6;
  color: white;
}

.btn-sm {
  padding: 6px 12px;
  font-size: 14px;
}

.btn:hover {
  opacity: 0.9;
  transform: translateY(-1px);
}

.btn:active {
  transform: translateY(0);
}
</style>
