<template>
  <div class="shopping-cart">
    <h2>Carrito de Compras</h2>
    
    <div v-if="cartItems.length === 0" class="empty-cart">
      El carrito está vacío
    </div>
    
    <div v-else>
      <div class="cart-items">
        <div v-for="item in cartItems" :key="item.productId" class="cart-item">
          <div class="item-info">
            <h4>{{ item.productName }}</h4>
            <p class="item-price">${{ item.unitPrice.toFixed(2) }} x {{ item.quantity }}</p>
          </div>
          <div class="item-total">
            <strong>${{ (item.unitPrice * item.quantity).toFixed(2) }}</strong>
            <button @click="removeItem(item.productId)" class="btn btn-danger btn-sm">
              Eliminar
            </button>
          </div>
        </div>
      </div>
      
      <div class="cart-summary">
        <div class="total">
          <strong>Total:</strong>
          <span class="total-amount">${{ totalAmount.toFixed(2) }}</span>
        </div>
        
        <button 
          @click="$emit('checkout')" 
          class="btn btn-success btn-block"
          :disabled="!customerId"
        >
          Finalizar Compra
        </button>
        
        <p v-if="!customerId" class="warning">
          Primero debes seleccionar o crear un cliente
        </p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'

interface CartItem {
  productId: string
  productName: string
  quantity: number
  unitPrice: number
}

const props = defineProps<{
  cartItems: CartItem[]
  customerId: string | null
}>()

const emit = defineEmits<{
  removeItem: [productId: string]
  checkout: []
}>()

const totalAmount = computed(() => {
  return props.cartItems.reduce((sum, item) => {
    return sum + (item.quantity * item.unitPrice)
  }, 0)
})

const removeItem = (productId: string) => {
  emit('removeItem', productId)
}
</script>

<style scoped>
.shopping-cart {
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

.empty-cart {
  text-align: center;
  padding: 40px;
  color: #7f8c8d;
  font-size: 16px;
}

.cart-items {
  margin-bottom: 20px;
}

.cart-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 15px;
  border-bottom: 1px solid #ecf0f1;
}

.cart-item:last-child {
  border-bottom: none;
}

.item-info h4 {
  margin: 0 0 5px 0;
  color: #2c3e50;
}

.item-price {
  color: #7f8c8d;
  margin: 0;
  font-size: 14px;
}

.item-total {
  display: flex;
  align-items: center;
  gap: 15px;
}

.cart-summary {
  border-top: 2px solid #ecf0f1;
  padding-top: 20px;
  margin-top: 20px;
}

.total {
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-size: 20px;
  margin-bottom: 20px;
}

.total-amount {
  color: #27ae60;
  font-size: 28px;
  font-weight: bold;
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

.btn-success {
  background-color: #27ae60;
  color: white;
}

.btn-success:hover:not(:disabled) {
  background-color: #229954;
}

.btn-danger {
  background-color: #e74c3c;
  color: white;
}

.btn-danger:hover {
  background-color: #c0392b;
}

.btn-sm {
  padding: 5px 10px;
  font-size: 14px;
}

.btn:disabled {
  background-color: #bdc3c7;
  cursor: not-allowed;
}

.warning {
  text-align: center;
  color: #e67e22;
  margin-top: 10px;
  font-size: 14px;
}
</style>
