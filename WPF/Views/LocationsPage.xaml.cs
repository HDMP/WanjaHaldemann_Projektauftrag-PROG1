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
    public partial class LocationsPage : UserControl, IUnsavedChangesPage
    {
        private readonly AppDbContext _context;
        private ObservableCollection<Location> _locations;
        private bool _isEditing = false;
        private bool _hasUnsavedChanges = false;

        public LocationsPage(AppDbContext context)
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
            // Load data from database into ObservableCollection
            var locations = _context.Locations.ToList();
            _locations = new ObservableCollection<Location>(locations);
            LocationsDataGrid.ItemsSource = _locations;
            ResetUnsavedChanges(); // Clear unsaved changes after data is loaded
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            var filter = FilterTextBox.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(filter))
            {
                LocationsDataGrid.ItemsSource = _locations
                    .Where(l => l.PostalCode.ToLower().Contains(filter) ||
                                l.City.ToLower().Contains(filter))
                    .ToList();
            }
        }

        private void ClearFilterButton_Click(object sender, RoutedEventArgs e)
        {
            FilterTextBox.Clear();
            LocationsDataGrid.ItemsSource = _locations;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            _isEditing = true;
            LocationsDataGrid.IsReadOnly = false;
            ToggleButtons();
            MarkAsUnsaved();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Ensure changes made in the ObservableCollection are tracked by DbContext
                foreach (var location in _locations)
                {
                    var existing = _context.Locations.FirstOrDefault(l => l.Id == location.Id);
                    if (existing != null)
                    {
                        _context.Entry(existing).CurrentValues.SetValues(location);
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
                LocationsDataGrid.IsReadOnly = true;
                LoadData(); // Refresh data after save
                ToggleButtons();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (ConfirmUnsavedChanges())
            {
                _isEditing = false;
                LocationsDataGrid.IsReadOnly = true;
                LoadData(); // Revert changes by reloading data
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
