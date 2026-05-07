using System;
using System.Globalization;

namespace PCClubAdmin.Models
{
    public class Session
    {
        public int Id { get; set; }
        public string ClientName { get; set; }
        public string ComputerName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string TariffName { get; set; }
        public decimal? HourlyRate { get; set; }

        // Исправленное вычисляемое свойство для продолжительности
        public string CalculatedDuration
        {
            get
            {
                if (!EndTime.HasValue)
                    return "Активна";

                TimeSpan duration = EndTime.Value - StartTime;
                int hours = (int)duration.TotalHours;
                int minutes = duration.Minutes;

                return $"{hours}ч {minutes}м";
            }
        }

        public decimal? TotalCost { get; set; }

        public string FormattedStartTime => StartTime.ToString("dd.MM.yyyy HH:mm", CultureInfo.CurrentCulture);

        public string FormattedEndTime => EndTime.HasValue
            ? EndTime.Value.ToString("dd.MM.yyyy HH:mm", CultureInfo.CurrentCulture)
            : "Активна";

        public string Duration => CalculatedDuration;

        public string FormattedCost => TotalCost.HasValue
            ? TotalCost.Value.ToString("C", CultureInfo.CurrentCulture)
            : "—";

        public string Status => EndTime.HasValue ? "Завершена" : "Активна";
    }
}