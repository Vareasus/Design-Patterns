// ✅ GOOD EXAMPLE — Following Open/Closed Principle
// Adding a new discount type = adding a new class. ZERO changes to existing code.

using System;
using System.Collections.Generic;
using System.Linq;

namespace OCP.Good
{
    public class Product
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
    }

    // ══════════════════════════════════════════════════════
    // THE ABSTRACTION — This is the "extension point"
    // New discount types implement this interface.
    // No switch statement. No if-else chain. Just polymorphism.
    // ══════════════════════════════════════════════════════

    public interface IDiscountStrategy
    {
        string Name { get; }
        decimal CalculateDiscount(Product product);
    }

    // ══════════════════════════════════════════════════════
    // IMPLEMENTATIONS — Each is a separate, independent class
    // Adding new ones NEVER touches existing code.
    // ══════════════════════════════════════════════════════

    public class NoDiscount : IDiscountStrategy
    {
        public string Name => "No Discount";
        public decimal CalculateDiscount(Product product) => 0;
    }

    public class SeasonalDiscount : IDiscountStrategy
    {
        public string Name => "Seasonal (15% off)";
        public decimal CalculateDiscount(Product product)
            => product.Price * 0.15m;
    }

    public class ClearanceDiscount : IDiscountStrategy
    {
        public string Name => "Clearance (30% off)";
        public decimal CalculateDiscount(Product product)
            => product.Price * 0.30m;
    }

    public class EmployeeDiscount : IDiscountStrategy
    {
        public string Name => "Employee (25% off)";
        public decimal CalculateDiscount(Product product)
            => product.Price * 0.25m;
    }

    public class VipDiscount : IDiscountStrategy
    {
        public string Name => "VIP (20% off)";
        public decimal CalculateDiscount(Product product)
            => product.Price * 0.20m;
    }

    // ✨ NEW DISCOUNT? Just add a new class. Nothing else changes!
    public class StudentDiscount : IDiscountStrategy
    {
        public string Name => "Student (10% off)";
        public decimal CalculateDiscount(Product product)
            => product.Price * 0.10m;
    }

    // ✨ Even complex, conditional discounts work seamlessly
    public class BuyMoreSaveMoreDiscount : IDiscountStrategy
    {
        private readonly int _quantity;

        public string Name => $"Buy More Save More ({_quantity}+ items)";

        public BuyMoreSaveMoreDiscount(int quantity)
        {
            _quantity = quantity;
        }

        public decimal CalculateDiscount(Product product)
        {
            // 5% per item over threshold
            var extraItems = Math.Max(0, _quantity - 1);
            var percentage = Math.Min(0.35m, extraItems * 0.05m); // Max 35%
            return product.Price * percentage;
        }
    }

    // ══════════════════════════════════════════════════════
    // THE CALCULATOR — Closed for modification!
    // This class NEVER needs to change when new discounts are added.
    // ══════════════════════════════════════════════════════

    public class DiscountCalculator
    {
        // This method works with ANY discount — past, present, or future
        public decimal ApplyDiscount(Product product, IDiscountStrategy strategy)
        {
            var discount = strategy.CalculateDiscount(product);
            return product.Price - discount;
        }

        // Compare all available discounts
        public void ShowAllDiscounts(Product product, IEnumerable<IDiscountStrategy> strategies)
        {
            Console.WriteLine($"\n📊 Discount Comparison for '{product.Name}' (${product.Price}):");
            Console.WriteLine(new string('─', 50));

            foreach (var strategy in strategies)
            {
                var discount = strategy.CalculateDiscount(product);
                var finalPrice = product.Price - discount;
                Console.WriteLine($"  {strategy.Name,-35} → ${finalPrice:F2} (save ${discount:F2})");
            }
        }
    }

    // ══════════════════════════════════════════════════════
    // REPORT FORMATTERS — Same principle applied to reports
    // ══════════════════════════════════════════════════════

    public interface IReportFormatter
    {
        string Format(List<Product> products);
    }

    public class TextReportFormatter : IReportFormatter
    {
        public string Format(List<Product> products)
        {
            var result = "=== Product Report ===\n";
            foreach (var p in products)
                result += $"  {p.Name}: ${p.Price}\n";
            return result;
        }
    }

    public class HtmlReportFormatter : IReportFormatter
    {
        public string Format(List<Product> products)
        {
            var html = "<table>\n<tr><th>Product</th><th>Price</th></tr>\n";
            foreach (var p in products)
                html += $"<tr><td>{p.Name}</td><td>${p.Price}</td></tr>\n";
            html += "</table>";
            return html;
        }
    }

    public class CsvReportFormatter : IReportFormatter
    {
        public string Format(List<Product> products)
        {
            var csv = "Name,Price\n";
            foreach (var p in products)
                csv += $"{p.Name},{p.Price}\n";
            return csv;
        }
    }

    // ✨ Want JSON? Just add a class. ReportGenerator never changes.
    public class JsonReportFormatter : IReportFormatter
    {
        public string Format(List<Product> products)
        {
            var items = products.Select(p => $"  {{ \"name\": \"{p.Name}\", \"price\": {p.Price} }}");
            return "[\n" + string.Join(",\n", items) + "\n]";
        }
    }

    // The report generator is CLOSED for modification
    public class ReportGenerator
    {
        public string GenerateReport(List<Product> products, IReportFormatter formatter)
        {
            return formatter.Format(products);
        }
    }

    // ══════════════════════════════════════════════════════
    // DEMO
    // ══════════════════════════════════════════════════════

    class Program
    {
        static void Main(string[] args)
        {
            var product = new Product { Name = "Laptop", Price = 1000m };
            var calculator = new DiscountCalculator();

            // All discount strategies — including ones added AFTER the calculator was written
            var strategies = new List<IDiscountStrategy>
            {
                new NoDiscount(),
                new SeasonalDiscount(),
                new ClearanceDiscount(),
                new EmployeeDiscount(),
                new VipDiscount(),
                new StudentDiscount(),            // ← Added later, zero changes!
                new BuyMoreSaveMoreDiscount(5)     // ← Added later, zero changes!
            };

            calculator.ShowAllDiscounts(product, strategies);

            // Apply a specific discount
            var finalPrice = calculator.ApplyDiscount(product, new VipDiscount());
            Console.WriteLine($"\n🛒 VIP Final Price: ${finalPrice:F2}");

            // Reports
            Console.WriteLine("\n── Report Formats ──");
            var products = new List<Product>
            {
                product,
                new Product { Name = "Mouse", Price = 49.99m },
                new Product { Name = "Keyboard", Price = 129.99m }
            };

            var reportGen = new ReportGenerator();
            Console.WriteLine("\n📄 CSV:");
            Console.WriteLine(reportGen.GenerateReport(products, new CsvReportFormatter()));
            Console.WriteLine("📄 JSON:");
            Console.WriteLine(reportGen.GenerateReport(products, new JsonReportFormatter()));

            Console.WriteLine("\n✨ Added 2 new discount types and 1 new report format.");
            Console.WriteLine("✨ DiscountCalculator and ReportGenerator were NEVER modified.");
            Console.WriteLine("✨ That's the Open/Closed Principle.");
        }
    }
}
