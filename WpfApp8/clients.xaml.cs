using System.Windows.Controls;
using PCClubAdmin.ViewModels;

namespace PCClubAdmin.Views
{
    public partial class ClientsView : UserControl
    {
        public ClientsView()
        {
            InitializeComponent();
            DataContext = new ClientsViewModel();
        }
    }
}
