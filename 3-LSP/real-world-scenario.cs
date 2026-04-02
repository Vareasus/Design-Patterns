// 🏗️ REAL-WORLD SCENARIO — Payment Processing System
//
// Scenario: An e-commerce platform supports multiple payment methods.
// Each payment method must be substitutable — RefundablePayment,
// SubscriptionPayment, etc. must all behave correctly when used in place
// of the base payment processor.

using System;
using System.Collections.Generic;

namespace LSP.RealWorld
{
    // ══════════════════════════════════════════════════════
    // DOMAIN MODELS
    // ══════════════════════════════════════════════════════

    public class PaymentRequest
    {
        public string OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public string CustomerEmail { get; set; }
    }

    public class PaymentResult
    {
        public bool Success { get; set; }
        public string TransactionId { get; set; }
        public string Message { get; set; }
        public decimal ChargedAmount { get; set; }
    }

    // ══════════════════════════════════════════════════════
    // CONTRACTS — Define what payment processors PROMISE
    // Each is a separate, minimal contract.
    // ══════════════════════════════════════════════════════

    // Base contract: All payment processors can charge
    public interface IPaymentProcessor
    {
        string ProviderName { get; }
        PaymentResult ProcessPayment(PaymentRequest request);
        bool SupportsRecurring { get; }
    }

    // Extended contract: Some processors support refunds
    public interface IRefundable
    {
        PaymentResult Refund(string transactionId, decimal amount);
    }

    // Extended contract: Some processors support subscriptions
    public interface ISubscriptionCapable
    {
        string CreateSubscription(string customerId, decimal monthlyAmount);
        bool CancelSubscription(string subscriptionId);
    }

    // ══════════════════════════════════════════════════════
    // IMPLEMENTATIONS — Each fulfills its contracts FULLY
    // No NotImplementedException. No empty methods. No lies.
    // ══════════════════════════════════════════════════════

    // Credit card: Supports payments + refunds + subscriptions
    public class CreditCardProcessor : IPaymentProcessor, IRefundable, ISubscriptionCapable
    {
        public string ProviderName => "Stripe (Credit Card)";
        public bool SupportsRecurring => true;

        public PaymentResult ProcessPayment(PaymentRequest request)
        {
            // Simulate credit card charge
            var transactionId = $"CC-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
            Console.WriteLine($"    💳 Charging ${request.Amount} to credit card...");

            return new PaymentResult
            {
                Success = true,
                TransactionId = transactionId,
                Message = "Payment approved",
                ChargedAmount = request.Amount
            };
        }

        public PaymentResult Refund(string transactionId, decimal amount)
        {
            Console.WriteLine($"    💳 Refunding ${amount} for transaction {transactionId}...");
            return new PaymentResult
            {
                Success = true,
                TransactionId = $"REF-{transactionId}",
                Message = "Refund processed",
                ChargedAmount = -amount
            };
        }

        public string CreateSubscription(string customerId, decimal monthlyAmount)
        {
            var subId = $"SUB-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
            Console.WriteLine($"    💳 Created subscription {subId} for ${monthlyAmount}/month");
            return subId;
        }

        public bool CancelSubscription(string subscriptionId)
        {
            Console.WriteLine($"    💳 Cancelled subscription {subscriptionId}");
            return true;
        }
    }

    // PayPal: Supports payments + refunds (no subscriptions)
    public class PayPalProcessor : IPaymentProcessor, IRefundable
    {
        public string ProviderName => "PayPal";
        public bool SupportsRecurring => false; // Honest about capabilities!

        public PaymentResult ProcessPayment(PaymentRequest request)
        {
            var transactionId = $"PP-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
            Console.WriteLine($"    🅿️ PayPal charging ${request.Amount}...");

            return new PaymentResult
            {
                Success = true,
                TransactionId = transactionId,
                Message = "PayPal payment complete",
                ChargedAmount = request.Amount
            };
        }

        public PaymentResult Refund(string transactionId, decimal amount)
        {
            Console.WriteLine($"    🅿️ PayPal refunding ${amount} for {transactionId}...");
            return new PaymentResult
            {
                Success = true,
                TransactionId = $"PP-REF-{transactionId}",
                Message = "PayPal refund issued",
                ChargedAmount = -amount
            };
        }
    }

    // Crypto: Supports payments only (no refunds by nature, no subscriptions)
    public class CryptoProcessor : IPaymentProcessor
    {
        public string ProviderName => "Coinbase (Crypto)";
        public bool SupportsRecurring => false; // Honest!

        public PaymentResult ProcessPayment(PaymentRequest request)
        {
            var transactionId = $"BTC-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
            Console.WriteLine($"    ₿ Processing crypto payment of ${request.Amount}...");

            return new PaymentResult
            {
                Success = true,
                TransactionId = transactionId,
                Message = "Crypto payment confirmed (3 confirmations)",
                ChargedAmount = request.Amount
            };
        }

        // NOTE: No Refund() method. Crypto doesn't support refunds.
        // Instead of throwing NotImplementedException, it simply
        // DOESN'T implement IRefundable. This is HONEST design.
    }

    // Bank Transfer: Supports payments + refunds (slow but reliable)
    public class BankTransferProcessor : IPaymentProcessor, IRefundable
    {
        public string ProviderName => "Bank Transfer (ACH)";
        public bool SupportsRecurring => false;

        public PaymentResult ProcessPayment(PaymentRequest request)
        {
            var transactionId = $"ACH-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
            Console.WriteLine($"    🏦 Initiating bank transfer of ${request.Amount}...");
            Console.WriteLine($"    🏦 (Processing time: 2-3 business days)");

            return new PaymentResult
            {
                Success = true,
                TransactionId = transactionId,
                Message = "Bank transfer initiated",
                ChargedAmount = request.Amount
            };
        }

        public PaymentResult Refund(string transactionId, decimal amount)
        {
            Console.WriteLine($"    🏦 Initiating bank refund of ${amount}...");
            Console.WriteLine($"    🏦 (Refund time: 5-7 business days)");
            return new PaymentResult
            {
                Success = true,
                TransactionId = $"ACH-REF-{transactionId}",
                Message = "Bank refund initiated",
                ChargedAmount = -amount
            };
        }
    }

    // ══════════════════════════════════════════════════════
    // SERVICE — Works with ANY IPaymentProcessor
    // No type checks. No special cases. Pure substitution.
    // ══════════════════════════════════════════════════════

    public class CheckoutService
    {
        // Process payment with ANY processor — LSP in action
        public PaymentResult Checkout(PaymentRequest request, IPaymentProcessor processor)
        {
            Console.WriteLine($"\n  🛒 Checkout via {processor.ProviderName}");
            Console.WriteLine($"  Order: {request.OrderId} | Amount: ${request.Amount}");
            Console.WriteLine(new string('─', 50));

            var result = processor.ProcessPayment(request);

            if (result.Success)
                Console.WriteLine($"  ✅ Success! Transaction: {result.TransactionId}");
            else
                Console.WriteLine($"  ❌ Failed: {result.Message}");

            return result;
        }

        // Refund only if the processor supports it — checked via interface
        public PaymentResult RequestRefund(IPaymentProcessor processor, string transactionId, decimal amount)
        {
            Console.WriteLine($"\n  💰 Refund Request via {processor.ProviderName}");
            Console.WriteLine(new string('─', 50));

            if (processor is IRefundable refundable)
            {
                var result = refundable.Refund(transactionId, amount);
                Console.WriteLine($"  ✅ Refund {(result.Success ? "approved" : "failed")}");
                return result;
            }
            else
            {
                Console.WriteLine($"  ⚠️ {processor.ProviderName} does not support refunds.");
                Console.WriteLine($"  ℹ️ Please contact support for manual refund processing.");
                return new PaymentResult
                {
                    Success = false,
                    Message = $"{processor.ProviderName} does not support automated refunds"
                };
            }
        }
    }

    // ══════════════════════════════════════════════════════
    // DEMO
    // ══════════════════════════════════════════════════════

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║   Payment System — LSP in the Real World    ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝");

            var checkout = new CheckoutService();

            var request = new PaymentRequest
            {
                OrderId = "ORD-7891",
                Amount = 299.99m,
                Currency = "USD",
                CustomerEmail = "customer@example.com"
            };

            // All processors are substitutable for IPaymentProcessor ✅
            var processors = new List<IPaymentProcessor>
            {
                new CreditCardProcessor(),
                new PayPalProcessor(),
                new CryptoProcessor(),
                new BankTransferProcessor()
            };

            // Process the same order through every processor — all work!
            var results = new Dictionary<string, PaymentResult>();
            foreach (var processor in processors)
            {
                var result = checkout.Checkout(request, processor);
                results[processor.ProviderName] = result;
            }

            // Try refunds — some support it, some don't (but none BREAK)
            Console.WriteLine("\n\n══ REFUND REQUESTS ══");
            foreach (var processor in processors)
            {
                var txId = results[processor.ProviderName].TransactionId;
                checkout.RequestRefund(processor, txId, 299.99m);
            }

            Console.WriteLine("\n" + new string('═', 50));
            Console.WriteLine("✨ 4 different payment processors.");
            Console.WriteLine("✨ All substitutable via IPaymentProcessor.");
            Console.WriteLine("✨ Refundable ones implement IRefundable — no exceptions thrown.");
            Console.WriteLine("✨ Non-refundable ones simply don't implement it — honest design.");
            Console.WriteLine("✨ That's LSP: every subtype honors its contract.");
        }
    }
}
