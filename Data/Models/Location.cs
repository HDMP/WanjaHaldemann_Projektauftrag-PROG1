namespace SwissAddressManager.Data.Models
{
    public class Location
    {
        public int Id { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }

        // Navigation property for related addresses
        public ICollection<Address> Addresses { get; set; }
    }
}
