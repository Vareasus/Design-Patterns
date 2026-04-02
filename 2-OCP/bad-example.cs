// ❌ BAD EXAMPLE — Violating Open/Closed Principle
// Every time we add a new discount type, we MODIFY the existing method.
// This class is NOT closed for modification.

using System;
using System.Collections.Generic;

namespace OCP.Bad
{
    public class Product
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
    }

    // 💥 This class VIOLATES OCP
    // Adding a new discount type requires MODIFYING this class
    public class DiscountCalculator
    {
        public decimal CalculateDiscount(Product product, string discountType)
        {
            // This switch grows FOREVER as new discount types are added
            switch (discountType)
            {
                case "None":
                    return 0;

                case "Seasonal":
                    // 15% off during seasonal sales
                    return product.Price * 0.15m;

                case "Clearance":
                    // 30% off for clearance items
                    return product.Price * 0.30m;

                case "Employee":
                    // 25% off for employees
                    return product.Price * 0.25m;

                case "VIP":
                    // 20% off for VIP customers
                    return product.Price * 0.20m;

                // ⬇️ Every new type = MODIFY this class
                // case "StudentDiscount": ...
                // case "MilitaryDiscount": ...
                // case "SeniorDiscount": ...
                // case "BundleDiscount": ...
                // case "LoyaltyDiscount": ...
                // case "FirstPurchaseDiscount": ...
                //
                // 💥 This list NEVER stops growing.
                // 💥 Each addition risks breaking existing discounts.
                // 💥 Unit tests for ALL discounts need re-running.

                default:
                    throw new ArgumentException($"Unknown discount type: {discountType}");
            }
        }

        // Same problem with report generation
        public string GenerateReport(List<Product> products, string format)
        {
            switch (format)
            {
                case "Text":
                    var text = "=== Product Report ===\n";
                    foreach (var p in products)
                        text += $"{p.Name}: ${p.Price}\n";
                    return text;

                case "HTML":
                    var html = "<table><tr><th>Product</th><th>Price</th></tr>";
                    foreach (var p in products)
                        html += $"<tr><td>{p.Name}</td><td>${p.Price}</td></tr>";
                    html += "</table>";
                    return html;

                case "CSV":
                    var csv = "Name,Price\n";
                    foreach (var p in products)
                        csv += $"{p.Name},{p.Price}\n";
                    return csv;

                // Want XML? Markdown? JSON? YAML? 
                // Modify this class EVERY TIME. 💥

                default:
                    throw new ArgumentException($"Unknown format: {format}");
            }
        }
    }

    // ──────────────────────────────────────────────────────
    // 💥 PROBLEMS WITH THIS APPROACH:
    // ──────────────────────────────────────────────────────
    // 1. Adding "StudentDiscount" = editing DiscountCalculator (risk!)
    // 2. The class file grows with every new feature
    // 3. Multiple developers editing the same file = merge conflicts
    // 4. Can't add discounts without source code access 
    //    (third-party libraries can't extend this)
    // 5. All unit tests must be re-validated on every change
    // ──────────────────────────────────────────────────────

    class Program
    {
        static void Main(string[] args)
        {
            var product = new Product { Name = "Laptop", Price = 1000m };
            var calculator = new DiscountCalculator();

            Console.WriteLine($"Product: {product.Name} — ${product.Price}");
            Console.WriteLine($"Seasonal Discount: ${calculator.CalculateDiscount(product, "Seasonal")}");
            Console.WriteLine($"VIP Discount: ${calculator.CalculateDiscount(product, "VIP")}");
            Console.WriteLine($"Employee Discount: ${calculator.CalculateDiscount(product, "Employee")}");

            // 💥 This throws an exception — no easy way to handle new types
            try
            {
                calculator.CalculateDiscount(product, "Student");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"\n❌ {ex.Message}");
                Console.WriteLine("   To fix this, you'd have to MODIFY the DiscountCalculator class.");
            }
        }
    }
}
