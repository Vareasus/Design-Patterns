// 🏗️ REAL-WORLD SCENARIO — E-Commerce Clean Architecture with DIP
//
// Scenario: A full e-commerce backend with proper layered architecture.
// The domain/business layer defines interfaces.
// The infrastructure layer implements them.
// The composition root wires everything together.
//
// You can swap ANY infrastructure component without touching business logic.

using System;
using System.Collections.Generic;
using System.Linq;

namespace DIP.RealWorld
{
    // ══════════════════════════════════════════════════════
    // LAYER 1: DOMAIN — The heart of the application
    // This layer has ZERO external dependencies.
    // It defines WHAT the system needs, not HOW.
    // ══════════════════════════════════════════════════════

    // Domain models
    public class Product
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }

    public class Order
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public List<OrderLine> Lines { get; set; } = new();
        public decimal Total { get; set; }
        public string Status { get; set; } = "Created";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class OrderLine
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal => Quantity * UnitPrice;
    }

    // Domain interfaces — DEFINED here, IMPLEMENTED elsewhere
    public interface IProductRepository
    {
        Product GetById(string id);
        void UpdateStock(string id, int newStock);
    }

    public interface IOrderRepository
    {
        void Save(Order order);
        Order GetById(string id);
        List<Order> GetByCustomer(string customerId);
    }

    public interface IPaymentService
    {
        (bool Success, string TransactionId) ProcessPayment(string customerId, decimal amount);
    }

    public interface INotificationService
    {
        void SendOrderConfirmation(string email, Order order);
        void SendShippingUpdate(string email, string orderId, string trackingNumber);
    }

    public interface IInventoryService
    {
        bool CheckAvailability(string productId, int quantity);
        void ReserveStock(string productId, int quantity);
        void ReleaseStock(string productId, int quantity);
    }

    public interface ILogger
    {
        void Info(string message);
        void Error(string message, Exception ex = null);
    }

    // Domain service — ONLY depends on interfaces (abstractions)
    public class OrderProcessingService
    {
        private readonly IProductRepository _productRepo;
        private readonly IOrderRepository _orderRepo;
        private readonly IPaymentService _paymentService;
        private readonly INotificationService _notificationService;
        private readonly IInventoryService _inventoryService;
        private readonly ILogger _logger;

        public OrderProcessingService(
            IProductRepository productRepo,
            IOrderRepository orderRepo,
            IPaymentService paymentService,
            INotificationService notificationService,
            IInventoryService inventoryService,
            ILogger logger)
        {
            _productRepo = productRepo;
            _orderRepo = orderRepo;
            _paymentService = paymentService;
            _notificationService = notificationService;
            _inventoryService = inventoryService;
            _logger = logger;
        }

        public Order CreateOrder(string customerId, List<(string ProductId, int Quantity)> items)
        {
            _logger.Info($"Processing order for customer {customerId}");

            // Step 1: Validate and build order
            var order = new Order
            {
                Id = $"ORD-{Guid.NewGuid().ToString("N")[..6].ToUpper()}",
                CustomerId = customerId
            };

            foreach (var (productId, quantity) in items)
            {
                // Check inventory
                if (!_inventoryService.CheckAvailability(productId, quantity))
                {
                    _logger.Error($"Product {productId} not available (qty: {quantity})");
                    throw new InvalidOperationException($"Product {productId} is out of stock.");
                }

                var product = _productRepo.GetById(productId);
                order.Lines.Add(new OrderLine
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Quantity = quantity,
                    UnitPrice = product.Price
                });
            }

            order.Total = order.Lines.Sum(l => l.LineTotal);
            _logger.Info($"Order {order.Id}: {order.Lines.Count} items, total ${order.Total:F2}");

            // Step 2: Reserve inventory
            foreach (var line in order.Lines)
            {
                _inventoryService.ReserveStock(line.ProductId, line.Quantity);
            }

            // Step 3: Process payment
            var (success, transactionId) = _paymentService.ProcessPayment(customerId, order.Total);
            if (!success)
            {
                // Rollback inventory
                foreach (var line in order.Lines)
                    _inventoryService.ReleaseStock(line.ProductId, line.Quantity);

                _logger.Error($"Payment failed for order {order.Id}");
                throw new InvalidOperationException("Payment failed.");
            }

            // Step 4: Save order
            order.Status = "Confirmed";
            _orderRepo.Save(order);

            // Step 5: Send notification
            _notificationService.SendOrderConfirmation($"{customerId}@example.com", order);

            _logger.Info($"Order {order.Id} completed successfully! Transaction: {transactionId}");
            return order;
        }
    }

    // ══════════════════════════════════════════════════════
    // LAYER 2: INFRASTRUCTURE — Implements domain interfaces
    // This layer knows about databases, APIs, file systems, etc.
    // ══════════════════════════════════════════════════════

    // ── SQL Implementation ──
    public class SqlProductRepository : IProductRepository
    {
        private readonly Dictionary<string, Product> _products = new();

        public SqlProductRepository()
        {
            // Seed data
            _products["P1"] = new Product { Id = "P1", Name = "Laptop", Price = 999.99m, Stock = 50 };
            _products["P2"] = new Product { Id = "P2", Name = "Mouse", Price = 29.99m, Stock = 200 };
            _products["P3"] = new Product { Id = "P3", Name = "Keyboard", Price = 79.99m, Stock = 150 };
        }

        public Product GetById(string id)
        {
            Console.WriteLine($"      💾 [SQL] SELECT * FROM Products WHERE Id = '{id}'");
            return _products.GetValueOrDefault(id);
        }

        public void UpdateStock(string id, int newStock)
        {
            if (_products.ContainsKey(id))
                _products[id].Stock = newStock;
            Console.WriteLine($"      💾 [SQL] UPDATE Products SET Stock = {newStock} WHERE Id = '{id}'");
        }
    }

    public class SqlOrderRepository : IOrderRepository
    {
        private readonly Dictionary<string, Order> _orders = new();

        public void Save(Order order)
        {
            _orders[order.Id] = order;
            Console.WriteLine($"      💾 [SQL] INSERT INTO Orders ({order.Id}, {order.CustomerId}, ${order.Total:F2})");
        }

        public Order GetById(string id) => _orders.GetValueOrDefault(id);

        public List<Order> GetByCustomer(string customerId)
            => _orders.Values.Where(o => o.CustomerId == customerId).ToList();
    }

    // ── Stripe Payment ──
    public class StripePaymentService : IPaymentService
    {
        public (bool Success, string TransactionId) ProcessPayment(string customerId, decimal amount)
        {
            var txId = $"pi_{Guid.NewGuid().ToString("N")[..12]}";
            Console.WriteLine($"      💳 [Stripe] Charging ${amount:F2} → Transaction: {txId}");
            return (true, txId);
        }
    }

    // ── Email Notification ──
    public class EmailNotificationService : INotificationService
    {
        public void SendOrderConfirmation(string email, Order order)
        {
            Console.WriteLine($"      📧 [Email] Order confirmation sent to {email}");
            Console.WriteLine($"         Order {order.Id}: {order.Lines.Count} items, ${order.Total:F2}");
        }

        public void SendShippingUpdate(string email, string orderId, string trackingNumber)
        {
            Console.WriteLine($"      📧 [Email] Shipping update to {email}: {trackingNumber}");
        }
    }

    // ── Warehouse Inventory ──
    public class WarehouseInventoryService : IInventoryService
    {
        public bool CheckAvailability(string productId, int quantity)
        {
            Console.WriteLine($"      📦 [Warehouse] Checking stock for {productId} (need {quantity})");
            return true; // Simplified
        }

        public void ReserveStock(string productId, int quantity)
        {
            Console.WriteLine($"      📦 [Warehouse] Reserved {quantity}x {productId}");
        }

        public void ReleaseStock(string productId, int quantity)
        {
            Console.WriteLine($"      📦 [Warehouse] Released {quantity}x {productId}");
        }
    }

    // ── Console Logger ──
    public class ConsoleAppLogger : ILogger
    {
        public void Info(string message)
            => Console.WriteLine($"      ℹ️ {message}");
        public void Error(string message, Exception ex = null)
            => Console.WriteLine($"      ❌ {message} {ex?.Message}");
    }

    // ══════════════════════════════════════════════════════
    // LAYER 3: COMPOSITION ROOT — Wires everything together
    // This is the ONLY place that knows about concrete types.
    // In a real app, this is your DI container configuration.
    // ══════════════════════════════════════════════════════

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("╔═══════════════════════════════════════════════════════╗");
            Console.WriteLine("║  Clean Architecture with DIP — Real World Example    ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════╝");

            // ── COMPOSITION ROOT ──
            // This is the ONLY place where concrete types are mentioned.
            // In ASP.NET Core, this would be in Program.cs / Startup.cs:
            //   builder.Services.AddScoped<IProductRepository, SqlProductRepository>();
            //   builder.Services.AddScoped<IPaymentService, StripePaymentService>();
            //   etc.

            var productRepo = new SqlProductRepository();
            var orderRepo = new SqlOrderRepository();
            var payment = new StripePaymentService();
            var notifications = new EmailNotificationService();
            var inventory = new WarehouseInventoryService();
            var logger = new ConsoleAppLogger();

            // OrderProcessingService knows NOTHING about SQL, Stripe, SMTP, etc.
            var orderService = new OrderProcessingService(
                productRepo, orderRepo, payment, notifications, inventory, logger);

            // ── Place an order ──
            Console.WriteLine("\n═══ ORDER 1: Single Item ═══\n");
            var order1 = orderService.CreateOrder("alice", new List<(string, int)>
            {
                ("P1", 1)  // 1 Laptop
            });

            Console.WriteLine($"\n  🎉 Order {order1.Id}: ${order1.Total:F2}");

            Console.WriteLine("\n═══ ORDER 2: Multiple Items ═══\n");
            var order2 = orderService.CreateOrder("bob", new List<(string, int)>
            {
                ("P2", 2),  // 2 Mice
                ("P3", 1)   // 1 Keyboard
            });

            Console.WriteLine($"\n  🎉 Order {order2.Id}: ${order2.Total:F2}");

            // ── Summary ──
            Console.WriteLine("\n" + new string('═', 57));
            Console.WriteLine("✨ OrderProcessingService has ZERO infrastructure knowledge.");
            Console.WriteLine("✨ Want to switch to MongoDB? Replace SqlProductRepository.");
            Console.WriteLine("✨ Want to use PayPal? Replace StripePaymentService.");
            Console.WriteLine("✨ Want SMS notifications? Replace EmailNotificationService.");
            Console.WriteLine("✨ Want to unit test? Inject mock implementations.");
            Console.WriteLine("✨ Change the composition root. Business logic stays untouched.");
            Console.WriteLine("✨ THAT is the Dependency Inversion Principle in production.");
        }
    }
}
