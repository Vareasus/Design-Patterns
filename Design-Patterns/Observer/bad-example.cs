// ❌ BAD — Direct coupling: Store knows about every notification type
using System;

namespace Observer.Bad
{
    public class OnlineStore
    {
        // 💥 Store directly coupled to every notification channel
        public void NotifyProductAvailable(string product, decimal price)
        {
            // Hardcoded email notification
            Console.WriteLine($"  📧 Email: '{product}' is now available at ${price}!");

            // Hardcoded SMS notification  
            Console.WriteLine($"  📱 SMS: '{product}' back in stock - ${price}");

            // Hardcoded app push notification
            Console.WriteLine($"  🔔 Push: '{product}' available! Get it now!");

            // 💥 Want Slack notification? Modify THIS method.
            // 💥 Want Telegram? Modify THIS method.
            // 💥 Want to remove SMS? Modify THIS method.
            // 💥 The store shouldn't know about notification details!
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var store = new OnlineStore();
            store.NotifyProductAvailable("iPhone 16", 999.99m);
            Console.WriteLine("\n💥 Adding any notification = modifying OnlineStore.");
        }
    }
}
