using PCClubAdmin.Models;
using System.Collections.Generic;

namespace PCClubAdmin.Services
{
    public class ClientRepository
    {
        public List<Client> GetAll()
        {
            return ApiClient.Get<List<Client>>("api/clients");
        }

        public int Add(Client client)
        {
            var created = ApiClient.Post<Client>("api/clients", client);
            return created.Id;
        }

        public void Update(Client client)
        {
            ApiClient.Put<Client>($"api/clients/{client.Id}", client);
        }

        public void Delete(int id)
        {
            ApiClient.Delete($"api/clients/{id}");
        }
    }
}
