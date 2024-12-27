using Microsoft.EntityFrameworkCore;
using SwissAddressManager.Data.DatabaseContext;
using SwissAddressManager.Data.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SwissAddressManager.WPF.Views
{
    public partial class LocationsPage : UserControl
    {
        private readonly AppDbContext _context;
        private ObservableCollection<Location> _locations;
        private bool _isEditing = false;

        public LocationsPage(AppDbContext context)
        {
            InitializeComponent();
            _context = context;
            LoadData();
        }

        private void LoadData()
        {
            // Load data from database into ObservableCollection
            var locations = _context.Locations.ToList();
            _locations = new ObservableCollection<Location>(locations);
            LocationsDataGrid.ItemsSource = _locations;
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
            _isEditing = false;
            LocationsDataGrid.IsReadOnly = true;
            LoadData(); // Revert changes by reloading data
            ToggleButtons();
        }

        private void ToggleButtons()
        {
            EditButton.Visibility = _isEditing ? Visibility.Collapsed : Visibility.Visible;
            SaveButton.Visibility = _isEditing ? Visibility.Visible : Visibility.Collapsed;
            CancelButton.Visibility = _isEditing ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
