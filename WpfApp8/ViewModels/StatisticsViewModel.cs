using PCClubAdmin.Infrastructure;
using PCClubAdmin.Models;
using PCClubAdmin.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace PCClubAdmin.ViewModels
{
    public class StatisticsViewModel : ObservableObject
    {
        private readonly SessionRepository _sessionRepository;
        private readonly ComputerRepository _computerRepository;
        private string _selectedPeriod = "Сегодня";
        private string _selectedComputer = "Все компьютеры";
        private string _totalRevenue = "0";
        private string _sessionCount = "0";
        private string _averageDuration = "0 мин";
        private string _occupancyRate = "0%";

        public StatisticsViewModel()
        {
            _sessionRepository = new SessionRepository();
            _computerRepository = new ComputerRepository();

            Periods = new ObservableCollection<string> { "Сегодня", "Эта неделя", "Этот месяц" };
            Computers = new ObservableCollection<string> { "Все компьютеры" };
            RecentSessions = new ObservableCollection<Session>();

            ApplyFiltersCommand = new RelayCommand(ApplyFilters);
            RefreshStatisticsCommand = new RelayCommand(ApplyFilters);

            foreach (var computer in _computerRepository.GetAll())
            {
                Computers.Add(computer.Name);
            }

            ApplyFilters();
        }

        public ObservableCollection<string> Periods { get; }
        public ObservableCollection<string> Computers { get; }
        public ObservableCollection<Session> RecentSessions { get; }

        public string SelectedPeriod
        {
            get => _selectedPeriod;
            set => SetProperty(ref _selectedPeriod, value);
        }

        public string SelectedComputer
        {
            get => _selectedComputer;
            set => SetProperty(ref _selectedComputer, value);
        }

        public string TotalRevenue
        {
            get => _totalRevenue;
            set => SetProperty(ref _totalRevenue, value);
        }

        public string SessionCount
        {
            get => _sessionCount;
            set => SetProperty(ref _sessionCount, value);
        }

        public string AverageDuration
        {
            get => _averageDuration;
            set => SetProperty(ref _averageDuration, value);
        }

        public string OccupancyRate
        {
            get => _occupancyRate;
            set => SetProperty(ref _occupancyRate, value);
        }

        public ICommand ApplyFiltersCommand { get; }
        public ICommand RefreshStatisticsCommand { get; }

        private void ApplyFilters()
        {
            ResolvePeriod(out var startDate, out var endDate);
            var computer = SelectedComputer == "Все компьютеры" ? null : SelectedComputer;

            var sessions = _sessionRepository.GetByPeriod(startDate, endDate, computer);
            var closedSessions = sessions.Where(s => s.EndTime.HasValue).ToList();

            var totalRevenue = sessions.Sum(s => s.TotalCost ?? 0m);
            var avgMinutes = closedSessions.Count == 0
                ? 0
                : closedSessions.Average(s => (s.EndTime.Value - s.StartTime).TotalMinutes);

            TotalRevenue = totalRevenue.ToString("C");
            SessionCount = sessions.Count.ToString();
            AverageDuration = $"{Math.Round(avgMinutes)} мин";
            OccupancyRate = CalculateOccupancyRate(sessions, startDate, endDate);

            RecentSessions.Clear();
            foreach (var session in sessions.Take(20))
            {
                RecentSessions.Add(session);
            }
        }

        private void ResolvePeriod(out DateTime startDate, out DateTime endDate)
        {
            var today = DateTime.Today;

            switch (SelectedPeriod)
            {
                case "Эта неделя":
                    var delta = ((int)today.DayOfWeek + 6) % 7;
                    startDate = today.AddDays(-delta);
                    endDate = startDate.AddDays(7).AddSeconds(-1);
                    break;
                case "Этот месяц":
                    startDate = new DateTime(today.Year, today.Month, 1);
                    endDate = startDate.AddMonths(1).AddSeconds(-1);
                    break;
                default:
                    startDate = today;
                    endDate = today.AddDays(1).AddSeconds(-1);
                    break;
            }
        }

        private string CalculateOccupancyRate(System.Collections.Generic.List<Session> sessions, DateTime startDate, DateTime endDate)
        {
            if (sessions.Count == 0)
            {
                return "0%";
            }

            var computerCount = Math.Max(1, Computers.Count - 1);
            var maxHours = (endDate - startDate).TotalHours * computerCount;
            if (maxHours <= 0)
            {
                return "0%";
            }

            var busyHours = 0d;
            foreach (var session in sessions)
            {
                var end = session.EndTime ?? DateTime.Now;
                var clampedStart = session.StartTime < startDate ? startDate : session.StartTime;
                var clampedEnd = end > endDate ? endDate : end;

                if (clampedEnd > clampedStart)
                {
                    busyHours += (clampedEnd - clampedStart).TotalHours;
                }
            }

            var rate = (busyHours / maxHours) * 100;
            return $"{Math.Round(rate)}%";
        }
    }
}
