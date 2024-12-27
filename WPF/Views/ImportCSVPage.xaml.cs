using System.Data;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using SwissAddressManager.Data.DatabaseContext;
using SwissAddressManager.Helpers;

namespace SwissAddressManager.WPF.Views
{
    public partial class ImportCSVPage : UserControl
    {
        private readonly AppDbContext _context;

        public ImportCSVPage(AppDbContext context)
        {
            InitializeComponent();
            _context = context;
        }

        private void SelectCSV_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var csvFilePath = openFileDialog.FileName;
                DataTable dataTable = CSVHelper.LoadCSV(csvFilePath);
                CSVDataGrid.ItemsSource = dataTable.DefaultView;
            }
        }

        private void SaveToDatabase_Click(object sender, RoutedEventArgs e)
        {
            // Logic to save CSV data to the database
        }
    }
}
