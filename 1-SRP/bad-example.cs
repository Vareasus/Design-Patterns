// ❌ BAD EXAMPLE — Violating Single Responsibility Principle
// This class has THREE reasons to change:
//   1. If invoice calculation logic changes (e.g., new tax rules)
//   2. If the printing/display format changes
//   3. If the database technology or schema changes

using System;
using System.Data.SqlClient;
using System.Net.Mail;

namespace SRP.Bad
{
    /// <summary>
    /// This "God Class" handles business logic, presentation, AND persistence.
    /// Any change in one area risks breaking the others.
    /// </summary>
    public class Invoice
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public decimal TaxRate { get; set; }

        // ──────────────────────────────────────────────
        // RESPONSIBILITY 1: Business Logic (Calculation)
        // Changed by: Finance/Accounting team
        // ──────────────────────────────────────────────
        public decimal CalculateSubtotal()
        {
            return Amount;
        }

        public decimal CalculateTax()
        {
            return Amount * TaxRate;
        }

        public decimal CalculateTotal()
        {
            return CalculateSubtotal() + CalculateTax();
        }

        public decimal ApplyDiscount(decimal discountPercentage)
        {
            var total = CalculateTotal();
            return total - (total * discountPercentage / 100);
        }

        // ──────────────────────────────────────────────
        // RESPONSIBILITY 2: Presentation (Formatting)
        // Changed by: UI/UX team or Report team
        // ──────────────────────────────────────────────
        public string PrintInvoice()
        {
            return $@"
╔══════════════════════════════════════╗
║           INVOICE #{Id}              ║
╠══════════════════════════════════════╣
║ Customer: {CustomerName}
║ Date:     {Date:yyyy-MM-dd}
║ ─────────────────────────────────── 
║ Subtotal: ${CalculateSubtotal():F2}
║ Tax ({TaxRate:P0}): ${CalculateTax():F2}
║ ───────────────────────────────────
║ TOTAL:    ${CalculateTotal():F2}
╚══════════════════════════════════════╝";
        }

        public string ExportToCsv()
        {
            return $"{Id},{CustomerName},{Date:yyyy-MM-dd},{Amount},{TaxRate},{CalculateTotal()}";
        }

        // ──────────────────────────────────────────────
        // RESPONSIBILITY 3: Persistence (Database)
        // Changed by: DBA or Backend team
        // ──────────────────────────────────────────────
        public void SaveToDatabase()
        {
            // Directly coupled to SQL Server!
            using (var connection = new SqlConnection("Server=localhost;Database=InvoiceDB;"))
            {
                connection.Open();
                var command = new SqlCommand(
                    "INSERT INTO Invoices (CustomerName, Date, Amount, TaxRate) " +
                    "VALUES (@name, @date, @amount, @tax)", connection);
                command.Parameters.AddWithValue("@name", CustomerName);
                command.Parameters.AddWithValue("@date", Date);
                command.Parameters.AddWithValue("@amount", Amount);
                command.Parameters.AddWithValue("@tax", TaxRate);
                command.ExecuteNonQuery();
            }
        }

        public void DeleteFromDatabase()
        {
            using (var connection = new SqlConnection("Server=localhost;Database=InvoiceDB;"))
            {
                connection.Open();
                var command = new SqlCommand(
                    "DELETE FROM Invoices WHERE Id = @id", connection);
                command.Parameters.AddWithValue("@id", Id);
                command.ExecuteNonQuery();
            }
        }

        // ──────────────────────────────────────────────
        // RESPONSIBILITY 4: Notification (Email)
        // Changed by: Marketing or Communication team
        // ──────────────────────────────────────────────
        public void SendInvoiceByEmail()
        {
            var client = new SmtpClient("smtp.company.com");
            var message = new MailMessage(
                "invoices@company.com",
                CustomerEmail,
                $"Invoice #{Id}",
                PrintInvoice()  // Coupled to presentation too!
            );
            client.Send(message);
        }
    }

    // ──────────────────────────────────────────────────────
    // 💥 PROBLEMS WITH THIS APPROACH:
    // ──────────────────────────────────────────────────────
    // 1. Can't test CalculateTotal() without a database connection string
    // 2. Can't change the email provider without touching the Invoice class
    // 3. Changing the CSV format risks breaking tax calculation
    // 4. Two developers can't work on email and printing independently
    // 5. The class will grow FOREVER as new requirements come in
    // ──────────────────────────────────────────────────────

    class Program
    {
        static void Main(string[] args)
        {
            var invoice = new Invoice
            {
                Id = 1001,
                CustomerName = "Acme Corp",
                CustomerEmail = "billing@acme.com",
                Date = DateTime.Now,
                Amount = 1500.00m,
                TaxRate = 0.18m
            };

            // All these operations are tangled in ONE class
            Console.WriteLine(invoice.PrintInvoice());
            Console.WriteLine($"CSV: {invoice.ExportToCsv()}");
            Console.WriteLine($"Total with 10% discount: ${invoice.ApplyDiscount(10):F2}");

            // These would fail without real infrastructure:
            // invoice.SaveToDatabase();    // Needs SQL Server running
            // invoice.SendInvoiceByEmail(); // Needs SMTP server running
        }
    }
}
