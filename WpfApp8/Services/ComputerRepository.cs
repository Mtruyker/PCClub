using Microsoft.Data.Sqlite;
using PCClubAdmin.Models;
using System.Collections.Generic;

namespace PCClubAdmin.Services
{
    public class ComputerRepository
    {
        private readonly string _connectionString = DatabaseInitializer.GetConnectionString();

        public List<Computer> GetAll()
        {
            var computers = new List<Computer>();

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Name, IsOccupied FROM Computers ORDER BY Id;";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var computer = new Computer
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    IsOccupied = reader.GetBoolean(2)
                };
                computer.UpdateStatus();
                computers.Add(computer);
            }

            return computers;
        }

        public int Add(Computer computer)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
                @"INSERT INTO Computers (Name, IsOccupied)
                  VALUES ($name, $isOccupied);
                  SELECT last_insert_rowid();";
            command.Parameters.AddWithValue("$name", computer.Name);
            command.Parameters.AddWithValue("$isOccupied", computer.IsOccupied ? 1 : 0);

            return (int)(long)command.ExecuteScalar();
        }

        public void Update(Computer computer)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
                @"UPDATE Computers
                  SET Name = $name, IsOccupied = $isOccupied
                  WHERE Id = $id;";
            command.Parameters.AddWithValue("$id", computer.Id);
            command.Parameters.AddWithValue("$name", computer.Name);
            command.Parameters.AddWithValue("$isOccupied", computer.IsOccupied ? 1 : 0);
            command.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Computers WHERE Id = $id;";
            command.Parameters.AddWithValue("$id", id);
            command.ExecuteNonQuery();
        }
    }
}
