using PCClubAdmin.Models;
using System.Collections.Generic;

namespace PCClubAdmin.Services
{
    public class ComputerRepository
    {
        public List<Computer> GetAll()
        {
            return ApiClient.Get<List<Computer>>("api/computers");
        }

        public int Add(Computer computer)
        {
            var created = ApiClient.Post<Computer>("api/computers", computer);
            return created.Id;
        }

        public void Update(Computer computer)
        {
            ApiClient.Put<Computer>($"api/computers/{computer.Id}", computer);
        }

        public void Delete(int id)
        {
            ApiClient.Delete($"api/computers/{id}");
        }
    }
}
