using System.Windows.Media;

namespace PCClubAdmin.Models
{
    public class Computer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsOccupied { get; set; }

        // Свойства для отображения в UI
        public string StatusText { get; private set; }
        public Brush StatusColor { get; private set; }

        // Метод для обновления статуса
        public void UpdateStatus()
        {
            if (IsOccupied)
            {
                StatusText = "Занят";
                StatusColor = Brushes.Red;
            }
            else
            {
                StatusText = "Свободен";
                StatusColor = Brushes.Green;
            }
        }

        // Конструктор
        public Computer()
        {
            UpdateStatus(); // Инициализация при создании
        }
    }
}
