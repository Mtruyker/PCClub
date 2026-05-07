namespace PCClubAdmin.Models
{
    public class Computer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsOccupied { get; set; }

        public string StatusText => IsOccupied ? "Занят" : "Свободен";
    }
}
