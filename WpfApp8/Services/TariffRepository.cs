using Microsoft.Data.Sqlite;
using PCClubAdmin.Models;
using System.Collections.Generic;

namespace PCClubAdmin.Services
{
    public class TariffRepository
    {
        private readonly string _connectionString = DatabaseInitializer.GetConnectionString();

        public List<Tariff> GetAll()
        {
            var tariffs = new List<Tariff>();

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Name, CostPerHour, Description FROM Tariffs ORDER BY Id;";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                tariffs.Add(new Tariff
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    CostPerHour = reader.GetDecimal(2),
                    Description = reader.IsDBNull(3) ? string.Empty : reader.GetString(3)
                });
            }

            return tariffs;
        }

        public int Add(Tariff tariff)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
                @"INSERT INTO Tariffs (Name, CostPerHour, Description)
                  VALUES ($name, $cost, $description);
                  SELECT last_insert_rowid();";
            command.Parameters.AddWithValue("$name", tariff.Name);
            command.Parameters.AddWithValue("$cost", tariff.CostPerHour);
            command.Parameters.AddWithValue("$description", tariff.Description ?? string.Empty);

            return (int)(long)command.ExecuteScalar();
        }

        public void Update(Tariff tariff)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
                @"UPDATE Tariffs
                  SET Name = $name, CostPerHour = $cost, Description = $description
                  WHERE Id = $id;";
            command.Parameters.AddWithValue("$id", tariff.Id);
            command.Parameters.AddWithValue("$name", tariff.Name);
            command.Parameters.AddWithValue("$cost", tariff.CostPerHour);
            command.Parameters.AddWithValue("$description", tariff.Description ?? string.Empty);
            command.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Tariffs WHERE Id = $id;";
            command.Parameters.AddWithValue("$id", id);
            command.ExecuteNonQuery();
        }
    }
}
