using Microsoft.EntityFrameworkCore;
using SwissAddressManager.Data.DatabaseContext;
using SwissAddressManager.Data.Models;
using System.Windows;
using System.Windows.Input;
using SwissAddressManager.Services.Interfaces;
using SwissAddressManager.WPF.Views;

namespace SwissAddressManager.WPF.Views
{
    public partial class MainWindow : Window
    {
        private readonly AppDbContext _context;

        public MainWindow(AppDbContext context)
        {
            InitializeComponent();
            _context = context;
            MainContentArea.Content = new DashboardPage();  // Default page
        }

        // Helper function to confirm unsaved changes before navigating to another page
        private bool ConfirmUnsavedChanges()
        {
            if (MainContentArea.Content is IUnsavedChangesPage page)
            {
                return page.ConfirmUnsavedChanges();
            }
            return true; // No unsaved changes, or no IUnsavedChangesPage interface implemented
        }

        // Navigation to AddressPage
        private void BtnAddress_Click(object sender, RoutedEventArgs e)
        {
            if (ConfirmUnsavedChanges())
            {
                MainContentArea.Content = new AddressPage(_context);
            }
        }

        // Navigation to LocationsPage
        private void BtnLocations_Click(object sender, RoutedEventArgs e)
        {
            if (ConfirmUnsavedChanges())
            {
                MainContentArea.Content = new LocationsPage(_context);
            }
        }

        // Navigation to ImportCSVPage
        private void BtnImportCSV_Click(object sender, RoutedEventArgs e)
        {
            if (ConfirmUnsavedChanges())
            {
                MainContentArea.Content = new ImportCSVPage(_context);
            }
        }

        // For other buttons (Dashboard, Settings, etc.)
        private void BtnDashboard_Click(object sender, RoutedEventArgs e)
        {
            if (ConfirmUnsavedChanges())
            {
                MainContentArea.Content = new DashboardPage();
            }
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            if (ConfirmUnsavedChanges())
            {
                MainContentArea.Content = new SettingsPage();
            }
        }

        private void BtnAbout_Click(object sender, RoutedEventArgs e)
        {
            if (ConfirmUnsavedChanges())
            {
                MainContentArea.Content = new AboutPage();
            }
        }

        private void MinimizeWindow_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void HeaderGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
    }
}
