using PCClubAdmin.Infrastructure;
using PCClubAdmin.Models;
using PCClubAdmin.Services;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Input;

namespace PCClubAdmin.ViewModels
{
    public class TariffsViewModel : ObservableObject
    {
        private readonly TariffRepository _repository;
        private bool _isAddDialogVisible;
        private string _tariffName;
        private string _tariffCost;
        private string _tariffDescription;
        private int? _editingTariffId;

        public TariffsViewModel()
        {
            _repository = new TariffRepository();
            Tariffs = new ObservableCollection<Tariff>();

            ShowAddDialogCommand = new RelayCommand(ShowAddDialog);
            SaveTariffCommand = new RelayCommand(SaveTariff);
            CancelAddTariffCommand = new RelayCommand(CancelAddTariff);
            RefreshTariffsCommand = new RelayCommand(LoadTariffs);
            EditTariffCommand = new RelayCommand<Tariff>(EditTariff);
            DeleteTariffCommand = new RelayCommand<Tariff>(DeleteTariff);

            ResetForm();
            SeedIfEmpty();
            LoadTariffs();
        }

        public ObservableCollection<Tariff> Tariffs { get; }

        public bool IsAddDialogVisible
        {
            get => _isAddDialogVisible;
            set => SetProperty(ref _isAddDialogVisible, value);
        }

        public string TariffName
        {
            get => _tariffName;
            set => SetProperty(ref _tariffName, value);
        }

        public string TariffCost
        {
            get => _tariffCost;
            set => SetProperty(ref _tariffCost, value);
        }

        public string TariffDescription
        {
            get => _tariffDescription;
            set => SetProperty(ref _tariffDescription, value);
        }

        public ICommand ShowAddDialogCommand { get; }
        public ICommand SaveTariffCommand { get; }
        public ICommand CancelAddTariffCommand { get; }
        public ICommand RefreshTariffsCommand { get; }
        public ICommand EditTariffCommand { get; }
        public ICommand DeleteTariffCommand { get; }
        public bool IsEditing => _editingTariffId.HasValue;
        public string DialogTitle => IsEditing ? "Редактирование тарифа" : "Добавление нового тарифа";
        public string SaveButtonText => IsEditing ? "Обновить" : "Сохранить";

        private void SeedIfEmpty()
        {
            if (_repository.GetAll().Count > 0)
            {
                return;
            }

            _repository.Add(new Tariff { Name = "Базовый", CostPerHour = 300m, Description = "Стандартный тариф для обычных пользователей" });
            _repository.Add(new Tariff { Name = "Премиум", CostPerHour = 450m, Description = "Повышенная скорость и приоритет в очереди" });
            _repository.Add(new Tariff { Name = "Ночной", CostPerHour = 150m, Description = "Скидка на использование в ночное время (22:00–08:00)" });
        }

        private void LoadTariffs()
        {
            Tariffs.Clear();
            foreach (var tariff in _repository.GetAll())
            {
                Tariffs.Add(tariff);
            }
        }

        private void ShowAddDialog()
        {
            _editingTariffId = null;
            ResetForm();
            OnPropertyChanged(nameof(IsEditing));
            OnPropertyChanged(nameof(DialogTitle));
            OnPropertyChanged(nameof(SaveButtonText));
            IsAddDialogVisible = true;
        }

        private void SaveTariff()
        {
            var name = (TariffName ?? string.Empty).Trim();
            var description = (TariffDescription ?? string.Empty).Trim();
            var costRaw = (TariffCost ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Введите название тарифа.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(costRaw, NumberStyles.Number, CultureInfo.CurrentCulture, out var cost) || cost <= 0)
            {
                MessageBox.Show("Введите корректную стоимость за час (положительное число).", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (IsEditing)
            {
                var existing = new Tariff
                {
                    Id = _editingTariffId.Value,
                    Name = name,
                    CostPerHour = cost,
                    Description = description
                };
                _repository.Update(existing);
                LoadTariffs();
            }
            else
            {
                var tariff = new Tariff
                {
                    Name = name,
                    CostPerHour = cost,
                    Description = description
                };
                tariff.Id = _repository.Add(tariff);
                Tariffs.Add(tariff);
            }

            _editingTariffId = null;
            OnPropertyChanged(nameof(IsEditing));
            OnPropertyChanged(nameof(DialogTitle));
            OnPropertyChanged(nameof(SaveButtonText));
            IsAddDialogVisible = false;
            ResetForm();
        }

        private void CancelAddTariff()
        {
            _editingTariffId = null;
            OnPropertyChanged(nameof(IsEditing));
            OnPropertyChanged(nameof(DialogTitle));
            OnPropertyChanged(nameof(SaveButtonText));
            IsAddDialogVisible = false;
            ResetForm();
        }

        private void EditTariff(Tariff tariff)
        {
            if (tariff == null)
            {
                return;
            }

            _editingTariffId = tariff.Id;
            TariffName = tariff.Name;
            TariffCost = tariff.CostPerHour.ToString(CultureInfo.CurrentCulture);
            TariffDescription = tariff.Description;
            OnPropertyChanged(nameof(IsEditing));
            OnPropertyChanged(nameof(DialogTitle));
            OnPropertyChanged(nameof(SaveButtonText));
            IsAddDialogVisible = true;
        }

        private void DeleteTariff(Tariff tariff)
        {
            if (tariff == null)
            {
                return;
            }

            var result = MessageBox.Show(
                $"Удалить тариф '{tariff.Name}'?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            _repository.Delete(tariff.Id);
            Tariffs.Remove(tariff);
        }

        private void ResetForm()
        {
            TariffName = string.Empty;
            TariffCost = string.Empty;
            TariffDescription = string.Empty;
        }
    }
}
