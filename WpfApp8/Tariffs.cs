namespace PCClubAdmin.Models
{
    public class Tariff
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal CostPerHour { get; set; }
        public string Description { get; set; }
    }
}