using PCClubAdmin.Infrastructure;
using PCClubAdmin.Models;
using PCClubAdmin.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace PCClubAdmin.ViewModels
{
    public class SessionsViewModel : ObservableObject
    {
        private readonly SessionRepository _sessionRepository;
        private readonly ClientRepository _clientRepository;
        private readonly ComputerRepository _computerRepository;
        private readonly TariffRepository _tariffRepository;
        private readonly IDialogService _dialogService;
        private bool _isAddDialogVisible;
        private string _selectedClientName;
        private string _selectedComputerName;
        private Tariff _selectedTariff;

        public SessionsViewModel(IDialogService dialogService = null)
        {
            _sessionRepository = new SessionRepository();
            _clientRepository = new ClientRepository();
            _computerRepository = new ComputerRepository();
            _tariffRepository = new TariffRepository();
            _dialogService = dialogService ?? new MessageBoxDialogService();

            Sessions = new ObservableCollection<Session>();
            AvailableClients = new ObservableCollection<string>();
            AvailableComputers = new ObservableCollection<string>();
            AvailableTariffs = new ObservableCollection<Tariff>();

            ShowAddDialogCommand = new RelayCommand(ShowAddDialog);
            SaveSessionCommand = new RelayCommand(SaveSession);
            CancelAddSessionCommand = new RelayCommand(CancelAddSession);
            RefreshSessionsCommand = new RelayCommand(Refresh);
            EndSessionCommand = new RelayCommand<Session>(EndSession);
            DeleteSessionCommand = new RelayCommand<Session>(DeleteSession);

            Refresh();
        }

        public ObservableCollection<Session> Sessions { get; }
        public ObservableCollection<string> AvailableClients { get; }
        public ObservableCollection<string> AvailableComputers { get; }
        public ObservableCollection<Tariff> AvailableTariffs { get; }

        public bool IsAddDialogVisible
        {
            get => _isAddDialogVisible;
            set => SetProperty(ref _isAddDialogVisible, value);
        }

        public string SelectedClientName
        {
            get => _selectedClientName;
            set => SetProperty(ref _selectedClientName, value);
        }

        public string SelectedComputerName
        {
            get => _selectedComputerName;
            set => SetProperty(ref _selectedComputerName, value);
        }

        public Tariff SelectedTariff
        {
            get => _selectedTariff;
            set => SetProperty(ref _selectedTariff, value);
        }

        public ICommand ShowAddDialogCommand { get; }
        public ICommand SaveSessionCommand { get; }
        public ICommand CancelAddSessionCommand { get; }
        public ICommand RefreshSessionsCommand { get; }
        public ICommand EndSessionCommand { get; }
        public ICommand DeleteSessionCommand { get; }

        private void Refresh()
        {
            Sessions.Clear();
            foreach (var session in _sessionRepository.GetAll())
            {
                Sessions.Add(session);
            }

            AvailableClients.Clear();
            foreach (var client in _clientRepository.GetAll())
            {
                AvailableClients.Add(client.Name);
            }

            AvailableComputers.Clear();
            foreach (var computer in _computerRepository.GetAll())
            {
                AvailableComputers.Add(computer.Name);
            }

            AvailableTariffs.Clear();
            foreach (var tariff in _tariffRepository.GetAll())
            {
                AvailableTariffs.Add(tariff);
            }

            if (string.IsNullOrWhiteSpace(SelectedClientName) && AvailableClients.Count > 0)
            {
                SelectedClientName = AvailableClients[0];
            }

            if (string.IsNullOrWhiteSpace(SelectedComputerName) && AvailableComputers.Count > 0)
            {
                SelectedComputerName = AvailableComputers[0];
            }

            if (SelectedTariff == null && AvailableTariffs.Count > 0)
            {
                SelectedTariff = AvailableTariffs[0];
            }
        }

        private void ShowAddDialog() => IsAddDialogVisible = true;

        private void SaveSession()
        {
            if (string.IsNullOrWhiteSpace(SelectedClientName) || string.IsNullOrWhiteSpace(SelectedComputerName) || SelectedTariff == null)
            {
                _dialogService.ShowWarning("Выберите клиента, компьютер и тариф.", "Ошибка");
                return;
            }

            if (Sessions.Any(s => s.ComputerName == SelectedComputerName && !s.EndTime.HasValue))
            {
                _dialogService.ShowWarning("Для этого компьютера уже есть активная сессия.", "Ошибка");
                return;
            }

            var session = new Session
            {
                ClientName = SelectedClientName,
                ComputerName = SelectedComputerName,
                StartTime = DateTime.Now,
                EndTime = null,
                TotalCost = null,
                TariffName = SelectedTariff.Name,
                HourlyRate = SelectedTariff.CostPerHour
            };

            session.Id = _sessionRepository.Add(session);
            Sessions.Insert(0, session);
            IsAddDialogVisible = false;
        }

        private void CancelAddSession() => IsAddDialogVisible = false;

        private void EndSession(Session session)
        {
            if (session == null || session.EndTime.HasValue)
            {
                return;
            }

            session.EndTime = DateTime.Now;
            var duration = session.EndTime.Value - session.StartTime;
            var hourlyRate = session.HourlyRate ?? 50m;
            session.TotalCost = (decimal)duration.TotalHours * hourlyRate;

            _sessionRepository.Complete(session);
            Refresh();
        }

        private void DeleteSession(Session session)
        {
            if (session == null)
            {
                return;
            }

            var confirmed = _dialogService.Confirm(
                $"Удалить сессию клиента '{session.ClientName}'?",
                "Подтверждение удаления");

            if (!confirmed)
            {
                return;
            }

            _sessionRepository.Delete(session.Id);
            Sessions.Remove(session);
        }
    }
}
