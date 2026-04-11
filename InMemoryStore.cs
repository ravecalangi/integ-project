using System;
using System.Collections.Generic;

namespace Program
{
    public static class InMemoryStore
    {
        public static List<FuelPrice> FuelPrices => DatabaseHelper.GetAllPrices();

        // ── AUTH ─────────────────────────────────────────────────────
        public static User Login(string username, string password)
            => DatabaseHelper.Login(username, password);

        public static bool UsernameExists(string username)
            => DatabaseHelper.UsernameExists(username);

        public static bool RegisterUser(string username, string password)
            => DatabaseHelper.RegisterUser(username, password);

        // ── PRICES ───────────────────────────────────────────────────
        public static List<FuelPrice> GetAllPrices(string fuelType = null, DateTime? from = null, DateTime? to = null)
            => DatabaseHelper.GetAllPrices(fuelType, from, to);

        public static List<FuelPrice> GetLatestPrices()
            => DatabaseHelper.GetLatestPrices();

        public static void AddPrice(FuelPrice fp)
            => DatabaseHelper.AddPrice(fp);

        public static void UpdatePrice(FuelPrice fp)
            => DatabaseHelper.UpdatePrice(fp);

        public static void DeletePrice(int id)
            => DatabaseHelper.DeletePrice(id);

        // ── NOTIFICATIONS ────────────────────────────────────────────
        public static List<Notification> GetUnreadNotifications(int userId)
            => DatabaseHelper.GetUnreadNotifications(userId);

        public static int GetUnreadCount(int userId)
            => DatabaseHelper.GetUnreadCount(userId);

        public static void MarkAllRead(int userId)
            => DatabaseHelper.MarkAllRead(userId);
    }
}