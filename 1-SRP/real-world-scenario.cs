// 🏗️ REAL-WORLD SCENARIO — E-Commerce Order Processing
// 
// Scenario: You're building an e-commerce platform.
// An order goes through: validation → pricing → payment → notification → persistence
//
// Each step is handled by a SEPARATE class with a SINGLE responsibility.

using System;
using System.Collections.Generic;
using System.Linq;

namespace SRP.RealWorld
{
    // ══════════════════════════════════════════════════════
    // DOMAIN MODELS — Pure data, no behavior beyond identity
    // ══════════════════════════════════════════════════════

    public enum OrderStatus
    {
        Pending,
        Validated,
        Priced,
        Paid,
        Shipped,
        Delivered,
        Cancelled
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }

    public class OrderItem
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }

    public class Order
    {
        public int Id { get; set; }
        public string CustomerEmail { get; set; }
        public List<OrderItem> Items { get; set; } = new();
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    // ══════════════════════════════════════════════════════
    // RESPONSIBILITY 1: Order Validation
    // Actor: Business/Compliance team
    // Changes when: Business rules change (e.g., minimum order, age check)
    // ══════════════════════════════════════════════════════

    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new();
    }

    public class OrderValidator
    {
        private const decimal MinimumOrderAmount = 10.00m;
        private const int MaxItemsPerOrder = 50;

        public ValidationResult Validate(Order order)
        {
            var result = new ValidationResult { IsValid = true };

            if (order.Items == null || !order.Items.Any())
            {
                result.IsValid = false;
                result.Errors.Add("Order must contain at least one item.");
            }

            if (order.Items?.Count > MaxItemsPerOrder)
            {
                result.IsValid = false;
                result.Errors.Add($"Order cannot exceed {MaxItemsPerOrder} items.");
            }

            // Check stock availability
            foreach (var item in order.Items ?? new List<OrderItem>())
            {
                if (item.Quantity > item.Product.StockQuantity)
                {
                    result.IsValid = false;
                    result.Errors.Add(
                        $"'{item.Product.Name}' has only {item.Product.StockQuantity} in stock " +
                        $"(requested: {item.Quantity}).");
                }

                if (item.Quantity <= 0)
                {
                    result.IsValid = false;
                    result.Errors.Add($"'{item.Product.Name}' quantity must be positive.");
                }
            }

            return result;
        }
    }

    // ══════════════════════════════════════════════════════
    // RESPONSIBILITY 2: Price Calculation
    // Actor: Finance/Pricing team
    // Changes when: Pricing rules, taxes, or discounts change
    // ══════════════════════════════════════════════════════

    public class OrderPricingEngine
    {
        private const decimal TaxRate = 0.18m;          // 18% VAT
        private const decimal BulkDiscountThreshold = 500m;
        private const decimal BulkDiscountPercentage = 0.05m; // 5% off

        public decimal CalculateTotal(Order order)
        {
            var subtotal = order.Items.Sum(item => 
                item.Product.Price * item.Quantity);

            var discount = subtotal >= BulkDiscountThreshold
                ? subtotal * BulkDiscountPercentage
                : 0m;

            var taxableAmount = subtotal - discount;
            var tax = taxableAmount * TaxRate;

            var total = taxableAmount + tax;

            Console.WriteLine($"   Subtotal:  ${subtotal:F2}");
            Console.WriteLine($"   Discount:  -${discount:F2}");
            Console.WriteLine($"   Tax (18%): +${tax:F2}");
            Console.WriteLine($"   Total:     ${total:F2}");

            return total;
        }
    }

    // ══════════════════════════════════════════════════════
    // RESPONSIBILITY 3: Payment Processing
    // Actor: Payment/FinTech team
    // Changes when: Payment providers or methods change
    // ══════════════════════════════════════════════════════

    public class PaymentResult
    {
        public bool Success { get; set; }
        public string TransactionId { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class PaymentProcessor
    {
        public PaymentResult ProcessPayment(Order order)
        {
            // Simulate payment processing
            Console.WriteLine($"   💳 Processing payment of ${order.TotalAmount:F2}...");

            // In a real app: call Stripe, PayPal, etc.
            var transactionId = Guid.NewGuid().ToString("N")[..12].ToUpper();

            return new PaymentResult
            {
                Success = true,
                TransactionId = transactionId
            };
        }
    }

    // ══════════════════════════════════════════════════════
    // RESPONSIBILITY 4: Notifications
    // Actor: Marketing/Communications team
    // Changes when: Notification channels or templates change
    // ══════════════════════════════════════════════════════

    public class OrderNotifier
    {
        public void SendOrderConfirmation(Order order, string transactionId)
        {
            Console.WriteLine($"   📧 Confirmation email sent to {order.CustomerEmail}");
            Console.WriteLine($"      Order #{order.Id} | Transaction: {transactionId}");
        }

        public void SendOrderShipped(Order order, string trackingNumber)
        {
            Console.WriteLine($"   📧 Shipping notification sent to {order.CustomerEmail}");
            Console.WriteLine($"      Tracking: {trackingNumber}");
        }
    }

    // ══════════════════════════════════════════════════════
    // RESPONSIBILITY 5: Persistence
    // Actor: DBA/Infrastructure team  
    // Changes when: Database schema or technology changes
    // ══════════════════════════════════════════════════════

    public class OrderRepository
    {
        private readonly Dictionary<int, Order> _orders = new();

        public void Save(Order order)
        {
            _orders[order.Id] = order;
            Console.WriteLine($"   💾 Order #{order.Id} saved to database.");
        }

        public Order GetById(int id)
        {
            return _orders.TryGetValue(id, out var order) ? order : null;
        }
    }

    // ══════════════════════════════════════════════════════
    // ORCHESTRATOR — Composes all the single-responsibility classes
    // This is the ONLY class that knows about all the others.
    // It coordinates the flow but delegates the WORK.
    // ══════════════════════════════════════════════════════

    public class OrderService
    {
        private readonly OrderValidator _validator;
        private readonly OrderPricingEngine _pricingEngine;
        private readonly PaymentProcessor _paymentProcessor;
        private readonly OrderNotifier _notifier;
        private readonly OrderRepository _repository;

        public OrderService(
            OrderValidator validator,
            OrderPricingEngine pricingEngine,
            PaymentProcessor paymentProcessor,
            OrderNotifier notifier,
            OrderRepository repository)
        {
            _validator = validator;
            _pricingEngine = pricingEngine;
            _paymentProcessor = paymentProcessor;
            _notifier = notifier;
            _repository = repository;
        }

        public bool PlaceOrder(Order order)
        {
            Console.WriteLine($"\n🛒 Processing Order #{order.Id}...\n");

            // Step 1: Validate
            Console.WriteLine("Step 1: Validation");
            var validation = _validator.Validate(order);
            if (!validation.IsValid)
            {
                Console.WriteLine("   ❌ Validation failed:");
                foreach (var error in validation.Errors)
                    Console.WriteLine($"      - {error}");
                return false;
            }
            Console.WriteLine("   ✅ Validation passed.");
            order.Status = OrderStatus.Validated;

            // Step 2: Calculate pricing
            Console.WriteLine("\nStep 2: Pricing");
            order.TotalAmount = _pricingEngine.CalculateTotal(order);
            order.Status = OrderStatus.Priced;

            // Step 3: Process payment
            Console.WriteLine("\nStep 3: Payment");
            var payment = _paymentProcessor.ProcessPayment(order);
            if (!payment.Success)
            {
                Console.WriteLine($"   ❌ Payment failed: {payment.ErrorMessage}");
                return false;
            }
            Console.WriteLine($"   ✅ Payment successful! Transaction: {payment.TransactionId}");
            order.Status = OrderStatus.Paid;

            // Step 4: Save to database
            Console.WriteLine("\nStep 4: Persistence");
            _repository.Save(order);

            // Step 5: Send confirmation
            Console.WriteLine("\nStep 5: Notification");
            _notifier.SendOrderConfirmation(order, payment.TransactionId);

            Console.WriteLine($"\n🎉 Order #{order.Id} completed successfully!\n");
            return true;
        }
    }

    // ══════════════════════════════════════════════════════
    // DEMO
    // ══════════════════════════════════════════════════════

    class Program
    {
        static void Main(string[] args)
        {
            // Create products
            var laptop = new Product { Id = 1, Name = "MacBook Pro", Price = 2499.99m, StockQuantity = 10 };
            var mouse = new Product { Id = 2, Name = "Magic Mouse", Price = 99.99m, StockQuantity = 50 };
            var keyboard = new Product { Id = 3, Name = "Mechanical Keyboard", Price = 149.99m, StockQuantity = 0 };

            // Create order
            var order = new Order
            {
                Id = 5001,
                CustomerEmail = "customer@example.com",
                Items = new List<OrderItem>
                {
                    new OrderItem { Product = laptop, Quantity = 1 },
                    new OrderItem { Product = mouse, Quantity = 2 }
                }
            };

            // Compose services (in real apps, use Dependency Injection container)
            var orderService = new OrderService(
                validator: new OrderValidator(),
                pricingEngine: new OrderPricingEngine(),
                paymentProcessor: new PaymentProcessor(),
                notifier: new OrderNotifier(),
                repository: new OrderRepository()
            );

            // Process valid order
            orderService.PlaceOrder(order);

            // ── Try an invalid order ──
            Console.WriteLine("═══════════════════════════════════════");
            var badOrder = new Order
            {
                Id = 5002,
                CustomerEmail = "customer@example.com",
                Items = new List<OrderItem>
                {
                    new OrderItem { Product = keyboard, Quantity = 3 } // Out of stock!
                }
            };
            orderService.PlaceOrder(badOrder);
        }
    }
}
