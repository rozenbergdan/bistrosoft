import axios from 'axios'
import type {
  Product,
  Customer,
  Order,
  CreateCustomerRequest,
  CreateOrderRequest,
  UpdateOrderStatusRequest
} from '../types'

const api = axios.create({
  baseURL: 'http://localhost:5000/api',
  headers: {
    'Content-Type': 'application/json'
  }
})

export const customersApi = {
  create: (data: CreateCustomerRequest) => 
    api.post<Customer>('/customers', data),
  
  getById: (id: string) => 
    api.get<Customer>(`/customers/${id}`),
  
  getOrders: (id: string) => 
    api.get<Order[]>(`/customers/${id}/orders`)
}

export const productsApi = {
  getAll: () => 
    api.get<Product[]>('/products')
}

export const ordersApi = {
  create: (data: CreateOrderRequest) => 
    api.post<Order>('/orders', data),
  
  updateStatus: (id: string, data: UpdateOrderStatusRequest) => 
    api.put<Order>(`/orders/${id}/status`, data)
}

export default api
