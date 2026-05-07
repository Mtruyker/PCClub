using Microsoft.Data.Sqlite;
using PCClubAdmin.Models;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace PCClubAdmin.Services
{
    public class SessionRepository
    {
        private readonly string _connectionString = DatabaseInitializer.GetConnectionString();

        public List<Session> GetAll()
        {
            var sessions = new List<Session>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, ClientName, ComputerName, StartTime, EndTime, TotalCost, TariffName, HourlyRate FROM Sessions ORDER BY Id DESC;";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                sessions.Add(new Session
                {
                    Id = reader.GetInt32(0),
                    ClientName = reader.GetString(1),
                    ComputerName = reader.GetString(2),
                    StartTime = DateTime.Parse(reader.GetString(3), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
                    EndTime = reader.IsDBNull(4)
                        ? null
                        : DateTime.Parse(reader.GetString(4), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
                    TotalCost = reader.IsDBNull(5) ? null : reader.GetDecimal(5),
                    TariffName = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                    HourlyRate = reader.IsDBNull(7) ? null : reader.GetDecimal(7)
                });
            }

            return sessions;
        }

        public List<Session> GetByPeriod(DateTime start, DateTime end, string computerName)
        {
            var sessions = new List<Session>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
                @"SELECT Id, ClientName, ComputerName, StartTime, EndTime, TotalCost, TariffName, HourlyRate
                  FROM Sessions
                  WHERE StartTime >= $start AND StartTime <= $end
                    AND ($computer IS NULL OR ComputerName = $computer)
                  ORDER BY StartTime DESC;";
            command.Parameters.AddWithValue("$start", start.ToString("O"));
            command.Parameters.AddWithValue("$end", end.ToString("O"));
            command.Parameters.AddWithValue("$computer", (object)computerName ?? DBNull.Value);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                sessions.Add(new Session
                {
                    Id = reader.GetInt32(0),
                    ClientName = reader.GetString(1),
                    ComputerName = reader.GetString(2),
                    StartTime = DateTime.Parse(reader.GetString(3), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
                    EndTime = reader.IsDBNull(4)
                        ? null
                        : DateTime.Parse(reader.GetString(4), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
                    TotalCost = reader.IsDBNull(5) ? null : reader.GetDecimal(5),
                    TariffName = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                    HourlyRate = reader.IsDBNull(7) ? null : reader.GetDecimal(7)
                });
            }

            return sessions;
        }

        public int Add(Session session)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
                @"INSERT INTO Sessions (ClientName, ComputerName, StartTime, EndTime, TotalCost, TariffName, HourlyRate)
                  VALUES ($client, $computer, $start, $end, $cost, $tariffName, $hourlyRate);
                  SELECT last_insert_rowid();";
            command.Parameters.AddWithValue("$client", session.ClientName);
            command.Parameters.AddWithValue("$computer", session.ComputerName);
            command.Parameters.AddWithValue("$start", session.StartTime.ToString("O"));
            command.Parameters.AddWithValue("$end", session.EndTime.HasValue ? session.EndTime.Value.ToString("O") : (object)DBNull.Value);
            command.Parameters.AddWithValue("$cost", session.TotalCost.HasValue ? session.TotalCost.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("$tariffName", string.IsNullOrWhiteSpace(session.TariffName) ? (object)DBNull.Value : session.TariffName);
            command.Parameters.AddWithValue("$hourlyRate", session.HourlyRate.HasValue ? session.HourlyRate.Value : (object)DBNull.Value);

            return (int)(long)command.ExecuteScalar();
        }

        public void Complete(Session session)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
                @"UPDATE Sessions
                  SET EndTime = $end, TotalCost = $cost
                  WHERE Id = $id;";
            command.Parameters.AddWithValue("$end", session.EndTime.Value.ToString("O"));
            command.Parameters.AddWithValue("$cost", session.TotalCost.Value);
            command.Parameters.AddWithValue("$id", session.Id);
            command.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Sessions WHERE Id = $id;";
            command.Parameters.AddWithValue("$id", id);
            command.ExecuteNonQuery();
        }
    }
}
