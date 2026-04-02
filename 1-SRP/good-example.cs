// ✅ GOOD EXAMPLE — Following Single Responsibility Principle
// Each class has exactly ONE reason to change.
// Each class is responsible to ONE actor/stakeholder.

using System;
using System.Collections.Generic;

namespace SRP.Good
{
    // ══════════════════════════════════════════════════════
    // CLASS 1: Invoice (Data Model)
    // Reason to change: Only if the invoice DATA STRUCTURE changes
    // Actor: Product team / Business analysts
    // ══════════════════════════════════════════════════════
    public class Invoice
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public decimal TaxRate { get; set; }
    }

    // ══════════════════════════════════════════════════════
    // CLASS 2: InvoiceCalculator (Business Logic)
    // Reason to change: Only if CALCULATION RULES change
    // Actor: Finance / Accounting team
    // ══════════════════════════════════════════════════════
    public class InvoiceCalculator
    {
        public decimal CalculateSubtotal(Invoice invoice)
        {
            return invoice.Amount;
        }

        public decimal CalculateTax(Invoice invoice)
        {
            return invoice.Amount * invoice.TaxRate;
        }

        public decimal CalculateTotal(Invoice invoice)
        {
            return CalculateSubtotal(invoice) + CalculateTax(invoice);
        }

        public decimal ApplyDiscount(Invoice invoice, decimal discountPercentage)
        {
            var total = CalculateTotal(invoice);
            return total - (total * discountPercentage / 100);
        }
    }

    // ══════════════════════════════════════════════════════
    // CLASS 3: InvoicePrinter (Presentation)
    // Reason to change: Only if DISPLAY FORMAT changes
    // Actor: UI/UX team or Report team
    // ══════════════════════════════════════════════════════
    public class InvoicePrinter
    {
        private readonly InvoiceCalculator _calculator;

        public InvoicePrinter(InvoiceCalculator calculator)
        {
            _calculator = calculator;
        }

        public string PrintFormatted(Invoice invoice)
        {
            return $@"
╔══════════════════════════════════════╗
║           INVOICE #{invoice.Id}              
╠══════════════════════════════════════╣
║ Customer: {invoice.CustomerName}
║ Date:     {invoice.Date:yyyy-MM-dd}
║ ─────────────────────────────────── 
║ Subtotal: ${_calculator.CalculateSubtotal(invoice):F2}
║ Tax ({invoice.TaxRate:P0}): ${_calculator.CalculateTax(invoice):F2}
║ ───────────────────────────────────
║ TOTAL:    ${_calculator.CalculateTotal(invoice):F2}
╚══════════════════════════════════════╝";
        }

        public string ExportToCsv(Invoice invoice)
        {
            var total = _calculator.CalculateTotal(invoice);
            return $"{invoice.Id},{invoice.CustomerName},{invoice.Date:yyyy-MM-dd},{invoice.Amount},{invoice.TaxRate},{total}";
        }

        public string ExportToJson(Invoice invoice)
        {
            var total = _calculator.CalculateTotal(invoice);
            return $@"{{
  ""id"": {invoice.Id},
  ""customer"": ""{invoice.CustomerName}"",
  ""date"": ""{invoice.Date:yyyy-MM-dd}"",
  ""subtotal"": {_calculator.CalculateSubtotal(invoice)},
  ""tax"": {_calculator.CalculateTax(invoice)},
  ""total"": {total}
}}";
        }
    }

    // ══════════════════════════════════════════════════════
    // CLASS 4: InvoiceRepository (Persistence)
    // Reason to change: Only if STORAGE mechanism changes
    // Actor: DBA / Backend team
    // ══════════════════════════════════════════════════════
    public interface IInvoiceRepository
    {
        void Save(Invoice invoice);
        Invoice GetById(int id);
        void Delete(int id);
    }

    public class InvoiceRepository : IInvoiceRepository
    {
        // In a real app, this would use Entity Framework, Dapper, etc.
        private readonly Dictionary<int, Invoice> _store = new();

        public void Save(Invoice invoice)
        {
            _store[invoice.Id] = invoice;
            Console.WriteLine($"✅ Invoice #{invoice.Id} saved to database.");
        }

        public Invoice GetById(int id)
        {
            return _store.TryGetValue(id, out var invoice) ? invoice : null;
        }

        public void Delete(int id)
        {
            _store.Remove(id);
            Console.WriteLine($"🗑️ Invoice #{id} deleted from database.");
        }
    }

    // ══════════════════════════════════════════════════════
    // CLASS 5: InvoiceNotifier (Communication)
    // Reason to change: Only if NOTIFICATION method changes
    // Actor: Marketing / Communication team
    // ══════════════════════════════════════════════════════
    public interface IInvoiceNotifier
    {
        void SendInvoice(Invoice invoice, string formattedContent);
    }

    public class EmailInvoiceNotifier : IInvoiceNotifier
    {
        public void SendInvoice(Invoice invoice, string formattedContent)
        {
            Console.WriteLine($"📧 Email sent to {invoice.CustomerEmail}");
            Console.WriteLine($"   Subject: Invoice #{invoice.Id}");
            // In real app: use SmtpClient, SendGrid, etc.
        }
    }

    public class SmsInvoiceNotifier : IInvoiceNotifier
    {
        public void SendInvoice(Invoice invoice, string formattedContent)
        {
            Console.WriteLine($"📱 SMS sent for Invoice #{invoice.Id}");
            // In real app: use Twilio, etc.
        }
    }

    // ══════════════════════════════════════════════════════
    // USAGE — Composing the separate classes together
    // ══════════════════════════════════════════════════════
    class Program
    {
        static void Main(string[] args)
        {
            // Create the invoice (data)
            var invoice = new Invoice
            {
                Id = 1001,
                CustomerName = "Acme Corp",
                CustomerEmail = "billing@acme.com",
                Date = DateTime.Now,
                Amount = 1500.00m,
                TaxRate = 0.18m
            };

            // Each class does ONE thing — composed together
            var calculator = new InvoiceCalculator();
            var printer = new InvoicePrinter(calculator);
            var repository = new InvoiceRepository();
            var notifier = new EmailInvoiceNotifier();

            // Calculate
            var total = calculator.CalculateTotal(invoice);
            var discountedTotal = calculator.ApplyDiscount(invoice, 10);
            Console.WriteLine($"Total: ${total:F2}");
            Console.WriteLine($"With 10% discount: ${discountedTotal:F2}");

            // Print in different formats
            Console.WriteLine(printer.PrintFormatted(invoice));
            Console.WriteLine($"CSV: {printer.ExportToCsv(invoice)}");
            Console.WriteLine($"JSON:\n{printer.ExportToJson(invoice)}");

            // Save (no SQL dependency needed for testing!)
            repository.Save(invoice);

            // Notify
            var content = printer.PrintFormatted(invoice);
            notifier.SendInvoice(invoice, content);

            Console.WriteLine("\n✨ Each class had ONE job. Each can be tested independently.");
            Console.WriteLine("✨ Change the email provider? Only EmailInvoiceNotifier changes.");
            Console.WriteLine("✨ Change the database? Only InvoiceRepository changes.");
            Console.WriteLine("✨ Change the tax formula? Only InvoiceCalculator changes.");
        }
    }
}
