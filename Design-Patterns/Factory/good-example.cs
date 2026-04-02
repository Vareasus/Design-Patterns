// ✅ GOOD — Factory Method Pattern
using System;
using System.Collections.Generic;

namespace Factory.Good
{
    // Product interface
    public interface IDocument
    {
        string Type { get; }
        void Generate();
        void Save(string path);
    }

    // Concrete products
    public class PdfDocument : IDocument
    {
        public string Type => "PDF";
        public void Generate() => Console.WriteLine("    📄 PDF document generated with formatting");
        public void Save(string path) => Console.WriteLine($"    💾 Saved PDF to {path}");
    }

    public class WordDocument : IDocument
    {
        public string Type => "Word";
        public void Generate() => Console.WriteLine("    📝 Word document generated with styles");
        public void Save(string path) => Console.WriteLine($"    💾 Saved DOCX to {path}");
    }

    public class ExcelDocument : IDocument
    {
        public string Type => "Excel";
        public void Generate() => Console.WriteLine("    📊 Excel spreadsheet generated with formulas");
        public void Save(string path) => Console.WriteLine($"    💾 Saved XLSX to {path}");
    }

    // ✨ Added later — zero changes to existing code!
    public class MarkdownDocument : IDocument
    {
        public string Type => "Markdown";
        public void Generate() => Console.WriteLine("    📋 Markdown document generated");
        public void Save(string path) => Console.WriteLine($"    💾 Saved MD to {path}");
    }

    // Factory
    public static class DocumentFactory
    {
        private static readonly Dictionary<string, Func<IDocument>> _creators = new()
        {
            ["pdf"] = () => new PdfDocument(),
            ["word"] = () => new WordDocument(),
            ["excel"] = () => new ExcelDocument(),
            ["markdown"] = () => new MarkdownDocument(),
        };

        public static IDocument Create(string type)
        {
            if (_creators.TryGetValue(type.ToLower(), out var creator))
                return creator();
            throw new ArgumentException($"Unknown document type: {type}");
        }

        // Register new types without modifying existing code!
        public static void Register(string type, Func<IDocument> creator)
        {
            _creators[type.ToLower()] = creator;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("📄 Document Factory Demo\n");

            var types = new[] { "pdf", "word", "excel", "markdown" };
            foreach (var type in types)
            {
                Console.WriteLine($"  Creating {type.ToUpper()} document:");
                var doc = DocumentFactory.Create(type);
                doc.Generate();
                doc.Save($"/documents/report.{type}");
                Console.WriteLine();
            }

            Console.WriteLine("✨ Client only knows IDocument. Factory handles creation.");
            Console.WriteLine("✨ New format? Register it. Nothing else changes.");
        }
    }
}
