export enum OrderStatus {
  Pending = 0,
  Paid = 1,
  Shipped = 2,
  Delivered = 3,
  Cancelled = 4
}

export interface Product {
  id: string
  name: string
  price: number
  stockQuantity: number
}

export interface Customer {
  id: string
  name: string
  email: string
  phoneNumber: string
}

export interface OrderItem {
  id: string
  productId: string
  productName: string
  quantity: number
  unitPrice: number
  subtotal: number
}

export interface Order {
  id: string
  customerId: string
  customerName: string
  totalAmount: number
  createdAt: string
  status: OrderStatus
  orderItems: OrderItem[]
}

export interface CreateCustomerRequest {
  name: string
  email: string
  phoneNumber: string
}

export interface CreateOrderRequest {
  customerId: string
  items: OrderItemRequest[]
}

export interface OrderItemRequest {
  productId: string
  quantity: number
}

export interface UpdateOrderStatusRequest {
  status: OrderStatus
}
