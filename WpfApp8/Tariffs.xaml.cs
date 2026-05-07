using System.Windows.Controls;
using PCClubAdmin.ViewModels;

namespace PCClubAdmin.Views
{
    public partial class TariffsView : UserControl
    {
        public TariffsView()
        {
            InitializeComponent();
            DataContext = new TariffsViewModel();
        }
    }
}