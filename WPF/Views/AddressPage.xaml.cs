using Microsoft.EntityFrameworkCore;
using SwissAddressManager.Data.DatabaseContext;
using SwissAddressManager.Data.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SwissAddressManager.WPF.Views
{
    public partial class AddressPage : UserControl
    {
        private readonly AppDbContext _context;
        private ObservableCollection<Address> _addresses;
        private bool _isEditing = false;

        public AddressPage(AppDbContext context)
        {
            InitializeComponent();
            _context = context;
            LoadData();
        }

        private void LoadData()
        {
            var addresses = _context.Addresses
                .Include(a => a.Location) // Include the related Location entity
                .ToList();

            _addresses = new ObservableCollection<Address>(addresses);
            AddressDataGrid.ItemsSource = _addresses;
        }


        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            var filter = FilterTextBox.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(filter))
            {
                AddressDataGrid.ItemsSource = _addresses
                    .Where(a => a.FirstName.ToLower().Contains(filter) ||
                                a.LastName.ToLower().Contains(filter) ||
                                a.Street.ToLower().Contains(filter))
                    .ToList();
            }
        }

        private void ClearFilterButton_Click(object sender, RoutedEventArgs e)
        {
            FilterTextBox.Clear();
            AddressDataGrid.ItemsSource = _addresses;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            _isEditing = true;
            AddressDataGrid.IsReadOnly = false;
            ToggleButtons();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _context.SaveChanges();
                MessageBox.Show("Changes saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show("Error saving changes.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                _isEditing = false;
                AddressDataGrid.IsReadOnly = true;
                LoadData();
                ToggleButtons();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _isEditing = false;
            AddressDataGrid.IsReadOnly = true;
            LoadData();
            ToggleButtons();
        }

        private void ImportCSVButton_Click(object sender, RoutedEventArgs e)
        {
            var importCSVPage = new ImportCSVPage();
            var parentWindow = Window.GetWindow(this);
            parentWindow.Content = importCSVPage;
        }

        private void ToggleButtons()
        {
            EditButton.Visibility = _isEditing ? Visibility.Collapsed : Visibility.Visible;
            SaveButton.Visibility = _isEditing ? Visibility.Visible : Visibility.Collapsed;
            CancelButton.Visibility = _isEditing ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
