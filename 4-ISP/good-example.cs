// ✅ GOOD EXAMPLE — Following Interface Segregation Principle
// Small, focused interfaces. Each class implements only what it can do.

using System;

namespace ISP.Good
{
    public class Document
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public int Pages { get; set; }
    }

    // ══════════════════════════════════════════════════════
    // SMALL, FOCUSED INTERFACES — Pick what you need
    // ══════════════════════════════════════════════════════

    public interface IPrinter
    {
        void Print(Document doc);
    }

    public interface IScanner
    {
        void Scan(Document doc);
    }

    public interface IFaxMachine
    {
        void Fax(Document doc);
    }

    public interface IStapler
    {
        void Staple(Document doc);
    }

    public interface IPhotoCopier
    {
        void PhotoCopy(Document doc);
    }

    public interface IEmailSender
    {
        void EmailDocument(Document doc, string recipient);
    }

    public interface IFileStorage
    {
        void SaveToDisk(Document doc, string path);
    }

    public interface IOcrEngine
    {
        string RecognizeText(Document doc);
    }

    // ══════════════════════════════════════════════════════
    // IMPLEMENTATIONS — Each implements ONLY what it can do
    // No NotSupportedException. No empty methods. Clean.
    // ══════════════════════════════════════════════════════

    // Enterprise device — implements everything it supports
    public class EnterprisePrinter : IPrinter, IScanner, IFaxMachine, IStapler,
                                     IPhotoCopier, IEmailSender, IFileStorage, IOcrEngine
    {
        public void Print(Document doc)
            => Console.WriteLine($"  🖨️ [Enterprise] Printing '{doc.Title}' ({doc.Pages} pages)");

        public void Scan(Document doc)
            => Console.WriteLine($"  📷 [Enterprise] Scanning '{doc.Title}' at 600 DPI");

        public void Fax(Document doc)
            => Console.WriteLine($"  📠 [Enterprise] Faxing '{doc.Title}'");

        public void Staple(Document doc)
            => Console.WriteLine($"  📎 [Enterprise] Stapling '{doc.Title}'");

        public void PhotoCopy(Document doc)
            => Console.WriteLine($"  📄 [Enterprise] Photocopying '{doc.Title}'");

        public void EmailDocument(Document doc, string recipient)
            => Console.WriteLine($"  📧 [Enterprise] Emailing '{doc.Title}' to {recipient}");

        public void SaveToDisk(Document doc, string path)
            => Console.WriteLine($"  💾 [Enterprise] Saving '{doc.Title}' to {path}");

        public string RecognizeText(Document doc)
        {
            Console.WriteLine($"  🔍 [Enterprise] Running OCR on '{doc.Title}'");
            return doc.Content;
        }
    }

    // Simple printer — ONLY implements IPrinter. Clean and honest.
    public class SimplePrinter : IPrinter
    {
        public void Print(Document doc)
            => Console.WriteLine($"  🖨️ [Simple] Printing '{doc.Title}'");

        // No Scan, no Fax, no Staple — because it CAN'T do those things.
        // And it's NOT forced to pretend it can!
    }

    // Home scanner — prints and scans, nothing else
    public class HomeScanner : IPrinter, IScanner
    {
        public void Print(Document doc)
            => Console.WriteLine($"  🖨️ [Home] Printing '{doc.Title}'");

        public void Scan(Document doc)
            => Console.WriteLine($"  📷 [Home] Scanning '{doc.Title}' at 300 DPI");
    }

    // Old fax machine — only faxes
    public class OldFaxMachine : IFaxMachine
    {
        public void Fax(Document doc)
            => Console.WriteLine($"  📠 [OldFax] Faxing '{doc.Title}' the old-fashioned way");
    }

    // Modern scanner with email capability
    public class SmartScanner : IScanner, IEmailSender, IFileStorage, IOcrEngine
    {
        public void Scan(Document doc)
            => Console.WriteLine($"  📷 [Smart] High-res scanning '{doc.Title}'");

        public void EmailDocument(Document doc, string recipient)
            => Console.WriteLine($"  📧 [Smart] Emailing scanned '{doc.Title}' to {recipient}");

        public void SaveToDisk(Document doc, string path)
            => Console.WriteLine($"  💾 [Smart] Saving scanned '{doc.Title}' to {path}");

        public string RecognizeText(Document doc)
        {
            Console.WriteLine($"  🔍 [Smart] OCR processing '{doc.Title}'");
            return doc.Content;
        }
    }

    // ══════════════════════════════════════════════════════
    // CLIENT CODE — Depends only on what it needs
    // ══════════════════════════════════════════════════════

    // This service only needs printing — it depends on IPrinter, not IMultiFunctionDevice
    public class PrintService
    {
        private readonly IPrinter _printer;

        public PrintService(IPrinter printer)
        {
            _printer = printer;
        }

        public void PrintDocument(Document doc)
        {
            Console.WriteLine($"\n  📋 Print Service processing '{doc.Title}':");
            _printer.Print(doc);
            Console.WriteLine("  ✅ Print job complete.");
        }
    }

    // This service needs scan + email — only those two interfaces
    public class ScanAndEmailService
    {
        private readonly IScanner _scanner;
        private readonly IEmailSender _emailSender;

        public ScanAndEmailService(IScanner scanner, IEmailSender emailSender)
        {
            _scanner = scanner;
            _emailSender = emailSender;
        }

        public void ScanAndEmail(Document doc, string recipient)
        {
            Console.WriteLine($"\n  📋 Scan & Email Service processing '{doc.Title}':");
            _scanner.Scan(doc);
            _emailSender.EmailDocument(doc, recipient);
            Console.WriteLine("  ✅ Scan and email complete.");
        }
    }

    // ══════════════════════════════════════════════════════
    // DEMO
    // ══════════════════════════════════════════════════════

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("╔═══════════════════════════════════════════════╗");
            Console.WriteLine("║  ISP CORRECT — Small Focused Interfaces      ║");
            Console.WriteLine("╚═══════════════════════════════════════════════╝");

            var doc = new Document
            {
                Title = "Quarterly Report",
                Content = "Revenue increased by 25%...",
                Pages = 12
            };

            // Print service works with ANY IPrinter
            var printService1 = new PrintService(new SimplePrinter());
            printService1.PrintDocument(doc);

            var printService2 = new PrintService(new EnterprisePrinter());
            printService2.PrintDocument(doc);

            var printService3 = new PrintService(new HomeScanner()); // HomeScanner IS an IPrinter
            printService3.PrintDocument(doc);

            // Scan & Email service — needs specific capabilities
            var smartScanner = new SmartScanner();
            var scanEmailService = new ScanAndEmailService(smartScanner, smartScanner);
            scanEmailService.ScanAndEmail(doc, "boss@company.com");

            // Enterprise printer can be used wherever ANY of its interfaces is needed
            var enterprise = new EnterprisePrinter();
            var anotherScanEmail = new ScanAndEmailService(enterprise, enterprise);
            anotherScanEmail.ScanAndEmail(doc, "team@company.com");

            Console.WriteLine("\n" + new string('═', 50));
            Console.WriteLine("✨ SimplePrinter only implements IPrinter — clean!");
            Console.WriteLine("✨ SmartScanner implements 4 interfaces — by choice!");
            Console.WriteLine("✨ No NotSupportedException anywhere.");
            Console.WriteLine("✨ Each client depends only on what it uses.");
            Console.WriteLine("✨ That's the Interface Segregation Principle.");
        }
    }
}
