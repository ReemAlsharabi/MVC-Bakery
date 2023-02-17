using MessagePack;
using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations.Schema;
namespace WebApplication1.Models
{
    public class Product
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageURL { get; set; }
        [NotMapped]
        public IFormFile Image { get; set; }
        public bool Available { get; set; }
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public string ?CategoryName;
    }
}
