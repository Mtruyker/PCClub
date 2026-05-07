using System.Windows.Controls;
using PCClubAdmin.ViewModels;

namespace PCClubAdmin.Views
{
    public partial class ComputersView : UserControl
    {
        public ComputersView()
        {
            InitializeComponent();
            DataContext = new ComputersViewModel();
        }
    }
}