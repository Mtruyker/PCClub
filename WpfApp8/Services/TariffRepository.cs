using PCClubAdmin.Models;
using System.Collections.Generic;

namespace PCClubAdmin.Services
{
    public class TariffRepository
    {
        public List<Tariff> GetAll()
        {
            return ApiClient.Get<List<Tariff>>("api/tariffs");
        }

        public int Add(Tariff tariff)
        {
            var created = ApiClient.Post<Tariff>("api/tariffs", tariff);
            return created.Id;
        }

        public void Update(Tariff tariff)
        {
            ApiClient.Put<Tariff>($"api/tariffs/{tariff.Id}", tariff);
        }

        public void Delete(int id)
        {
            ApiClient.Delete($"api/tariffs/{id}");
        }
    }
}
