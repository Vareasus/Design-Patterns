// ❌ BAD EXAMPLE — Violating Interface Segregation Principle
// One massive interface forces ALL implementors to handle methods they can't do.

using System;

namespace ISP.Bad
{
    // ══════════════════════════════════════════════════════
    // THE FAT INTERFACE — Forces everyone to implement everything
    // ══════════════════════════════════════════════════════

    public class Document
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public int Pages { get; set; }
    }

    // 💥 This interface is a MONSTER — 8 methods!
    // Not every device can do all of these.
    public interface IMultiFunctionDevice
    {
        void Print(Document doc);
        void Scan(Document doc);
        void Fax(Document doc);
        void Staple(Document doc);
        void PhotoCopy(Document doc);
        void EmailDocument(Document doc, string recipient);
        void SaveToDisk(Document doc, string path);
        string OCR(Document doc); // Optical Character Recognition
    }

    // ── Implementation 1: Enterprise Printer ──
    // This high-end device CAN do everything. Fine here.
    public class EnterprisePrinter : IMultiFunctionDevice
    {
        public void Print(Document doc)
            => Console.WriteLine($"  🖨️ Printing '{doc.Title}' ({doc.Pages} pages)...");

        public void Scan(Document doc)
            => Console.WriteLine($"  📷 Scanning '{doc.Title}'...");

        public void Fax(Document doc)
            => Console.WriteLine($"  📠 Faxing '{doc.Title}'...");

        public void Staple(Document doc)
            => Console.WriteLine($"  📎 Stapling '{doc.Title}'...");

        public void PhotoCopy(Document doc)
            => Console.WriteLine($"  📄 Photocopying '{doc.Title}'...");

        public void EmailDocument(Document doc, string recipient)
            => Console.WriteLine($"  📧 Emailing '{doc.Title}' to {recipient}...");

        public void SaveToDisk(Document doc, string path)
            => Console.WriteLine($"  💾 Saving '{doc.Title}' to {path}...");

        public string OCR(Document doc)
        {
            Console.WriteLine($"  🔍 Running OCR on '{doc.Title}'...");
            return doc.Content;
        }
    }

    // ── Implementation 2: Simple Home Printer ──
    // 💥 This basic printer can ONLY print. But it's forced to implement EVERYTHING.
    public class SimplePrinter : IMultiFunctionDevice
    {
        public void Print(Document doc)
            => Console.WriteLine($"  🖨️ Printing '{doc.Title}'...");

        // 💥 Can't scan — but FORCED to implement it
        public void Scan(Document doc)
            => throw new NotSupportedException("SimplePrinter cannot scan!");

        // 💥 Can't fax — but FORCED to implement it
        public void Fax(Document doc)
            => throw new NotSupportedException("SimplePrinter cannot fax!");

        // 💥 Can't staple — but FORCED to implement it
        public void Staple(Document doc)
            => throw new NotSupportedException("SimplePrinter cannot staple!");

        // 💥 Can't photocopy — but FORCED to implement it
        public void PhotoCopy(Document doc)
            => throw new NotSupportedException("SimplePrinter cannot photocopy!");

        // 💥 Can't email — but FORCED to implement it
        public void EmailDocument(Document doc, string recipient)
            => throw new NotSupportedException("SimplePrinter cannot email!");

        // 💥 Can't save — but FORCED to implement it  
        public void SaveToDisk(Document doc, string path)
            => throw new NotSupportedException("SimplePrinter cannot save to disk!");

        // 💥 Can't OCR — but FORCED to implement it
        public string OCR(Document doc)
            => throw new NotSupportedException("SimplePrinter cannot do OCR!");
    }

    // ── Implementation 3: Old School Fax Machine ──
    // 💥 Can only fax. Seven useless methods.
    public class OldFaxMachine : IMultiFunctionDevice
    {
        public void Print(Document doc)
            => throw new NotSupportedException("FaxMachine cannot print!");

        public void Scan(Document doc)
            => throw new NotSupportedException("FaxMachine cannot scan!");

        public void Fax(Document doc)
            => Console.WriteLine($"  📠 Faxing '{doc.Title}' the old-fashioned way...");

        public void Staple(Document doc)
            => throw new NotSupportedException("FaxMachine cannot staple!");

        public void PhotoCopy(Document doc)
            => throw new NotSupportedException("FaxMachine cannot photocopy!");

        public void EmailDocument(Document doc, string recipient)
            => throw new NotSupportedException("FaxMachine doesn't know what email is!");

        public void SaveToDisk(Document doc, string path)
            => throw new NotSupportedException("FaxMachine has no disk!");

        public string OCR(Document doc)
            => throw new NotSupportedException("FaxMachine cannot do OCR!");
    }

    // ══════════════════════════════════════════════════════
    // THE PROBLEM IN ACTION
    // ══════════════════════════════════════════════════════

    class Program
    {
        // This method accepts IMultiFunctionDevice — but which methods are safe to call?
        // You DON'T KNOW without checking the concrete type! 💥
        static void ProcessDocument(IMultiFunctionDevice device, Document doc)
        {
            Console.WriteLine($"\n  Processing with: {device.GetType().Name}");
            Console.WriteLine(new string('─', 45));

            try { device.Print(doc); }
            catch (NotSupportedException ex) { Console.WriteLine($"  ❌ {ex.Message}"); }

            try { device.Scan(doc); }
            catch (NotSupportedException ex) { Console.WriteLine($"  ❌ {ex.Message}"); }

            try { device.Fax(doc); }
            catch (NotSupportedException ex) { Console.WriteLine($"  ❌ {ex.Message}"); }

            try { device.EmailDocument(doc, "boss@company.com"); }
            catch (NotSupportedException ex) { Console.WriteLine($"  ❌ {ex.Message}"); }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("╔═══════════════════════════════════════════════╗");
            Console.WriteLine("║  ISP VIOLATION — Fat Interface Problems      ║");
            Console.WriteLine("╚═══════════════════════════════════════════════╝");

            var doc = new Document
            {
                Title = "Quarterly Report",
                Content = "Revenue increased by 25%...",
                Pages = 12
            };

            // Enterprise printer — works fine (it can do everything)
            ProcessDocument(new EnterprisePrinter(), doc);

            // Simple printer — explodes 💥
            ProcessDocument(new SimplePrinter(), doc);

            // Fax machine — explodes 💥
            ProcessDocument(new OldFaxMachine(), doc);

            Console.WriteLine("\n💥 PROBLEMS:");
            Console.WriteLine("  1. SimplePrinter was forced to implement 7 methods it can't do");
            Console.WriteLine("  2. OldFaxMachine was forced to implement 7 methods it can't do");
            Console.WriteLine("  3. Callers don't know which methods are safe without try/catch");
            Console.WriteLine("  4. Adding a new method forces ALL classes to update");
        }
    }
}
