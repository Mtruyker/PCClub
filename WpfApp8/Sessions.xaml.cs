using System.Windows.Controls;
using PCClubAdmin.ViewModels;

namespace PCClubAdmin.Views
{
    public partial class SessionsView : UserControl
    {
        public SessionsView()
        {
            InitializeComponent();
            DataContext = new SessionsViewModel();
        }
    }
}