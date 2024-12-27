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
        public ObservableCollection<Location> Locations { get; private set; }
        private bool _isEditing = false;

        public AddressPage(AppDbContext context)
        {
            InitializeComponent();
            _context = context;
            LoadData();
        }

        private void LoadData()
        {
            // Load addresses with related locations
            var addresses = _context.Addresses.Include(a => a.Location).ToList();
            _addresses = new ObservableCollection<Address>(addresses);
            AddressDataGrid.ItemsSource = _addresses;

            // Load all locations for ComboBox
            var locations = _context.Locations.ToList();
            Locations = new ObservableCollection<Location>(locations);

            // Set DataContext to make Locations available for binding
            DataContext = this;
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            var filter = FilterTextBox.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(filter))
            {
                AddressDataGrid.ItemsSource = _addresses
                    .Where(a => a.FirstName.ToLower().Contains(filter) ||
                                a.LastName.ToLower().Contains(filter) ||
                                a.Street.ToLower().Contains(filter) ||
                                (a.Location != null && a.Location.PostalCode.ToLower().Contains(filter)))
                    .ToList();
            }
        }

        private void ClearFilterButton_Click(object sender, RoutedEventArgs e)
        {
            FilterTextBox.Clear();
            AddressDataGrid.ItemsSource = _addresses;
        }

        private void PostalCodeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem is Location selectedLocation)
            {
                var currentAddress = AddressDataGrid.SelectedItem as Address;
                if (currentAddress != null)
                {
                    currentAddress.PostalCodeID = selectedLocation.Id;
                    currentAddress.Location = selectedLocation;
                    AddressDataGrid.CommitEdit(); // Commit changes to update the UI
                }
            }
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
                foreach (var address in _addresses)
                {
                    if (address.Id == 0)
                    {
                        // New Address
                        _context.Addresses.Add(address);
                    }
                    else
                    {
                        // Update existing
                        var existing = _context.Addresses.FirstOrDefault(a => a.Id == address.Id);
                        if (existing != null)
                        {
                            _context.Entry(existing).CurrentValues.SetValues(address);
                        }
                    }
                }

                _context.SaveChanges();
                MessageBox.Show("Changes saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving changes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void AddLocationButton_Click(object sender, RoutedEventArgs e)
        {
            var parentWindow = Window.GetWindow(this) as MainWindow;
            parentWindow.MainContentArea.Content = new LocationsPage(_context);
        }

        private void ImportCSVButton_Click(object sender, RoutedEventArgs e)
        {
            var parentWindow = Window.GetWindow(this) as MainWindow;
            parentWindow.MainContentArea.Content = new ImportCSVPage(_context);
        }

        private void ToggleButtons()
        {
            EditButton.Visibility = _isEditing ? Visibility.Collapsed : Visibility.Visible;
            SaveButton.Visibility = _isEditing ? Visibility.Visible : Visibility.Collapsed;
            CancelButton.Visibility = _isEditing ? Visibility.Visible : Visibility.Collapsed;
            AddLocationButton.Visibility = _isEditing ? Visibility.Collapsed : Visibility.Visible;
            ImportCSVButton.Visibility = _isEditing ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
