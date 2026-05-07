using PCClubAdmin.Infrastructure;
using PCClubAdmin.Models;
using PCClubAdmin.Services;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Input;

namespace PCClubAdmin.ViewModels
{
    public class ClientsViewModel : ObservableObject
    {
        private readonly ClientRepository _repository;
        private bool _isAddDialogVisible;
        private string _clientName;
        private string _clientPhone;
        private string _clientEmail;
        private string _clientBalance;
        private int? _editingClientId;

        public ClientsViewModel()
        {
            _repository = new ClientRepository();
            Clients = new ObservableCollection<Client>();

            ShowAddDialogCommand = new RelayCommand(ShowAddDialog);
            SaveClientCommand = new RelayCommand(SaveClient);
            CancelAddClientCommand = new RelayCommand(CancelAddClient);
            RefreshClientsCommand = new RelayCommand(LoadClients);
            EditClientCommand = new RelayCommand<Client>(EditClient);
            DeleteClientCommand = new RelayCommand<Client>(DeleteClient);

            ResetForm();
            SeedIfEmpty();
            LoadClients();
        }

        public ObservableCollection<Client> Clients { get; }

        public bool IsAddDialogVisible
        {
            get => _isAddDialogVisible;
            set => SetProperty(ref _isAddDialogVisible, value);
        }

        public string ClientName
        {
            get => _clientName;
            set => SetProperty(ref _clientName, value);
        }

        public string ClientPhone
        {
            get => _clientPhone;
            set => SetProperty(ref _clientPhone, value);
        }

        public string ClientEmail
        {
            get => _clientEmail;
            set => SetProperty(ref _clientEmail, value);
        }

        public string ClientBalance
        {
            get => _clientBalance;
            set => SetProperty(ref _clientBalance, value);
        }

        public ICommand ShowAddDialogCommand { get; }
        public ICommand SaveClientCommand { get; }
        public ICommand CancelAddClientCommand { get; }
        public ICommand RefreshClientsCommand { get; }
        public ICommand EditClientCommand { get; }
        public ICommand DeleteClientCommand { get; }
        public bool IsEditing => _editingClientId.HasValue;
        public string DialogTitle => IsEditing ? "Редактирование клиента" : "Добавление нового клиента";
        public string SaveButtonText => IsEditing ? "Обновить" : "Сохранить";

        private void SeedIfEmpty()
        {
            if (_repository.GetAll().Count > 0)
            {
                return;
            }

            _repository.Add(new Client { Name = "Иванов Иван", Phone = "+7 (999) 123-45-67", Email = "ivan@mail.ru", Balance = 1500m });
            _repository.Add(new Client { Name = "Петров Пётр", Phone = "+7 (999) 234-56-78", Email = "petrov@gmail.com", Balance = 2300m });
            _repository.Add(new Client { Name = "Сидорова Анна", Phone = "+7 (999) 345-67-89", Email = "anna@yandex.ru", Balance = 850m });
        }

        private void LoadClients()
        {
            Clients.Clear();
            foreach (var client in _repository.GetAll())
            {
                Clients.Add(client);
            }
        }

        private void ShowAddDialog()
        {
            _editingClientId = null;
            ResetForm();
            OnPropertyChanged(nameof(IsEditing));
            OnPropertyChanged(nameof(DialogTitle));
            OnPropertyChanged(nameof(SaveButtonText));
            IsAddDialogVisible = true;
        }

        private void SaveClient()
        {
            var name = (ClientName ?? string.Empty).Trim();
            var phone = (ClientPhone ?? string.Empty).Trim();
            var email = (ClientEmail ?? string.Empty).Trim();
            var balanceRaw = (ClientBalance ?? "0").Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Введите имя клиента.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(balanceRaw, NumberStyles.Number, CultureInfo.CurrentCulture, out var balance) || balance < 0)
            {
                MessageBox.Show("Введите корректный баланс (неотрицательное число).", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (IsEditing)
            {
                var existing = new Client
                {
                    Id = _editingClientId.Value,
                    Name = name,
                    Phone = phone,
                    Email = email,
                    Balance = balance
                };
                _repository.Update(existing);
                LoadClients();
            }
            else
            {
                var client = new Client
                {
                    Name = name,
                    Phone = phone,
                    Email = email,
                    Balance = balance
                };

                client.Id = _repository.Add(client);
                Clients.Add(client);
            }

            _editingClientId = null;
            OnPropertyChanged(nameof(IsEditing));
            OnPropertyChanged(nameof(DialogTitle));
            OnPropertyChanged(nameof(SaveButtonText));
            IsAddDialogVisible = false;
            ResetForm();
        }

        private void CancelAddClient()
        {
            _editingClientId = null;
            OnPropertyChanged(nameof(IsEditing));
            OnPropertyChanged(nameof(DialogTitle));
            OnPropertyChanged(nameof(SaveButtonText));
            IsAddDialogVisible = false;
            ResetForm();
        }

        private void EditClient(Client client)
        {
            if (client == null)
            {
                return;
            }

            _editingClientId = client.Id;
            ClientName = client.Name;
            ClientPhone = client.Phone;
            ClientEmail = client.Email;
            ClientBalance = client.Balance.ToString(CultureInfo.CurrentCulture);
            OnPropertyChanged(nameof(IsEditing));
            OnPropertyChanged(nameof(DialogTitle));
            OnPropertyChanged(nameof(SaveButtonText));
            IsAddDialogVisible = true;
        }

        private void DeleteClient(Client client)
        {
            if (client == null)
            {
                return;
            }

            var result = MessageBox.Show(
                $"Удалить клиента '{client.Name}'?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            _repository.Delete(client.Id);
            Clients.Remove(client);
        }

        private void ResetForm()
        {
            ClientName = string.Empty;
            ClientPhone = "+7 (___) ___-__-__";
            ClientEmail = string.Empty;
            ClientBalance = "0";
        }
    }
}
