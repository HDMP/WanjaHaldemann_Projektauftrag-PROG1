using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using SwissAddressManager.Data.DatabaseContext;
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

            // Set default view to DashboardPage
            MainContentArea.Content = new DashboardPage();

            // Test database connection
            //TestDatabaseConnection();
        }

        private void TestDatabaseConnection()
        {
            try
            {
                var locationsCount = _context.Locations.Count();
                MessageBox.Show($"Database connection successful! Found {locationsCount} locations.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error connecting to database: {ex.Message}");
            }
        }
        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MinimizeWindow_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void BtnDashboard_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = new DashboardPage();
        }

        private void BtnImportCSV_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = new ImportCSVPage(_context);
        }

        private void BtnLocations_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = new LocationsPage(_context);
        }

        private void BtnAddress_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = new AddressPage(_context);
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = new SettingsPage();
        }

        private void BtnAbout_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = new AboutPage();
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
