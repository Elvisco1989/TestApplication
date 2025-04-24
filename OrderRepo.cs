namespace TestApplication
{
    public class OrderRepo
    {
        private readonly List<Order> _orders = new()
        {
            
        };
        private int _orderIdCounter = 1;

        public List<Order> GetOrders()
        {
            return _orders;
        }
        public void AddProductToOrder(int orderId, Product product, int quantity)
        {
            var order = _orders.FirstOrDefault(o => o.OrderId == orderId);
            if (order != null)
            {
                var orderItem = new OrderItem
                {
                    OrderId = orderId,
                    ProductId = product.ProductId,
                    Product = product,
                    Quantity = quantity
                };
                order.OrderItems.Add(orderItem);
            }
        }
        public Order GetOrder(int orderId)
        {
            return _orders.FirstOrDefault(o => o.OrderId == orderId);
        }
        public Order AddOrder(Order order)
        {
            // Assign a new OrderId to the order
            order.OrderId = _orderIdCounter++;
            _orders.Add(order);
            return order;
        }
        public void UpdateOrder(Order order)
        {
            var existingOrder = GetOrder(order.OrderId);
            if (existingOrder != null)
            {
                existingOrder.customerId = order.customerId;
                existingOrder.OrderItems = order.OrderItems;
            }
        }
    }
}
