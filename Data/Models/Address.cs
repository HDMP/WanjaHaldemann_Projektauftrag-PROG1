using System.ComponentModel.DataAnnotations.Schema;

namespace SwissAddressManager.Data.Models
{
    public class Address
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string Street { get; set; }
        public string HouseNumber { get; set; }

        // This matches the database column name
        public int PostalCodeId { get; set; }

        [ForeignKey("PostalCodeId")]
        public Location Location { get; set; }
    }
}
