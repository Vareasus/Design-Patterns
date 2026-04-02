// ✅ GOOD — Thread-safe Singleton + DI-friendly approach
using System;

namespace Singleton.Good
{
    // Approach 1: Thread-safe with Lazy<T>
    public sealed class AppConfiguration
    {
        // Lazy<T> guarantees thread-safe, single initialization
        private static readonly Lazy<AppConfiguration> _instance =
            new(() => new AppConfiguration());

        public static AppConfiguration Instance => _instance.Value;

        // Settings
        public string AppName { get; set; } = "Design Patterns App";
        public string Version { get; set; } = "1.0.0";
        public string Environment { get; set; } = "Production";

        private AppConfiguration()
        {
            Console.WriteLine("  ⚙️ Configuration loaded (once!)");
        }

        public void Display()
        {
            Console.WriteLine($"    App: {AppName} v{Version}");
            Console.WriteLine($"    Env: {Environment}");
        }
    }

    // Approach 2: DI-friendly — register as singleton in DI container
    // This is the PREFERRED approach in modern apps
    public interface ILogService
    {
        void Log(string message);
        int LogCount { get; }
    }

    public class LogService : ILogService
    {
        private int _count = 0;

        public int LogCount => _count;

        public void Log(string message)
        {
            _count++;
            Console.WriteLine($"    📝 [{_count}] {message}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("⚙️ Singleton Pattern Demo\n");

            // Lazy singleton — same instance everywhere
            Console.WriteLine("── AppConfiguration (Lazy Singleton) ──");
            var config1 = AppConfiguration.Instance;
            var config2 = AppConfiguration.Instance;
            Console.WriteLine($"  Same instance? {ReferenceEquals(config1, config2)}"); // True
            config1.Display();

            // DI-friendly approach
            Console.WriteLine("\n── LogService (DI Singleton) ──");
            // In real app: builder.Services.AddSingleton<ILogService, LogService>();
            ILogService logger = new LogService(); // registered once in DI
            logger.Log("App started");
            logger.Log("User logged in");
            logger.Log("Order placed");
            Console.WriteLine($"  Total logs: {logger.LogCount}");

            Console.WriteLine("\n✨ Lazy<T> = thread-safe singleton.");
            Console.WriteLine("✨ DI container = testable singleton (can mock ILogService).");
        }
    }
}
