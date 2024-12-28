using System.Linq;
using System.Timers;
using Microsoft.EntityFrameworkCore;
using SwissAddressManager.Data.DatabaseContext;

namespace SwissAddressManager.WPF.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        private string _currentTime;

        public int TotalAddresses { get; set; }
        public int TotalLocations { get; set; }
        public string MostCommonCity { get; set; }
        public int MostCommonCityCount { get; set; }

        public string CurrentTime
        {
            get => _currentTime;
            set
            {
                _currentTime = value;
                OnPropertyChanged();
            }
        }

        public DashboardViewModel(AppDbContext context)
        {
            TotalAddresses = context.Addresses.Count();
            TotalLocations = context.Locations.Count();

            var addressList = context.Addresses
    .Include(a => a.Location) // Ensure Location is eagerly loaded
    .Where(a => a.Location != null && a.Location.City != null) // Exclude null values
    .ToList(); // Materialize data into memory

            foreach (var address in addressList)
            {
                if (address.Location == null)
                {
                    throw new System.Exception($"Address with ID {address.Id} has a null Location.");
                }

                if (address.Location.City == null)
                {
                    throw new System.Exception($"Address with ID {address.Id} has a Location with a null City.");
                }
            }

            var commonCity = addressList
                .GroupBy(a => a.Location.City)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault();

            if (commonCity != null)
            {
                MostCommonCity = commonCity.Key;
                MostCommonCityCount = commonCity.Count();
            }
            else
            {
                MostCommonCity = "N/A";
                MostCommonCityCount = 0;
            }




            // Initialize live time updater
            InitializeTimeUpdater();
        }

        private void InitializeTimeUpdater()
        {
            var timer = new System.Timers.Timer(1000); // Specify System.Timers.Timer
            timer.Elapsed += (sender, e) =>
            {
                CurrentTime = System.DateTime.Now.ToString("HH:mm:ss");
            };
            timer.Start();
        }

    }
}
