using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Microsoft.Win32;
using SwissAddressManager.Data.DatabaseContext;
using SwissAddressManager.Data.Models;
using SwissAddressManager.Helpers;
using SwissAddressManager.Services.Interfaces;
using SwissAddressManager.Services.Implementations;

namespace SwissAddressManager.WPF.Views
{
    public partial class ImportCSVPage : UserControl, IUnsavedChangesPage
    {
        private readonly AppDbContext _context;
        private DataTable _dataTable; // Store the imported CSV data
        private Dictionary<string, string> _columnMappings; // Mapping: CSV Column -> Database Field
        private bool _isValidated; // Tracks if the validation is complete

        public bool HasUnsavedChanges { get; private set; } // Tracks unsaved changes

        // Map CSV headers to database fields
        private readonly Dictionary<string, string> _csvToDbFieldMapping = new Dictionary<string, string>
        {
            { "Vorname", "FirstName" },
            { "Name", "LastName" },
            { "Firma", "Company" },
            { "Strasse", "Street" },
            { "Hausnummer", "HouseNumber" },
            { "Postleitzahl", "PostalCode" },
            { "Ortschaft", "City" }
        };

        public ImportCSVPage(AppDbContext context)
        {
            InitializeComponent();
            _context = context;
            _columnMappings = new Dictionary<string, string>();
            _isValidated = false;
            ClearUnsavedChanges(); // Initialize as having no unsaved changes
        }

        public void ResetUnsavedChanges()
        {
            HasUnsavedChanges = false;
        }

        public bool ConfirmUnsavedChanges()
        {
            if (HasUnsavedChanges)
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
            HasUnsavedChanges = true;
        }

        private void ClearUnsavedChanges()
        {
            HasUnsavedChanges = false;
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
                FileNameTextBlock.Text = $"Selected File: {System.IO.Path.GetFileName(csvFilePath)}";

                // Load CSV data with validation
                var (dataTable, inconsistentRows) = CSVHelper.LoadCSVWithValidation(csvFilePath, detectSeparators: true);

                _dataTable = dataTable;
                _columnMappings.Clear();
                _isValidated = false;

                // Populate DataGrid
                GenerateDataGridColumns();

                // Optionally log inconsistent rows
                if (inconsistentRows.Any())
                {
                    MessageBox.Show(
                        "The following rows were inconsistent and adjusted:\n" +
                        string.Join("\n", inconsistentRows.Take(10)) + // Show only the first 10 rows for brevity
                        (inconsistentRows.Count > 10 ? $"\n...and {inconsistentRows.Count - 10} more." : ""),
                        "Inconsistent Rows",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }

                // Update the UI
                ValidateButton.Visibility = Visibility.Visible;
                CancelButton.Visibility = Visibility.Visible;
                SaveToDatabaseButton.Visibility = Visibility.Collapsed;

                MarkAsUnsaved();
            }
        }

        private void GenerateDataGridColumns()
        {
            if (_dataTable == null) return;

            CSVDataGrid.Columns.Clear();
            CSVDataGrid.CanUserSortColumns = false;

            foreach (DataColumn column in _dataTable.Columns)
            {
                var comboBox = new ComboBox
                {
                    ItemsSource = _csvToDbFieldMapping.Values.ToList(),
                    SelectedValue = GetMappedDatabaseField(column.ColumnName),
                    Style = (Style)Application.Current.Resources["ModernComboBoxStyle"],
                    Width = 150,
                    IsEnabled = !_isValidated
                };

                if (comboBox.SelectedValue != null)
                {
                    _columnMappings[column.ColumnName] = comboBox.SelectedValue.ToString();
                }

                comboBox.SelectionChanged += (s, e) =>
                {
                    if (comboBox.SelectedValue != null)
                    {
                        _columnMappings[column.ColumnName] = comboBox.SelectedValue.ToString();
                        MarkAsUnsaved();
                    }
                };

                var dataGridColumn = new DataGridTextColumn
                {
                    Header = new StackPanel
                    {
                        Orientation = Orientation.Vertical,
                        Children =
                {
                    new TextBlock { Text = column.ColumnName, Foreground = Brushes.White },
                    comboBox
                }
                    },
                    Binding = new System.Windows.Data.Binding($"[{column.ColumnName}]")
                };

                CSVDataGrid.Columns.Add(dataGridColumn);
            }

            CSVDataGrid.ItemsSource = _dataTable.DefaultView;
        }

        private string GetMappedDatabaseField(string csvColumnName)
        {
            // Check if the CSV column name exists in the dictionary (case-insensitive)
            var mappedField = _csvToDbFieldMapping.FirstOrDefault(kvp => string.Equals(kvp.Key, csvColumnName, StringComparison.OrdinalIgnoreCase));

            // If a match is found, add it to the column mappings and return the mapped value
            if (!string.IsNullOrEmpty(mappedField.Value))
            {
                // Add to column mappings (ensuring it's automatically mapped)
                if (!_columnMappings.ContainsKey(csvColumnName))
                {
                    _columnMappings[csvColumnName] = mappedField.Value;
                }
                return mappedField.Value;
            }

            // Return null if no mapping found
            return null;
        }

        private List<(int rowIndex, string columnName, string error)> ValidateCSVContent()
        {
            var errors = new List<(int rowIndex, string columnName, string error)>();

            for (int rowIndex = 0; rowIndex < _dataTable.Rows.Count; rowIndex++)
            {
                var row = _dataTable.Rows[rowIndex];

                // Retrieve values from the row
                string firstName = row[_columnMappings.FirstOrDefault(m => m.Value == "FirstName").Key]?.ToString()?.Trim();
                string lastName = row[_columnMappings.FirstOrDefault(m => m.Value == "LastName").Key]?.ToString()?.Trim();
                string company = row[_columnMappings.FirstOrDefault(m => m.Value == "Company").Key]?.ToString()?.Trim();
                string street = row[_columnMappings.FirstOrDefault(m => m.Value == "Street").Key]?.ToString()?.Trim();
                string houseNumber = row[_columnMappings.FirstOrDefault(m => m.Value == "HouseNumber").Key]?.ToString()?.Trim();
                string postalCode = row[_columnMappings.FirstOrDefault(m => m.Value == "PostalCode").Key]?.ToString()?.Trim();

                // Validate FirstName, LastName, and Company
                bool hasFirstName = !string.IsNullOrWhiteSpace(firstName);
                bool hasLastName = !string.IsNullOrWhiteSpace(lastName);
                bool hasCompany = !string.IsNullOrWhiteSpace(company);

                if (!hasLastName && !hasCompany)
                {
                    if (hasFirstName)
                    {
                        // Highlight LastName and Company if only FirstName is provided
                        errors.Add((rowIndex, "LastName", "LastName is required if only FirstName is set."));
                        errors.Add((rowIndex, "Company", "Company is required if only FirstName is set."));
                    }
                    else if (!hasFirstName)
                    {
                        // Highlight all if none are provided
                        errors.Add((rowIndex, "FirstName", "At least LastName or Company is required."));
                        errors.Add((rowIndex, "LastName", "At least LastName or Company is required."));
                        errors.Add((rowIndex, "Company", "At least LastName or Company is required."));
                    }
                }

                // Validate Street (Address)
                if (string.IsNullOrWhiteSpace(street))
                {
                    errors.Add((rowIndex, "Street", "Street is required."));
                }

                // Validate HouseNumber
                if (!string.IsNullOrWhiteSpace(houseNumber) && !int.TryParse(houseNumber, out _))
                {
                    errors.Add((rowIndex, "HouseNumber", "HouseNumber must be a valid number or left empty."));
                }

                // Validate PostalCode
                if (string.IsNullOrWhiteSpace(postalCode) || !int.TryParse(postalCode, out _) || postalCode.Length != 4)
                {
                    errors.Add((rowIndex, "PostalCode", "Invalid Postal Code (must be a 4-digit number)."));
                }
            }

            return errors;
        }
        private void HighlightFaultyCells(List<(int rowIndex, string columnName, string error)> errors)
        {
            foreach (var error in errors)
            {
                var rowIndex = error.rowIndex;
                var columnName = error.columnName;

                var column = CSVDataGrid.Columns
                    .FirstOrDefault(c => c.Header is StackPanel stackPanel &&
                                         stackPanel.Children.OfType<ComboBox>().Any(cb => cb.SelectedValue?.ToString() == columnName));

                if (column != null)
                {
                    var row = CSVDataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;
                    if (row != null)
                    {
                        var cell = column.GetCell(row);
                        if (cell != null)
                        {
                            cell.Background = Brushes.Red; // Highlight the faulty cell
                        }
                    }
                }
            }
        }

        private void ValidateCSV_Click(object sender, RoutedEventArgs e)
        {
            // Check if all required fields are mapped
            var requiredFields = new List<string> { "FirstName", "LastName", "Company", "Street", "HouseNumber", "PostalCode", "City" };
            foreach (var field in requiredFields)
            {
                if (!_columnMappings.ContainsValue(field))
                {
                    MessageBox.Show($"Please map the required field: {field}.",
                                    "Mapping Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return; // Stop validation if a required field is not mapped
                }
            }

            // Perform CSV content validation
            var errors = ValidateCSVContent();

            // Highlight faulty cells with the list of errors
            HighlightFaultyCells(errors);

            if (errors.Any())
            {
                string errorMessage = "The following issues were found in the CSV:\n\n" +
                                      string.Join("\n", errors.Select(e => $"Row {e.rowIndex + 1}, Column '{e.columnName}': {e.error}"));
                MessageBox.Show(errorMessage, "Validation Errors", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // Stop validation if there are errors
            }

            // If validation is successful
            ValidateButton.Visibility = Visibility.Collapsed;
            _isValidated = true;
            GenerateDataGridColumns(); // Regenerate columns with ComboBoxes disabled

            MessageBox.Show("Validation successful. You can now save to the database.",
                             "Validation Successful", MessageBoxButton.OK, MessageBoxImage.Information);

            SaveToDatabaseButton.Visibility = Visibility.Visible;
        }



        private void SaveToDatabase_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var duplicates = new List<(DataRow row, Address existingAddress)>();
                var rowsToAdd = new List<Address>();
                int savedRowsCount = 0; // Counter for successfully saved rows
                bool skipAllDuplicates = false;
                bool overwriteAllDuplicates = false;

                // Pre-check all rows for duplicates
                foreach (DataRow row in _dataTable.Rows)
                {
                    var address = new Address
                    {
                        FirstName = row[_columnMappings.FirstOrDefault(m => m.Value == "FirstName").Key]?.ToString(),
                        LastName = row[_columnMappings.FirstOrDefault(m => m.Value == "LastName").Key]?.ToString(),
                        Company = row[_columnMappings.FirstOrDefault(m => m.Value == "Company").Key]?.ToString(),
                        Street = row[_columnMappings.FirstOrDefault(m => m.Value == "Street").Key]?.ToString(),
                        HouseNumber = row[_columnMappings.FirstOrDefault(m => m.Value == "HouseNumber").Key]?.ToString(),
                    };

                    string postalCode = row[_columnMappings.FirstOrDefault(m => m.Value == "PostalCode").Key]?.ToString();
                    string city = row[_columnMappings.FirstOrDefault(m => m.Value == "City").Key]?.ToString();

                    if (string.IsNullOrEmpty(postalCode) || string.IsNullOrEmpty(city))
                    {
                        MessageBox.Show("Postal code and city cannot be empty. Please fix the CSV.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var existingAddress = _context.Addresses
                        .FirstOrDefault(a => a.FirstName == address.FirstName &&
                                             a.LastName == address.LastName &&
                                             a.Company == address.Company &&
                                             a.Street == address.Street &&
                                             a.HouseNumber == address.HouseNumber &&
                                             a.Location.PostalCode == postalCode &&
                                             a.Location.City == city);

                    if (existingAddress != null)
                    {
                        // Collect duplicates for later decision
                        duplicates.Add((row, existingAddress));
                    }
                    else
                    {
                        // Prepare to add the row if not a duplicate
                        var location = GetOrCreateLocation(postalCode, city);
                        if (location == null)
                        {
                            MessageBox.Show("The address could not be saved due to location issues. Please fix the CSV.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        address.Location = location;
                        rowsToAdd.Add(address);
                    }
                }

                // Handle duplicates
                if (duplicates.Any())
                {
                    var result = MessageBox.Show(
                        $"{duplicates.Count} duplicate rows found. Do you want to save these anyway?\n\n" +
                        "Click 'Yes' to save all duplicates.\nClick 'No' to skip all duplicates.\nClick 'Cancel' to review the duplicates.",
                        "Duplicates Found",
                        MessageBoxButton.YesNoCancel,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Cancel)
                    {
                        return; // Exit saving process
                    }
                    else if (result == MessageBoxResult.No)
                    {
                        skipAllDuplicates = true;
                    }
                    else if (result == MessageBoxResult.Yes)
                    {
                        overwriteAllDuplicates = true;
                    }
                }

                // Process duplicates based on user choice
                if (overwriteAllDuplicates)
                {
                    foreach (var (row, existingAddress) in duplicates)
                    {
                        _context.Addresses.Remove(existingAddress);

                        var address = new Address
                        {
                            FirstName = row[_columnMappings.FirstOrDefault(m => m.Value == "FirstName").Key]?.ToString(),
                            LastName = row[_columnMappings.FirstOrDefault(m => m.Value == "LastName").Key]?.ToString(),
                            Company = row[_columnMappings.FirstOrDefault(m => m.Value == "Company").Key]?.ToString(),
                            Street = row[_columnMappings.FirstOrDefault(m => m.Value == "Street").Key]?.ToString(),
                            HouseNumber = row[_columnMappings.FirstOrDefault(m => m.Value == "HouseNumber").Key]?.ToString(),
                        };

                        string postalCode = row[_columnMappings.FirstOrDefault(m => m.Value == "PostalCode").Key]?.ToString();
                        string city = row[_columnMappings.FirstOrDefault(m => m.Value == "City").Key]?.ToString();

                        var location = GetOrCreateLocation(postalCode, city);
                        if (location == null)
                        {
                            MessageBox.Show("The address could not be saved due to location issues. Please fix the CSV.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        address.Location = location;
                        rowsToAdd.Add(address);
                    }
                }

                // Add non-duplicate rows
                foreach (var address in rowsToAdd)
                {
                    _context.Addresses.Add(address);
                    savedRowsCount++;
                }

                // Save all changes to the database
                _context.SaveChanges();

                // Show success message with the number of saved rows
                MessageBox.Show($"{savedRowsCount} rows saved to the database successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                ClearUnsavedChanges(); // Clear unsaved changes
            }
            catch (Exception ex)
            {
                // Display full exception information
                MessageBox.Show($"Error saving data: {ex.Message}\n\nInner Exception: {ex.InnerException?.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private Location GetOrCreateLocation(string postalCode, string city)
        {
            // Convert postalCode and city to lowercase for case-insensitive comparison
            var existingLocation = _context.Locations
                .FirstOrDefault(l => l.PostalCode.ToLower() == postalCode.ToLower());

            if (existingLocation != null)
            {
                // If the postal code exists but the city name is different, prompt the user
                if (!existingLocation.City.Equals(city, StringComparison.OrdinalIgnoreCase))
                {
                    var result = MessageBox.Show(
    $"The postal code '{postalCode}' already exists with the city name '{existingLocation.City}'. " +
    $"The new city name is '{city}'.\n\n" +
    "Yes: Keep the existing city name.\n" +
    "No: Update to the new city name.\n" +
    "Cancel: Abort the operation.",
    "Postal Code Conflict",
    MessageBoxButton.YesNoCancel,
    MessageBoxImage.Question);


                    if (result == MessageBoxResult.Yes)
                    {
                        // Keep the existing city (do not update)
                        return existingLocation;
                    }
                    else if (result == MessageBoxResult.No)
                    {
                        // Update the city to the new one
                        existingLocation.City = city;
                        _context.SaveChanges(); // Save the updated city name
                        return existingLocation;
                    }
                    else
                    {
                        // User canceled the action
                        return null;
                    }
                }
                else
                {
                    // If the postal code and city both match, return the existing location
                    return existingLocation;
                }
            }
            else
            {
                // Create a new location if postal code doesn't exist
                var newLocation = new Location { PostalCode = postalCode, City = city };
                _context.Locations.Add(newLocation);
                _context.SaveChanges(); // Save the new location to the database
                return newLocation;
            }
        }

        private void CancelCSV_Click(object sender, RoutedEventArgs e)
        {
            if (ConfirmUnsavedChanges())
            {
                _dataTable = null;
                CSVDataGrid.Columns.Clear();
                CSVDataGrid.ItemsSource = null;
                FileNameTextBlock.Text = string.Empty;
                ValidateButton.Visibility = Visibility.Collapsed;
                SaveToDatabaseButton.Visibility = Visibility.Collapsed;
                CancelButton.Visibility = Visibility.Collapsed;
                _isValidated = false;
                ClearUnsavedChanges();
            }
        }
    }
}