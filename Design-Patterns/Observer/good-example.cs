// ✅ GOOD — Observer Pattern: Decoupled event system
using System;
using System.Collections.Generic;

namespace Observer.Good
{
    // Event data
    public class ProductEvent
    {
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    // Observer interface
    public interface IProductObserver
    {
        string Name { get; }
        void OnProductAvailable(ProductEvent e);
    }

    // Subject (Publisher)
    public class OnlineStore
    {
        private readonly List<IProductObserver> _observers = new();

        public void Subscribe(IProductObserver observer)
        {
            _observers.Add(observer);
            Console.WriteLine($"  ✅ {observer.Name} subscribed.");
        }

        public void Unsubscribe(IProductObserver observer)
        {
            _observers.Remove(observer);
            Console.WriteLine($"  ❌ {observer.Name} unsubscribed.");
        }

        public void NotifyProductAvailable(string product, decimal price)
        {
            Console.WriteLine($"\n  📢 '{product}' is now available at ${price}!");
            var e = new ProductEvent { ProductName = product, Price = price };
            foreach (var observer in _observers)
                observer.OnProductAvailable(e);
        }
    }

    // Concrete observers — each independent, each can be added/removed
    public class EmailNotifier : IProductObserver
    {
        public string Name => "Email Notifier";
        public void OnProductAvailable(ProductEvent e)
            => Console.WriteLine($"    📧 Email sent: '{e.ProductName}' at ${e.Price}");
    }

    public class SmsNotifier : IProductObserver
    {
        public string Name => "SMS Notifier";
        public void OnProductAvailable(ProductEvent e)
            => Console.WriteLine($"    📱 SMS sent: '{e.ProductName}' back in stock!");
    }

    public class SlackNotifier : IProductObserver
    {
        public string Name => "Slack Notifier";
        public void OnProductAvailable(ProductEvent e)
            => Console.WriteLine($"    💬 Slack: #{e.ProductName.ToLower().Replace(" ", "-")} is available!");
    }

    public class AnalyticsTracker : IProductObserver
    {
        public string Name => "Analytics Tracker";
        public void OnProductAvailable(ProductEvent e)
            => Console.WriteLine($"    📊 Analytics: tracked restock event for '{e.ProductName}'");
    }

    class Program
    {
        static void Main(string[] args)
        {
            var store = new OnlineStore();

            var email = new EmailNotifier();
            var sms = new SmsNotifier();
            var slack = new SlackNotifier();
            var analytics = new AnalyticsTracker();

            // Subscribe
            store.Subscribe(email);
            store.Subscribe(sms);
            store.Subscribe(slack);
            store.Subscribe(analytics);

            store.NotifyProductAvailable("MacBook Pro M4", 2499.99m);

            // Unsubscribe SMS
            Console.WriteLine();
            store.Unsubscribe(sms);
            store.NotifyProductAvailable("AirPods Pro 3", 249.99m);

            Console.WriteLine("\n✨ Store doesn't know about Email, SMS, Slack details.");
            Console.WriteLine("✨ Add/remove observers freely. Store never changes.");
        }
    }
}
