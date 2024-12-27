using Microsoft.EntityFrameworkCore;
using SwissAddressManager.Data.DatabaseContext;
using SwissAddressManager.Data.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SwissAddressManager.Services.Interfaces;

namespace SwissAddressManager.WPF.Views
{
    public partial class AddressPage : UserControl, IUnsavedChangesPage
    {
        private readonly AppDbContext _context;
        private ObservableCollection<Address> _addresses;
        public ObservableCollection<Location> Locations { get; private set; }
        private bool _isEditing = false;
        private bool _hasUnsavedChanges = false;

        public AddressPage(AppDbContext context)
        {
            InitializeComponent();
            _context = context;
            LoadData();
        }

        public bool HasUnsavedChanges => _hasUnsavedChanges;

        public void ResetUnsavedChanges()
        {
            _hasUnsavedChanges = false;
        }

        public bool ConfirmUnsavedChanges()
        {
            if (_hasUnsavedChanges)
            {
                var result = MessageBox.Show(
                    "You have unsaved changes. Do you want to discard them?",
                    "Unsaved Changes",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                return result == MessageBoxResult.Yes;
            }
            return true;
        }

        private void MarkAsUnsaved()
        {
            _hasUnsavedChanges = true;
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

            ResetUnsavedChanges(); // Clear unsaved changes after data is loaded
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

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            _isEditing = true;
            AddressDataGrid.IsReadOnly = false;
            ToggleButtons();
            MarkAsUnsaved();
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
                ResetUnsavedChanges(); // Clear unsaved changes after saving
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
            if (ConfirmUnsavedChanges())
            {
                _isEditing = false;
                AddressDataGrid.IsReadOnly = true;
                LoadData();
                ToggleButtons();
            }
        }

        private void ToggleButtons()
        {
            EditButton.Visibility = _isEditing ? Visibility.Collapsed : Visibility.Visible;
            SaveButton.Visibility = _isEditing ? Visibility.Visible : Visibility.Collapsed;
            CancelButton.Visibility = _isEditing ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
