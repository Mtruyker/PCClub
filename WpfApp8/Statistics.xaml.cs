using System.Windows.Controls;
using PCClubAdmin.ViewModels;

namespace PCClubAdmin.Views
{
    public partial class StatisticsView : UserControl
    {
        public StatisticsView()
        {
            InitializeComponent();
            DataContext = new StatisticsViewModel();
        }
    }
}