using System.ComponentModel.DataAnnotations;

namespace TokenGenerator.Models
{
    /// <summary>
    /// The Customer class models an application customer
    /// </summary>
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }
    }
}
