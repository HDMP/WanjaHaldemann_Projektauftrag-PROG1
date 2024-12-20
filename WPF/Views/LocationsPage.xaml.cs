using System.Windows.Controls;

namespace SwissAddressManager.WPF.Views
{
    public partial class LocationsPage : UserControl
    {
        public LocationsPage()
        {
            InitializeComponent();
            LoadAddresses();
        }

        private void LoadAddresses()
        {
            // Example: Replace with actual data source logic
            var sampleData = new[]
            {
                new { Street = "Main St", City = "Zurich", PostalCode = "8000" },
                new { Street = "Bahnhofstrasse", City = "Geneva", PostalCode = "1201" }
            };

            // Assuming a DataGrid is added in the XAML for displaying addresses
            // Replace DataGridName with the actual name of your DataGrid in XAML
            AddressDataGrid.ItemsSource = sampleData;
        }
    }
}
