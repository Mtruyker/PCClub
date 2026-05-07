using Microsoft.Data.Sqlite;
using PCClubAdmin.Models;
using System.Collections.Generic;

namespace PCClubAdmin.Services
{
    public class ClientRepository
    {
        private readonly string _connectionString;

        public ClientRepository()
        {
            _connectionString = DatabaseInitializer.GetConnectionString();
        }

        public List<Client> GetAll()
        {
            var clients = new List<Client>();

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Name, Phone, Email, Balance FROM Clients ORDER BY Id;";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                clients.Add(new Client
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Phone = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    Email = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                    Balance = reader.GetDecimal(4)
                });
            }

            return clients;
        }

        public int Add(Client client)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
                @"INSERT INTO Clients (Name, Phone, Email, Balance)
                  VALUES ($name, $phone, $email, $balance);
                  SELECT last_insert_rowid();";

            command.Parameters.AddWithValue("$name", client.Name);
            command.Parameters.AddWithValue("$phone", client.Phone ?? string.Empty);
            command.Parameters.AddWithValue("$email", client.Email ?? string.Empty);
            command.Parameters.AddWithValue("$balance", client.Balance);

            return (int)(long)command.ExecuteScalar();
        }

        public void Update(Client client)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
                @"UPDATE Clients
                  SET Name = $name, Phone = $phone, Email = $email, Balance = $balance
                  WHERE Id = $id;";

            command.Parameters.AddWithValue("$id", client.Id);
            command.Parameters.AddWithValue("$name", client.Name);
            command.Parameters.AddWithValue("$phone", client.Phone ?? string.Empty);
            command.Parameters.AddWithValue("$email", client.Email ?? string.Empty);
            command.Parameters.AddWithValue("$balance", client.Balance);
            command.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Clients WHERE Id = $id;";
            command.Parameters.AddWithValue("$id", id);
            command.ExecuteNonQuery();
        }
    }
}
