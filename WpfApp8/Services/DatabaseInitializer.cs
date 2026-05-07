using Microsoft.Data.Sqlite;
using System;
using System.IO;

namespace PCClubAdmin.Services
{
    public static class DatabaseInitializer
    {
        private const string DbFileName = "pcclub.db";

        public static string GetConnectionString()
        {
            var appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "PCClubAdmin");

            Directory.CreateDirectory(appDataPath);
            var dbPath = Path.Combine(appDataPath, DbFileName);
            return $"Data Source={dbPath}";
        }

        public static void Initialize()
        {
            using var connection = new SqliteConnection(GetConnectionString());
            connection.Open();

            var createClients = connection.CreateCommand();
            createClients.CommandText =
                @"CREATE TABLE IF NOT EXISTS Clients (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Phone TEXT,
                    Email TEXT,
                    Balance REAL NOT NULL DEFAULT 0
                  );";
            createClients.ExecuteNonQuery();

            var createComputers = connection.CreateCommand();
            createComputers.CommandText =
                @"CREATE TABLE IF NOT EXISTS Computers (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    IsOccupied INTEGER NOT NULL DEFAULT 0
                  );";
            createComputers.ExecuteNonQuery();

            var createTariffs = connection.CreateCommand();
            createTariffs.CommandText =
                @"CREATE TABLE IF NOT EXISTS Tariffs (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    CostPerHour REAL NOT NULL,
                    Description TEXT
                  );";
            createTariffs.ExecuteNonQuery();

            var createSessions = connection.CreateCommand();
            createSessions.CommandText =
                @"CREATE TABLE IF NOT EXISTS Sessions (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ClientName TEXT NOT NULL,
                    ComputerName TEXT NOT NULL,
                    StartTime TEXT NOT NULL,
                    EndTime TEXT NULL,
                    TotalCost REAL NULL
                  );";
            createSessions.ExecuteNonQuery();

            EnsureColumnExists(connection, "Sessions", "TariffName", "TEXT");
            EnsureColumnExists(connection, "Sessions", "HourlyRate", "REAL");
        }

        private static void EnsureColumnExists(SqliteConnection connection, string tableName, string columnName, string sqlType)
        {
            using var pragma = connection.CreateCommand();
            pragma.CommandText = $"PRAGMA table_info({tableName});";

            using var reader = pragma.ExecuteReader();
            while (reader.Read())
            {
                var existingColumnName = reader.GetString(1);
                if (string.Equals(existingColumnName, columnName, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
            }

            using var alter = connection.CreateCommand();
            alter.CommandText = $"ALTER TABLE {tableName} ADD COLUMN {columnName} {sqlType};";
            alter.ExecuteNonQuery();
        }
    }
}
