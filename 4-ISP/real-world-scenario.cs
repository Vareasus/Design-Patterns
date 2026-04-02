// 🏗️ REAL-WORLD SCENARIO — User Management System
//
// Scenario: A SaaS platform has different user roles:
// - Regular users (can view profile, update settings)
// - Content creators (can also publish content)
// - Admins (can also manage users and view analytics)
// - Super admins (can also manage billing and system config)
//
// Without ISP: One giant IUser interface forces all roles to implement everything.
// With ISP: Each role implements only the interfaces it needs.

using System;
using System.Collections.Generic;
using System.Linq;

namespace ISP.RealWorld
{
    // ══════════════════════════════════════════════════════
    // DOMAIN MODELS
    // ══════════════════════════════════════════════════════

    public class UserProfile
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }

    public class Content
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime PublishedAt { get; set; }
    }

    public class AnalyticsReport
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public decimal Revenue { get; set; }
    }

    // ══════════════════════════════════════════════════════
    // SEGREGATED INTERFACES — Each defines one capability
    // ══════════════════════════════════════════════════════

    // All users can view and update their own profile
    public interface IProfileManager
    {
        UserProfile GetProfile();
        void UpdateProfile(string name, string email);
    }

    // All users can manage their settings
    public interface ISettingsManager
    {
        void UpdateNotificationPreferences(bool emailEnabled, bool smsEnabled);
        void ChangePassword(string oldPassword, string newPassword);
    }

    // Content creators can publish and manage content
    public interface IContentCreator
    {
        void PublishContent(Content content);
        List<Content> GetMyContent();
        void DeleteContent(string contentId);
    }

    // Admins can manage other users
    public interface IUserAdministrator
    {
        List<UserProfile> GetAllUsers();
        void SuspendUser(string userId);
        void ActivateUser(string userId);
        void ChangeUserRole(string userId, string newRole);
    }

    // Admins can view analytics
    public interface IAnalyticsViewer
    {
        AnalyticsReport GetDashboard();
        void ExportReport(string format);
    }

    // Super admins can manage billing
    public interface IBillingManager
    {
        decimal GetCurrentRevenue();
        void ProcessRefund(string orderId, decimal amount);
        void UpdatePricingPlan(string planId, decimal newPrice);
    }

    // Super admins can configure the system
    public interface ISystemConfigurator
    {
        void UpdateSystemSetting(string key, string value);
        void ToggleMaintenanceMode(bool enabled);
        void PurgeCache();
    }

    // ══════════════════════════════════════════════════════
    // IMPLEMENTATIONS — Each role implements ONLY what it needs
    // ══════════════════════════════════════════════════════

    // Regular user: Profile + Settings (that's it!)
    public class RegularUser : IProfileManager, ISettingsManager
    {
        private readonly UserProfile _profile;

        public RegularUser(string id, string name, string email)
        {
            _profile = new UserProfile { Id = id, Name = name, Email = email, Role = "User" };
        }

        public UserProfile GetProfile() => _profile;

        public void UpdateProfile(string name, string email)
        {
            _profile.Name = name;
            _profile.Email = email;
            Console.WriteLine($"    ✅ Profile updated: {name} ({email})");
        }

        public void UpdateNotificationPreferences(bool emailEnabled, bool smsEnabled)
            => Console.WriteLine($"    🔔 Notifications: Email={emailEnabled}, SMS={smsEnabled}");

        public void ChangePassword(string oldPassword, string newPassword)
            => Console.WriteLine($"    🔐 Password changed successfully.");
    }

    // Content creator: Profile + Settings + Content
    public class ContentCreator : IProfileManager, ISettingsManager, IContentCreator
    {
        private readonly UserProfile _profile;
        private readonly List<Content> _content = new();

        public ContentCreator(string id, string name, string email)
        {
            _profile = new UserProfile { Id = id, Name = name, Email = email, Role = "Creator" };
        }

        public UserProfile GetProfile() => _profile;
        public void UpdateProfile(string name, string email)
        {
            _profile.Name = name;
            _profile.Email = email;
        }

        public void UpdateNotificationPreferences(bool emailEnabled, bool smsEnabled)
            => Console.WriteLine($"    🔔 Creator notifications updated.");

        public void ChangePassword(string oldPassword, string newPassword)
            => Console.WriteLine($"    🔐 Password changed.");

        public void PublishContent(Content content)
        {
            content.PublishedAt = DateTime.UtcNow;
            _content.Add(content);
            Console.WriteLine($"    📝 Published: '{content.Title}'");
        }

        public List<Content> GetMyContent() => _content;

        public void DeleteContent(string contentId)
            => Console.WriteLine($"    🗑️ Content deleted.");
    }

    // Admin: Profile + Settings + User Management + Analytics
    public class AdminUser : IProfileManager, ISettingsManager, IUserAdministrator, IAnalyticsViewer
    {
        private readonly UserProfile _profile;

        public AdminUser(string id, string name, string email)
        {
            _profile = new UserProfile { Id = id, Name = name, Email = email, Role = "Admin" };
        }

        public UserProfile GetProfile() => _profile;
        public void UpdateProfile(string name, string email)
        {
            _profile.Name = name;
            _profile.Email = email;
        }

        public void UpdateNotificationPreferences(bool emailEnabled, bool smsEnabled)
            => Console.WriteLine($"    🔔 Admin notifications updated.");
        public void ChangePassword(string oldPassword, string newPassword)
            => Console.WriteLine($"    🔐 Password changed.");

        public List<UserProfile> GetAllUsers()
        {
            Console.WriteLine("    👥 Fetching all users...");
            return new List<UserProfile>
            {
                new() { Id = "1", Name = "Alice", Role = "User" },
                new() { Id = "2", Name = "Bob", Role = "Creator" }
            };
        }

        public void SuspendUser(string userId)
            => Console.WriteLine($"    ⛔ User {userId} suspended.");

        public void ActivateUser(string userId)
            => Console.WriteLine($"    ✅ User {userId} activated.");

        public void ChangeUserRole(string userId, string newRole)
            => Console.WriteLine($"    🔄 User {userId} role changed to {newRole}.");

        public AnalyticsReport GetDashboard()
        {
            Console.WriteLine("    📊 Loading analytics dashboard...");
            return new AnalyticsReport
            {
                TotalUsers = 15000,
                ActiveUsers = 8500,
                Revenue = 125000m
            };
        }

        public void ExportReport(string format)
            => Console.WriteLine($"    📄 Exporting report as {format}...");
    }

    // Super Admin: EVERYTHING
    public class SuperAdmin : IProfileManager, ISettingsManager, IUserAdministrator,
                               IAnalyticsViewer, IBillingManager, ISystemConfigurator
    {
        private readonly UserProfile _profile;

        public SuperAdmin(string id, string name, string email)
        {
            _profile = new UserProfile { Id = id, Name = name, Email = email, Role = "SuperAdmin" };
        }

        public UserProfile GetProfile() => _profile;
        public void UpdateProfile(string name, string email) { _profile.Name = name; _profile.Email = email; }
        public void UpdateNotificationPreferences(bool emailEnabled, bool smsEnabled)
            => Console.WriteLine($"    🔔 SuperAdmin notifications updated.");
        public void ChangePassword(string old, string @new) => Console.WriteLine("    🔐 Password changed.");

        public List<UserProfile> GetAllUsers()
        {
            Console.WriteLine("    👥 [SuperAdmin] Fetching ALL users across all tenants...");
            return new List<UserProfile>();
        }
        public void SuspendUser(string userId) => Console.WriteLine($"    ⛔ User {userId} suspended.");
        public void ActivateUser(string userId) => Console.WriteLine($"    ✅ User {userId} activated.");
        public void ChangeUserRole(string userId, string newRole)
            => Console.WriteLine($"    🔄 User {userId} → {newRole}.");

        public AnalyticsReport GetDashboard()
        {
            Console.WriteLine("    📊 [SuperAdmin] Full platform analytics...");
            return new AnalyticsReport { TotalUsers = 50000, ActiveUsers = 32000, Revenue = 890000m };
        }
        public void ExportReport(string format) => Console.WriteLine($"    📄 Exporting as {format}.");

        public decimal GetCurrentRevenue()
        {
            Console.WriteLine("    💰 Current MRR: $890,000");
            return 890000m;
        }
        public void ProcessRefund(string orderId, decimal amount)
            => Console.WriteLine($"    💸 Refund of ${amount} processed for order {orderId}.");
        public void UpdatePricingPlan(string planId, decimal newPrice)
            => Console.WriteLine($"    💲 Plan {planId} updated to ${newPrice}/month.");

        public void UpdateSystemSetting(string key, string value)
            => Console.WriteLine($"    ⚙️ System setting [{key}] = {value}");
        public void ToggleMaintenanceMode(bool enabled)
            => Console.WriteLine($"    🔧 Maintenance mode: {(enabled ? "ON" : "OFF")}");
        public void PurgeCache()
            => Console.WriteLine($"    🗑️ Cache purged!");
    }

    // ══════════════════════════════════════════════════════
    // DEMO
    // ══════════════════════════════════════════════════════

    class Program
    {
        // This function only needs IProfileManager — minimal dependency
        static void DisplayUserInfo(IProfileManager user)
        {
            var profile = user.GetProfile();
            Console.WriteLine($"    [{profile.Role}] {profile.Name} ({profile.Email})");
        }

        // This function only needs IAnalyticsViewer
        static void ShowDashboard(IAnalyticsViewer viewer)
        {
            var report = viewer.GetDashboard();
            Console.WriteLine($"    Total: {report.TotalUsers} | Active: {report.ActiveUsers} | Revenue: ${report.Revenue:N0}");
        }

        static void Main(string[] args)
        {
            Console.WriteLine("╔═══════════════════════════════════════════════════╗");
            Console.WriteLine("║  User Management System — ISP in the Real World  ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════╝");

            var regularUser = new RegularUser("1", "Alice", "alice@example.com");
            var creator = new ContentCreator("2", "Bob", "bob@example.com");
            var admin = new AdminUser("3", "Charlie", "charlie@example.com");
            var superAdmin = new SuperAdmin("4", "Diana", "diana@example.com");

            // All users are IProfileManager — works with everyone
            Console.WriteLine("\n── All Users (via IProfileManager) ──");
            DisplayUserInfo(regularUser);
            DisplayUserInfo(creator);
            DisplayUserInfo(admin);
            DisplayUserInfo(superAdmin);

            // Only creators can publish content
            Console.WriteLine("\n── Content Creation (via IContentCreator) ──");
            creator.PublishContent(new Content { Title = "SOLID Principles Guide", Body = "..." });

            // Only admins can manage users
            Console.WriteLine("\n── User Administration (via IUserAdministrator) ──");
            admin.SuspendUser("suspicious-user-42");
            admin.ChangeUserRole("2", "Premium Creator");

            // Only admins+ can view analytics
            Console.WriteLine("\n── Analytics (via IAnalyticsViewer) ──");
            ShowDashboard(admin);
            ShowDashboard(superAdmin);

            // Only super admins can manage billing and system
            Console.WriteLine("\n── Super Admin Only (via IBillingManager + ISystemConfigurator) ──");
            superAdmin.ProcessRefund("ORD-789", 49.99m);
            superAdmin.ToggleMaintenanceMode(false);
            superAdmin.PurgeCache();

            Console.WriteLine("\n" + new string('═', 55));
            Console.WriteLine("✨ RegularUser implements 2 interfaces — clean and focused.");
            Console.WriteLine("✨ ContentCreator adds 1 more — IContentCreator.");
            Console.WriteLine("✨ AdminUser adds IUserAdministrator + IAnalyticsViewer.");
            Console.WriteLine("✨ SuperAdmin implements all 7 — because it NEEDS all 7.");
            Console.WriteLine("✨ No one implements methods they can't use. That's ISP.");
        }
    }
}
