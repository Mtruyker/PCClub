using System.Windows;
using System.Windows.Controls;

namespace PCClubAdmin
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // При запуске открываем экран с компьютерами
            MainFrame.Navigate(new Views.ComputersView());
        }

        private void NavigateToComputers(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Views.ComputersView());
        }

        private void NavigateToClients(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Views.ClientsView());
        }

        private void NavigateToSessions(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Views.SessionsView());
        }

        private void NavigateToTariffs(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Views.TariffsView());
        }

        private void NavigateToStatistics(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Views.StatisticsView());
        }
    }
}