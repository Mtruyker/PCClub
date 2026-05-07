using PCClubAdmin.Infrastructure;
using PCClubAdmin.Models;
using PCClubAdmin.Services;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;

namespace PCClubAdmin.ViewModels
{
    public class TariffsViewModel : ObservableObject
    {
        private readonly TariffRepository _repository;
        private readonly IDialogService _dialogService;
        private bool _isAddDialogVisible;
        private string _tariffName;
        private string _tariffCost;
        private string _tariffDescription;
        private int? _editingTariffId;

        public TariffsViewModel(IDialogService dialogService = null)
        {
            _repository = new TariffRepository();
            _dialogService = dialogService ?? new MessageBoxDialogService();
            Tariffs = new ObservableCollection<Tariff>();

            ShowAddDialogCommand = new RelayCommand(ShowAddDialog);
            SaveTariffCommand = new RelayCommand(SaveTariff);
            CancelAddTariffCommand = new RelayCommand(CancelAddTariff);
            RefreshTariffsCommand = new RelayCommand(LoadTariffs);
            EditTariffCommand = new RelayCommand<Tariff>(EditTariff);
            DeleteTariffCommand = new RelayCommand<Tariff>(DeleteTariff);

            ResetForm();
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
                _dialogService.ShowWarning("Введите название тарифа.", "Ошибка");
                return;
            }

            if (!decimal.TryParse(costRaw, NumberStyles.Number, CultureInfo.CurrentCulture, out var cost) || cost <= 0)
            {
                _dialogService.ShowWarning("Введите корректную стоимость за час (положительное число).", "Ошибка");
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

            var confirmed = _dialogService.Confirm(
                $"Удалить тариф '{tariff.Name}'?",
                "Подтверждение удаления");

            if (!confirmed)
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
