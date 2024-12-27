using System.ComponentModel.DataAnnotations;

namespace SwissAddressManager.Data.Models
{
    public class Location
    {
        [Key]
        public int Id { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
    }
}
