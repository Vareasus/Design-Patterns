// ✅ GOOD EXAMPLE — Following Dependency Inversion Principle
// High-level modules depend on ABSTRACTIONS (interfaces).
// Low-level modules IMPLEMENT those abstractions.
// Everything is decoupled, testable, and swappable.

using System;
using System.Collections.Generic;

namespace DIP.Good
{
    // ══════════════════════════════════════════════════════
    // ABSTRACTIONS — Defined by the HIGH-LEVEL layer
    // These interfaces represent WHAT the business logic needs,
    // not HOW it's implemented.
    // ══════════════════════════════════════════════════════

    public interface ILogger
    {
        void Info(string message);
        void Error(string message);
        void Warning(string message);
    }

    public interface IEmailService
    {
        void Send(string to, string subject, string body);
    }

    public interface IPaymentGateway
    {
        bool Charge(string customerId, decimal amount);
        bool Refund(string transactionId, decimal amount);
    }

    public interface IOrderRepository
    {
        void Save(string orderId, object orderData);
        object GetById(string orderId);
    }

    // ══════════════════════════════════════════════════════
    // LOW-LEVEL IMPLEMENTATIONS — Can be swapped freely
    // ══════════════════════════════════════════════════════

    // ── Loggers ──
    public class ConsoleLogger : ILogger
    {
        public void Info(string message)
            => Console.WriteLine($"  ℹ️ [Console] {message}");
        public void Error(string message)
            => Console.WriteLine($"  ❌ [Console] ERROR: {message}");
        public void Warning(string message)
            => Console.WriteLine($"  ⚠️ [Console] WARN: {message}");
    }

    public class FileLogger : ILogger
    {
        public void Info(string message)
            => Console.WriteLine($"  📝 [File] {message}");
        public void Error(string message)
            => Console.WriteLine($"  📝 [File] ERROR: {message}");
        public void Warning(string message)
            => Console.WriteLine($"  📝 [File] WARN: {message}");
    }

    // ── Email Services ──
    public class SmtpEmailService : IEmailService
    {
        public void Send(string to, string subject, string body)
        {
            Console.WriteLine($"  📧 [SMTP] To: {to}");
            Console.WriteLine($"     Subject: {subject}");
        }
    }

    public class SendGridEmailService : IEmailService
    {
        public void Send(string to, string subject, string body)
        {
            Console.WriteLine($"  📧 [SendGrid API] To: {to}");
            Console.WriteLine($"     Subject: {subject}");
        }
    }

    // ── Payment Gateways ──
    public class StripeGateway : IPaymentGateway
    {
        public bool Charge(string customerId, decimal amount)
        {
            Console.WriteLine($"  💳 [Stripe] Charging ${amount} to {customerId}");
            return true;
        }
        public bool Refund(string transactionId, decimal amount)
        {
            Console.WriteLine($"  💳 [Stripe] Refunding ${amount} for {transactionId}");
            return true;
        }
    }

    public class PayPalGateway : IPaymentGateway
    {
        public bool Charge(string customerId, decimal amount)
        {
            Console.WriteLine($"  🅿️ [PayPal] Charging ${amount} to {customerId}");
            return true;
        }
        public bool Refund(string transactionId, decimal amount)
        {
            Console.WriteLine($"  🅿️ [PayPal] Refunding ${amount} for {transactionId}");
            return true;
        }
    }

    // ── Repositories ──
    public class SqlServerRepository : IOrderRepository
    {
        private readonly Dictionary<string, object> _store = new();

        public void Save(string orderId, object orderData)
        {
            _store[orderId] = orderData;
            Console.WriteLine($"  💾 [SQL Server] Saved order {orderId}");
        }

        public object GetById(string orderId)
        {
            Console.WriteLine($"  💾 [SQL Server] Loading order {orderId}");
            return _store.GetValueOrDefault(orderId);
        }
    }

    public class MongoRepository : IOrderRepository
    {
        private readonly Dictionary<string, object> _store = new();

        public void Save(string orderId, object orderData)
        {
            _store[orderId] = orderData;
            Console.WriteLine($"  🍃 [MongoDB] Saved order {orderId}");
        }

        public object GetById(string orderId)
        {
            Console.WriteLine($"  🍃 [MongoDB] Loading order {orderId}");
            return _store.GetValueOrDefault(orderId);
        }
    }

    // For unit tests — no real infrastructure needed!
    public class InMemoryRepository : IOrderRepository
    {
        private readonly Dictionary<string, object> _store = new();

        public void Save(string orderId, object orderData)
        {
            _store[orderId] = orderData;
            Console.WriteLine($"  🧪 [InMemory] Saved order {orderId}");
        }

        public object GetById(string orderId)
        {
            Console.WriteLine($"  🧪 [InMemory] Loading order {orderId}");
            return _store.GetValueOrDefault(orderId);
        }
    }

    // ══════════════════════════════════════════════════════
    // HIGH-LEVEL MODULE — Depends ONLY on abstractions!
    // Doesn't know or care about SQL, Stripe, SMTP, files...
    // ══════════════════════════════════════════════════════

    public class OrderService
    {
        // All dependencies are INTERFACES — not concrete classes
        private readonly IOrderRepository _repository;
        private readonly IPaymentGateway _paymentGateway;
        private readonly IEmailService _emailService;
        private readonly ILogger _logger;

        // Dependencies are INJECTED through the constructor
        public OrderService(
            IOrderRepository repository,
            IPaymentGateway paymentGateway,
            IEmailService emailService,
            ILogger logger)
        {
            _repository = repository;
            _paymentGateway = paymentGateway;
            _emailService = emailService;
            _logger = logger;
        }

        public void PlaceOrder(string customerId, string productName, decimal price)
        {
            Console.WriteLine($"\n  🛒 Placing order for {customerId}...\n");

            _logger.Info($"Order started for {customerId}");

            // Charge — doesn't know if it's Stripe, PayPal, or a mock
            var charged = _paymentGateway.Charge(customerId, price);
            if (!charged)
            {
                _logger.Error("Payment failed!");
                return;
            }

            // Save — doesn't know if it's SQL Server, MongoDB, or in-memory
            var orderId = Guid.NewGuid().ToString("N")[..8];
            _repository.Save(orderId, new { customerId, productName, price });

            // Notify — doesn't know if it's SMTP, SendGrid, or a mock
            _emailService.Send(
                $"{customerId}@example.com",
                "Order Confirmed!",
                $"Your order for {productName} (${price}) has been placed.");

            _logger.Info($"Order {orderId} completed successfully!");
            Console.WriteLine($"\n  ✅ Order {orderId} placed.\n");
        }
    }

    // ══════════════════════════════════════════════════════
    // DEMO — Same business logic, different infrastructure
    // ══════════════════════════════════════════════════════

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("╔═══════════════════════════════════════════════════╗");
            Console.WriteLine("║  DIP — Same Logic, Swappable Infrastructure      ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════╝");

            // ── Configuration 1: Production (SQL + Stripe + SMTP) ──
            Console.WriteLine("\n══ PRODUCTION SETUP ══");
            var prodService = new OrderService(
                repository: new SqlServerRepository(),
                paymentGateway: new StripeGateway(),
                emailService: new SmtpEmailService(),
                logger: new FileLogger()
            );
            prodService.PlaceOrder("customer-42", "Mechanical Keyboard", 149.99m);

            // ── Configuration 2: Alternative (Mongo + PayPal + SendGrid) ──
            Console.WriteLine("══ ALTERNATIVE SETUP ══");
            var altService = new OrderService(
                repository: new MongoRepository(),
                paymentGateway: new PayPalGateway(),
                emailService: new SendGridEmailService(),
                logger: new ConsoleLogger()
            );
            altService.PlaceOrder("customer-99", "Ergonomic Chair", 599.99m);

            // ── Configuration 3: Testing (all mocks/in-memory) ──
            Console.WriteLine("══ TEST SETUP ══");
            var testService = new OrderService(
                repository: new InMemoryRepository(),
                paymentGateway: new StripeGateway(),  // Could be a mock
                emailService: new SendGridEmailService(), // Could be a mock
                logger: new ConsoleLogger()
            );
            testService.PlaceOrder("test-user", "Test Product", 9.99m);

            Console.WriteLine("═══════════════════════════════════════════════════");
            Console.WriteLine("✨ SAME OrderService class. ZERO code changes.");
            Console.WriteLine("✨ 3 different infrastructure configurations.");
            Console.WriteLine("✨ Switch databases? Change ONE line in DI config.");
            Console.WriteLine("✨ Switch payment? Change ONE line in DI config.");
            Console.WriteLine("✨ Unit test? Inject mocks. No real infra needed.");
            Console.WriteLine("✨ That's the Dependency Inversion Principle.");
        }
    }
}
