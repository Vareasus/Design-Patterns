// ❌ BAD — Direct object creation with switch
using System;

namespace Factory.Bad
{
    public class PdfDocument { public void Generate() => Console.WriteLine("  📄 PDF generated"); }
    public class WordDocument { public void Generate() => Console.WriteLine("  📝 Word generated"); }
    public class ExcelDocument { public void Generate() => Console.WriteLine("  📊 Excel generated"); }

    public class DocumentService
    {
        // 💥 Knows about EVERY concrete type
        public object CreateDocument(string type)
        {
            switch (type)
            {
                case "pdf": return new PdfDocument();
                case "word": return new WordDocument();
                case "excel": return new ExcelDocument();
                // 💥 Add PowerPoint? Modify this method.
                default: throw new ArgumentException($"Unknown: {type}");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var service = new DocumentService();
            var doc = service.CreateDocument("pdf");
            Console.WriteLine("💥 Client tied to concrete types and switch statements.");
        }
    }
}
