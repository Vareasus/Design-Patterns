// ❌ BAD EXAMPLE — Violating Dependency Inversion Principle
// High-level modules directly depend on low-level modules.
// Everything is tightly coupled. Nothing is testable.

using System;
using System.Collections.Generic;

namespace DIP.Bad
{
    // ══════════════════════════════════════════════════════
    // LOW-LEVEL MODULES — Concrete implementations
    // ══════════════════════════════════════════════════════

    // Concrete email sender — uses SMTP directly
    public class SmtpEmailSender
    {
        public void SendEmail(string to, string subject, string body)
        {
            Console.WriteLine($"  📧 [SMTP] Sending email to {to}");
            Console.WriteLine($"     Subject: {subject}");
            // In real code: uses System.Net.Mail.SmtpClient
        }
    }

    // Concrete database — uses SQL Server directly
    public class SqlServerDatabase
    {
        private readonly Dictionary<string, object> _data = new();

        public void Insert(string table, string id, object data)
        {
            _data[$"{table}:{id}"] = data;
            Console.WriteLine($"  💾 [SQL Server] INSERT INTO {table} VALUES ({id}, ...)");
        }

        public object Query(string table, string id)
        {
            Console.WriteLine($"  💾 [SQL Server] SELECT * FROM {table} WHERE Id = '{id}'");
            return _data.GetValueOrDefault($"{table}:{id}");
        }
    }

    // Concrete logger — writes to file directly
    public class FileLogger
    {
        public void Log(string message)
        {
            Console.WriteLine($"  📝 [File] LOG: {message}");
            // In real code: File.AppendAllText("app.log", message)
        }
    }

    // Concrete payment gateway — uses Stripe directly
    public class StripePaymentGateway
    {
        public bool Charge(string customerId, decimal amount)
        {
            Console.WriteLine($"  💳 [Stripe] Charging ${amount} to customer {customerId}");
            return true; // Simulate success
        }
    }

    // ══════════════════════════════════════════════════════
    // HIGH-LEVEL MODULE — Directly coupled to EVERYTHING 💥
    // ══════════════════════════════════════════════════════

    public class OrderService
    {
        // 💥 Direct dependency on CONCRETE classes
        // Not interfaces. Not abstractions. Concrete implementations.
        private readonly SqlServerDatabase _database = new SqlServerDatabase();
        private readonly SmtpEmailSender _emailSender = new SmtpEmailSender();
        private readonly FileLogger _logger = new FileLogger();
        private readonly StripePaymentGateway _paymentGateway = new StripePaymentGateway();

        public void PlaceOrder(string customerId, string productName, decimal price)
        {
            Console.WriteLine($"\n🛒 Placing order for {customerId}...\n");

            // Step 1: Log (coupled to FileLogger)
            _logger.Log($"Order started for {customerId}");

            // Step 2: Charge (coupled to Stripe)
            var charged = _paymentGateway.Charge(customerId, price);
            if (!charged)
            {
                _logger.Log("Payment failed!");
                return;
            }

            // Step 3: Save (coupled to SQL Server)
            var orderId = Guid.NewGuid().ToString("N")[..8];
            _database.Insert("Orders", orderId, new { customerId, productName, price });

            // Step 4: Notify (coupled to SMTP)
            _emailSender.SendEmail(
                $"{customerId}@example.com",
                "Order Confirmed!",
                $"Your order for {productName} (${price}) has been placed.");

            _logger.Log($"Order {orderId} completed.");
            Console.WriteLine($"\n  ✅ Order {orderId} placed.\n");
        }
    }

    // ══════════════════════════════════════════════════════
    // THE PROBLEMS
    // ══════════════════════════════════════════════════════

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("╔═══════════════════════════════════════════════╗");
            Console.WriteLine("║  DIP VIOLATION — Tight Coupling Everywhere   ║");
            Console.WriteLine("╚═══════════════════════════════════════════════╝");

            var orderService = new OrderService();
            orderService.PlaceOrder("customer-42", "Mechanical Keyboard", 149.99m);

            Console.WriteLine("💥 PROBLEMS WITH THIS APPROACH:");
            Console.WriteLine("─────────────────────────────────────────────");
            Console.WriteLine("  1. Can't switch from SQL Server to MongoDB without");
            Console.WriteLine("     editing OrderService source code.");
            Console.WriteLine("");
            Console.WriteLine("  2. Can't switch from Stripe to PayPal without");
            Console.WriteLine("     editing OrderService source code.");
            Console.WriteLine("");
            Console.WriteLine("  3. Can't unit test OrderService without:");
            Console.WriteLine("     - A real SQL Server instance running");
            Console.WriteLine("     - A real SMTP server running");
            Console.WriteLine("     - A real Stripe account configured");
            Console.WriteLine("     - A real file system for logs");
            Console.WriteLine("");
            Console.WriteLine("  4. OrderService creates its own dependencies with 'new'.");
            Console.WriteLine("     It's impossible to inject alternatives.");
            Console.WriteLine("");
            Console.WriteLine("  5. Every infrastructure change ripples through");
            Console.WriteLine("     business logic code.");
        }
    }
}
