using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace Program
{
    public static class DatabaseHelper
    {
        private const string ConnStr =
            "Server=localhost;Port=3306;Database=fueltrack;Uid=root;Pwd=;CharSet=utf8mb4;";

        // ── helpers ──────────────────────────────────────────────────
        private static MySqlConnection Open()
        {
            var conn = new MySqlConnection(ConnStr);
            conn.Open();
            return conn;
        }

        // ── AUTH ─────────────────────────────────────────────────────
        public static User Login(string username, string password)
        {
            using var conn = Open();
            using var cmd = new MySqlCommand(
                "SELECT id, username, password, role FROM users WHERE username=@u AND password=@p LIMIT 1", conn);
            cmd.Parameters.AddWithValue("@u", username);
            cmd.Parameters.AddWithValue("@p", password);
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;
            return new User
            {
                Id = r.GetInt32("id"),
                Username = r.GetString("username"),
                Password = r.GetString("password"),
                Role = r.GetString("role")
            };
        }

        public static bool UsernameExists(string username)
        {
            using var conn = Open();
            using var cmd = new MySqlCommand(
                "SELECT COUNT(*) FROM users WHERE username=@u", conn);
            cmd.Parameters.AddWithValue("@u", username);
            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

        public static bool RegisterUser(string username, string password)
        {
            try
            {
                using var conn = Open();
                using var cmd = new MySqlCommand(
                    "INSERT INTO users (username, password, role) VALUES (@u, @p, 'user')", conn);
                cmd.Parameters.AddWithValue("@u", username);
                cmd.Parameters.AddWithValue("@p", password);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // ── FUEL PRICES ──────────────────────────────────────────────
        public static List<FuelPrice> GetAllPrices(string fuelType = null, DateTime? from = null, DateTime? to = null)
        {
            var list = new List<FuelPrice>();
            using var conn = Open();

            var sql = "SELECT * FROM fuel_prices WHERE 1=1";
            if (!string.IsNullOrEmpty(fuelType) && fuelType != "All") sql += " AND fuel_type=@ft";
            if (from.HasValue) sql += " AND effective_date >= @from";
            if (to.HasValue) sql += " AND effective_date <= @to";
            sql += " ORDER BY effective_date DESC, id DESC";

            using var cmd = new MySqlCommand(sql, conn);
            if (!string.IsNullOrEmpty(fuelType) && fuelType != "All") cmd.Parameters.AddWithValue("@ft", fuelType);
            if (from.HasValue) cmd.Parameters.AddWithValue("@from", from.Value.Date);
            if (to.HasValue) cmd.Parameters.AddWithValue("@to", to.Value.Date);

            using var r = cmd.ExecuteReader();
            while (r.Read()) list.Add(MapPrice(r));
            return list;
        }

        public static List<FuelPrice> GetLatestPrices()
        {
            var list = new List<FuelPrice>();
            using var conn = Open();
            const string sql = @"
                SELECT fp.*
                FROM fuel_prices fp
                INNER JOIN (
                    SELECT fuel_type, MAX(effective_date) AS max_date
                    FROM fuel_prices
                    GROUP BY fuel_type
                ) latest ON fp.fuel_type = latest.fuel_type AND fp.effective_date = latest.max_date
                ORDER BY fp.fuel_type";
            using var cmd = new MySqlCommand(sql, conn);
            using var r = cmd.ExecuteReader();
            while (r.Read()) list.Add(MapPrice(r));
            return list;
        }

        public static void AddPrice(FuelPrice fp)
        {
            using var conn = Open();
            const string sql = @"
                INSERT INTO fuel_prices (fuel_type, price, effective_date, station_name, notes, created_at)
                VALUES (@ft, @p, @ed, @sn, @n, @ca);
                SELECT LAST_INSERT_ID();";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@ft", fp.FuelType);
            cmd.Parameters.AddWithValue("@p", fp.Price);
            cmd.Parameters.AddWithValue("@ed", fp.EffectiveDate.Date);
            cmd.Parameters.AddWithValue("@sn", fp.StationName ?? "");
            cmd.Parameters.AddWithValue("@n", fp.Notes ?? "");
            cmd.Parameters.AddWithValue("@ca", DateTime.Now);
            fp.Id = Convert.ToInt32(cmd.ExecuteScalar());
            fp.CreatedAt = DateTime.Now;

            NotifyAllUsers(conn,
                $"🔔 New {fp.FuelType} price: ₱{fp.Price:F2} effective {fp.EffectiveDate:MMM dd, yyyy}");
        }

        public static void UpdatePrice(FuelPrice fp)
        {
            using var conn = Open();
            const string sql = @"
                UPDATE fuel_prices
                SET fuel_type=@ft, price=@p, effective_date=@ed, station_name=@sn, notes=@n
                WHERE id=@id";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@ft", fp.FuelType);
            cmd.Parameters.AddWithValue("@p", fp.Price);
            cmd.Parameters.AddWithValue("@ed", fp.EffectiveDate.Date);
            cmd.Parameters.AddWithValue("@sn", fp.StationName ?? "");
            cmd.Parameters.AddWithValue("@n", fp.Notes ?? "");
            cmd.Parameters.AddWithValue("@id", fp.Id);
            cmd.ExecuteNonQuery();

            NotifyAllUsers(conn,
                $"✏️ {fp.FuelType} price updated to ₱{fp.Price:F2} effective {fp.EffectiveDate:MMM dd, yyyy}");
        }

        public static void DeletePrice(int id)
        {
            using var conn = Open();
            using var cmd = new MySqlCommand("DELETE FROM fuel_prices WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        // ── NOTIFICATIONS ────────────────────────────────────────────
        public static List<Notification> GetUnreadNotifications(int userId)
        {
            var list = new List<Notification>();
            using var conn = Open();
            using var cmd = new MySqlCommand(
                "SELECT * FROM notifications WHERE user_id=@uid AND is_read=0 ORDER BY created_at DESC", conn);
            cmd.Parameters.AddWithValue("@uid", userId);
            using var r = cmd.ExecuteReader();
            while (r.Read()) list.Add(MapNotif(r));
            return list;
        }

        public static int GetUnreadCount(int userId)
        {
            using var conn = Open();
            using var cmd = new MySqlCommand(
                "SELECT COUNT(*) FROM notifications WHERE user_id=@uid AND is_read=0", conn);
            cmd.Parameters.AddWithValue("@uid", userId);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public static void MarkAllRead(int userId)
        {
            using var conn = Open();
            using var cmd = new MySqlCommand(
                "UPDATE notifications SET is_read=1 WHERE user_id=@uid AND is_read=0", conn);
            cmd.Parameters.AddWithValue("@uid", userId);
            cmd.ExecuteNonQuery();
        }

        // ── private helpers ──────────────────────────────────────────
        private static void NotifyAllUsers(MySqlConnection conn, string message)
        {
            var ids = new List<int>();
            using (var cmd = new MySqlCommand("SELECT id FROM users WHERE role='user'", conn))
            using (var r = cmd.ExecuteReader())
                while (r.Read()) ids.Add(r.GetInt32(0));

            foreach (var uid in ids)
            {
                using var ins = new MySqlCommand(
                    "INSERT INTO notifications (user_id, message, is_read, created_at) VALUES (@uid,@msg,0,@ca)", conn);
                ins.Parameters.AddWithValue("@uid", uid);
                ins.Parameters.AddWithValue("@msg", message);
                ins.Parameters.AddWithValue("@ca", DateTime.Now);
                ins.ExecuteNonQuery();
            }
        }

        private static FuelPrice MapPrice(MySqlDataReader r) => new FuelPrice
        {
            Id = r.GetInt32("id"),
            FuelType = r.GetString("fuel_type"),
            Price = r.GetDecimal("price"),
            EffectiveDate = r.GetDateTime("effective_date"),
            StationName = r.GetString("station_name"),
            Notes = r["notes"] == DBNull.Value ? "" : r.GetString("notes"),
            CreatedAt = r.GetDateTime("created_at")
        };

        private static Notification MapNotif(MySqlDataReader r) => new Notification
        {
            Id = r.GetInt32("id"),
            UserId = r.GetInt32("user_id"),
            Message = r.GetString("message"),
            IsRead = r.GetBoolean("is_read"),
            CreatedAt = r.GetDateTime("created_at")
        };
    }
}