using PCClubAdmin.Infrastructure;
using PCClubAdmin.Models;
using PCClubAdmin.Services;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace PCClubAdmin.ViewModels
{
    public class ComputersViewModel : ObservableObject
    {
        private readonly ComputerRepository _repository;
        private bool _isAddDialogVisible;
        private string _computerName;
        private string _selectedStatus;
        private int? _editingComputerId;

        public ComputersViewModel()
        {
            _repository = new ComputerRepository();
            Computers = new ObservableCollection<Computer>();
            Statuses = new ObservableCollection<string> { "Свободен", "Занят" };

            ShowAddDialogCommand = new RelayCommand(ShowAddDialog);
            SaveComputerCommand = new RelayCommand(SaveComputer);
            CancelAddComputerCommand = new RelayCommand(CancelAddComputer);
            EditComputerCommand = new RelayCommand<Computer>(EditComputer);
            DeleteComputerCommand = new RelayCommand<Computer>(DeleteComputer);

            ResetForm();
            SeedIfEmpty();
            LoadComputers();
        }

        public ObservableCollection<Computer> Computers { get; }
        public ObservableCollection<string> Statuses { get; }

        public bool IsAddDialogVisible
        {
            get => _isAddDialogVisible;
            set => SetProperty(ref _isAddDialogVisible, value);
        }

        public string ComputerName
        {
            get => _computerName;
            set => SetProperty(ref _computerName, value);
        }

        public string SelectedStatus
        {
            get => _selectedStatus;
            set => SetProperty(ref _selectedStatus, value);
        }

        public ICommand ShowAddDialogCommand { get; }
        public ICommand SaveComputerCommand { get; }
        public ICommand CancelAddComputerCommand { get; }
        public ICommand EditComputerCommand { get; }
        public ICommand DeleteComputerCommand { get; }
        public bool IsEditing => _editingComputerId.HasValue;
        public string DialogTitle => IsEditing ? "Редактирование компьютера" : "Добавление нового компьютера";
        public string SaveButtonText => IsEditing ? "Обновить" : "Сохранить";

        private void SeedIfEmpty()
        {
            if (_repository.GetAll().Count > 0)
            {
                return;
            }

            _repository.Add(new Computer { Name = "ПК-01", IsOccupied = false });
            _repository.Add(new Computer { Name = "ПК-02", IsOccupied = true });
            _repository.Add(new Computer { Name = "ПК-03", IsOccupied = false });
        }

        private void LoadComputers()
        {
            Computers.Clear();
            foreach (var computer in _repository.GetAll())
            {
                Computers.Add(computer);
            }
        }

        private void ShowAddDialog()
        {
            _editingComputerId = null;
            ResetForm();
            OnPropertyChanged(nameof(IsEditing));
            OnPropertyChanged(nameof(DialogTitle));
            OnPropertyChanged(nameof(SaveButtonText));
            IsAddDialogVisible = true;
        }

        private void SaveComputer()
        {
            var name = (ComputerName ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Введите название компьютера.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (IsEditing)
            {
                var existing = new Computer
                {
                    Id = _editingComputerId.Value,
                    Name = name,
                    IsOccupied = SelectedStatus == "Занят"
                };
                existing.UpdateStatus();
                _repository.Update(existing);
                LoadComputers();
            }
            else
            {
                var computer = new Computer
                {
                    Name = name,
                    IsOccupied = SelectedStatus == "Занят"
                };
                computer.UpdateStatus();
                computer.Id = _repository.Add(computer);
                Computers.Add(computer);
            }

            _editingComputerId = null;
            OnPropertyChanged(nameof(IsEditing));
            OnPropertyChanged(nameof(DialogTitle));
            OnPropertyChanged(nameof(SaveButtonText));
            IsAddDialogVisible = false;
            ResetForm();
        }

        private void CancelAddComputer()
        {
            _editingComputerId = null;
            OnPropertyChanged(nameof(IsEditing));
            OnPropertyChanged(nameof(DialogTitle));
            OnPropertyChanged(nameof(SaveButtonText));
            IsAddDialogVisible = false;
            ResetForm();
        }

        private void EditComputer(Computer computer)
        {
            if (computer == null)
            {
                return;
            }

            _editingComputerId = computer.Id;
            ComputerName = computer.Name;
            SelectedStatus = computer.IsOccupied ? "Занят" : "Свободен";
            OnPropertyChanged(nameof(IsEditing));
            OnPropertyChanged(nameof(DialogTitle));
            OnPropertyChanged(nameof(SaveButtonText));
            IsAddDialogVisible = true;
        }

        private void DeleteComputer(Computer computer)
        {
            if (computer == null)
            {
                return;
            }

            var result = MessageBox.Show(
                $"Удалить компьютер '{computer.Name}'?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            _repository.Delete(computer.Id);
            Computers.Remove(computer);
        }

        private void ResetForm()
        {
            ComputerName = string.Empty;
            SelectedStatus = "Свободен";
        }
    }
}
