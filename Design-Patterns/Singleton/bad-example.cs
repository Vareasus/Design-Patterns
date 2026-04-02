// ❌ BAD — Naive Singleton (not thread-safe, not testable)
using System;

namespace Singleton.Bad
{
    // 💥 Not thread-safe — two threads can create two instances
    public class DatabaseConnection
    {
        private static DatabaseConnection _instance;

        public string ConnectionString { get; set; } = "Server=localhost;Database=MyApp";

        // Private constructor
        private DatabaseConnection()
        {
            Console.WriteLine("  ⚡ Creating new DB connection...");
        }

        // 💥 Race condition: two threads call this simultaneously
        public static DatabaseConnection Instance
        {
            get
            {
                if (_instance == null) // Thread A checks: null ✓
                {                      // Thread B checks: null ✓ (before A creates)
                    _instance = new DatabaseConnection(); // Both create!
                }
                return _instance;
            }
        }

        public void Query(string sql) => Console.WriteLine($"  📊 Executing: {sql}");
    }

    class Program
    {
        static void Main(string[] args)
        {
            var db = DatabaseConnection.Instance;
            db.Query("SELECT * FROM Users");
            Console.WriteLine("\n💥 Not thread-safe. Can't mock for tests. Hard to change.");
        }
    }
}
