using System.ComponentModel.DataAnnotations;

namespace Bombardiro_Cardealo.Models
{
    public class Car
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string PriceRange { get; set; }

        public string ImageFileName { get; set; } 
    }
}
