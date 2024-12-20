using System.Windows;
using System.Windows.Controls;

namespace SwissAddressManager.WPF.Views
{
    public partial class SettingsPage : UserControl
    {
        private string InitialConnectionString = "Server=WIV11-VMWP;Database=SwissAddresses;Trusted_Connection=True;TrustServerCertificate=True;";
        private bool IsEditing = false;

        public SettingsPage()
        {
            InitializeComponent();
            LoadInitialConnectionString();
        }

        private void LoadInitialConnectionString()
        {
            // Parse the connection string into the textboxes
            ServerTextBox.Text = "WIV11-VMWP";
            DatabaseTextBox.Text = "SwissAddresses";
            UsernameTextBox.Text = ""; // Empty for Trusted Connection
            PasswordBox.Password = ""; // Empty for Trusted Connection
            WindowsAuthCheckBox.IsChecked = true; // Assume Windows Authentication
            UpdateFieldStates();
        }

        private void ConfigureButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleEditingMode(true);
        }

        private void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Saving the Settings is not permitted in the trial version of this application!", "Warning", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Revert to the initial state
            LoadInitialConnectionString();
            ToggleEditingMode(false);
        }

        private void ToggleEditingMode(bool isEditing)
        {
            IsEditing = isEditing;

            // Enable/Disable controls
            ServerTextBox.IsEnabled = isEditing;
            DatabaseTextBox.IsEnabled = isEditing;
            WindowsAuthCheckBox.IsEnabled = isEditing;

            // Update fields based on the Windows Auth checkbox state
            UpdateFieldStates();

            // Show/Hide buttons
            ConfigureButton.Visibility = isEditing ? Visibility.Collapsed : Visibility.Visible;
            SaveChangesButton.Visibility = isEditing ? Visibility.Visible : Visibility.Collapsed;
            CancelButton.Visibility = isEditing ? Visibility.Visible : Visibility.Collapsed;
        }

        private void WindowsAuthCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            UpdateFieldStates();
        }

        private void UpdateFieldStates()
        {
            // Disable Username and Password if Windows Auth is checked
            bool isWindowsAuth = WindowsAuthCheckBox.IsChecked ?? false;
            UsernameTextBox.IsEnabled = IsEditing && !isWindowsAuth;
            PasswordBox.IsEnabled = IsEditing && !isWindowsAuth;
        }
    }
}
