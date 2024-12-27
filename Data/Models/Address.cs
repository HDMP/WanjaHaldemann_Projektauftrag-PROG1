using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SwissAddressManager.Data.Models
{
    public class Address
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Street { get; set; }
        public string HouseNumber { get; set; }

        // Foreign Key to Locations
        [ForeignKey("Location")]
        public int? PostalCodeID { get; set; } // Matches the database column name
        public virtual Location Location { get; set; } // Navigation property
    }
}
