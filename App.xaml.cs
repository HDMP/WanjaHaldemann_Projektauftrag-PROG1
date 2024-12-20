using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SwissAddressManager.Data.DatabaseContext;
using SwissAddressManager.WPF.Views;
using System;
using System.Windows;

namespace SwissAddressManager
{
    public partial class App : Application
    {
        private IServiceProvider _serviceProvider;

        public App()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            var connectionString = "Server=WIV11-VMWP;Database=SwissAddresses;Trusted_Connection=True;TrustServerCertificate=True;";
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Register MainWindow with DI
            services.AddTransient<MainWindow>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Resolve MainWindow from the service provider
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }
}
