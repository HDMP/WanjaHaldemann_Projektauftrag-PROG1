using System.Windows.Controls;
using SwissAddressManager.Data.DatabaseContext;
using SwissAddressManager.WPF.ViewModels;

namespace SwissAddressManager.WPF.Views
{
    public partial class DashboardPage : UserControl
    {
        public DashboardPage(AppDbContext context)
        {
            InitializeComponent();
            DataContext = new DashboardViewModel(context);
        }
    }
}
