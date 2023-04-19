using System.ComponentModel.DataAnnotations;

namespace IntuitionApp.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        [Required]
        public decimal Price { get; set; }
    }
}
