// 🏗️ REAL-WORLD SCENARIO — Notification System with Pluggable Channels
//
// Scenario: You're building a notification system for a SaaS platform.
// Users can receive notifications via Email, SMS, Push, Slack, Teams, etc.
// New channels should be addable WITHOUT touching existing notification logic.

using System;
using System.Collections.Generic;
using System.Linq;

namespace OCP.RealWorld
{
    // ══════════════════════════════════════════════════════
    // DOMAIN MODELS
    // ══════════════════════════════════════════════════════

    public enum NotificationPriority
    {
        Low,
        Medium,
        High,
        Critical
    }

    public class Notification
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string RecipientId { get; set; }
        public NotificationPriority Priority { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Dictionary<string, string> Metadata { get; set; } = new();
    }

    public class DeliveryResult
    {
        public bool Success { get; set; }
        public string Channel { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime DeliveredAt { get; set; }
    }

    // ══════════════════════════════════════════════════════
    // THE ABSTRACTION — Extension point for new channels
    // This interface NEVER changes. New channels implement it.
    // ══════════════════════════════════════════════════════

    public interface INotificationChannel
    {
        string ChannelName { get; }
        bool CanHandle(NotificationPriority priority);
        DeliveryResult Send(Notification notification);
    }

    // ══════════════════════════════════════════════════════
    // CHANNEL IMPLEMENTATIONS — Each is independent
    // ══════════════════════════════════════════════════════

    public class EmailChannel : INotificationChannel
    {
        public string ChannelName => "Email";

        public bool CanHandle(NotificationPriority priority)
            => true; // Email handles all priorities

        public DeliveryResult Send(Notification notification)
        {
            Console.WriteLine($"    📧 [EMAIL] To: {notification.RecipientId}");
            Console.WriteLine($"       Subject: {notification.Title}");
            Console.WriteLine($"       Body: {notification.Message}");

            return new DeliveryResult
            {
                Success = true,
                Channel = ChannelName,
                DeliveredAt = DateTime.UtcNow
            };
        }
    }

    public class SmsChannel : INotificationChannel
    {
        public string ChannelName => "SMS";

        public bool CanHandle(NotificationPriority priority)
            => priority >= NotificationPriority.High; // SMS only for urgent

        public DeliveryResult Send(Notification notification)
        {
            // SMS has character limits
            var truncatedMessage = notification.Message.Length > 140
                ? notification.Message[..137] + "..."
                : notification.Message;

            Console.WriteLine($"    📱 [SMS] To: {notification.RecipientId}");
            Console.WriteLine($"       Message: {truncatedMessage}");

            return new DeliveryResult
            {
                Success = true,
                Channel = ChannelName,
                DeliveredAt = DateTime.UtcNow
            };
        }
    }

    public class PushNotificationChannel : INotificationChannel
    {
        public string ChannelName => "Push Notification";

        public bool CanHandle(NotificationPriority priority)
            => priority >= NotificationPriority.Medium;

        public DeliveryResult Send(Notification notification)
        {
            Console.WriteLine($"    🔔 [PUSH] To: {notification.RecipientId}");
            Console.WriteLine($"       Title: {notification.Title}");
            Console.WriteLine($"       Body: {notification.Message[..Math.Min(50, notification.Message.Length)]}...");

            return new DeliveryResult
            {
                Success = true,
                Channel = ChannelName,
                DeliveredAt = DateTime.UtcNow
            };
        }
    }

    // ✨ ADDED LATER — Zero changes to NotificationService!
    public class SlackChannel : INotificationChannel
    {
        public string ChannelName => "Slack";

        public bool CanHandle(NotificationPriority priority)
            => true;

        public DeliveryResult Send(Notification notification)
        {
            var emoji = notification.Priority switch
            {
                NotificationPriority.Critical => "🚨",
                NotificationPriority.High => "⚠️",
                NotificationPriority.Medium => "📋",
                _ => "ℹ️"
            };

            Console.WriteLine($"    💬 [SLACK] Channel: #notifications");
            Console.WriteLine($"       {emoji} *{notification.Title}*");
            Console.WriteLine($"       {notification.Message}");

            return new DeliveryResult
            {
                Success = true,
                Channel = ChannelName,
                DeliveredAt = DateTime.UtcNow
            };
        }
    }

    // ✨ ADDED EVEN LATER — Still zero changes to NotificationService!
    public class MicrosoftTeamsChannel : INotificationChannel
    {
        public string ChannelName => "Microsoft Teams";

        public bool CanHandle(NotificationPriority priority)
            => priority >= NotificationPriority.Medium;

        public DeliveryResult Send(Notification notification)
        {
            Console.WriteLine($"    🟦 [TEAMS] Sending Adaptive Card...");
            Console.WriteLine($"       Title: {notification.Title}");
            Console.WriteLine($"       Priority: {notification.Priority}");
            Console.WriteLine($"       Body: {notification.Message}");

            return new DeliveryResult
            {
                Success = true,
                Channel = ChannelName,
                DeliveredAt = DateTime.UtcNow
            };
        }
    }

    // ══════════════════════════════════════════════════════
    // THE SERVICE — Closed for modification!
    // This class works with ANY number of channels.
    // It was written ONCE and NEVER needs to change.
    // ══════════════════════════════════════════════════════

    public class NotificationService
    {
        private readonly List<INotificationChannel> _channels;

        public NotificationService(IEnumerable<INotificationChannel> channels)
        {
            _channels = channels.ToList();
        }

        public List<DeliveryResult> SendNotification(Notification notification)
        {
            Console.WriteLine($"\n📨 Sending: \"{notification.Title}\" (Priority: {notification.Priority})");
            Console.WriteLine(new string('─', 55));

            var results = new List<DeliveryResult>();

            foreach (var channel in _channels)
            {
                if (channel.CanHandle(notification.Priority))
                {
                    try
                    {
                        var result = channel.Send(notification);
                        results.Add(result);
                        Console.WriteLine($"       ✅ Delivered via {channel.ChannelName}");
                    }
                    catch (Exception ex)
                    {
                        results.Add(new DeliveryResult
                        {
                            Success = false,
                            Channel = channel.ChannelName,
                            ErrorMessage = ex.Message
                        });
                        Console.WriteLine($"       ❌ Failed via {channel.ChannelName}: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"       ⏭️ Skipped {channel.ChannelName} (priority too low)");
                }
            }

            Console.WriteLine($"\n  📊 Delivered to {results.Count(r => r.Success)}/{results.Count} channels.");
            return results;
        }
    }

    // ══════════════════════════════════════════════════════
    // DEMO
    // ══════════════════════════════════════════════════════

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("🔔 NOTIFICATION SYSTEM — Open/Closed Principle Demo\n");

            // Register all channels (in a real app, use DI container)
            var channels = new List<INotificationChannel>
            {
                new EmailChannel(),
                new SmsChannel(),
                new PushNotificationChannel(),
                new SlackChannel(),               // Added without modifying anything!
                new MicrosoftTeamsChannel()        // Added without modifying anything!
            };

            var service = new NotificationService(channels);

            // Test 1: Low priority — only Email and Slack
            service.SendNotification(new Notification
            {
                Title = "Weekly Newsletter",
                Message = "Check out our latest blog post about SOLID principles!",
                RecipientId = "user@example.com",
                Priority = NotificationPriority.Low
            });

            // Test 2: High priority — most channels
            service.SendNotification(new Notification
            {
                Title = "Account Security Alert",
                Message = "Suspicious login detected from a new device in Istanbul, Turkey.",
                RecipientId = "user@example.com",
                Priority = NotificationPriority.High
            });

            // Test 3: Critical — ALL channels
            service.SendNotification(new Notification
            {
                Title = "🚨 SYSTEM DOWN",
                Message = "Production database is unreachable. Immediate action required!",
                RecipientId = "oncall@example.com",
                Priority = NotificationPriority.Critical
            });

            Console.WriteLine("\n" + new string('═', 55));
            Console.WriteLine("✨ 5 notification channels. NotificationService was NEVER modified.");
            Console.WriteLine("✨ Want to add WhatsApp? Discord? Telegram?");
            Console.WriteLine("✨ Just implement INotificationChannel. That's OCP.");
        }
    }
}
