using MessagePack;
using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class Order
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int Id { get; set; }
        [Required]
        public DateTime OrderDateTime { get; set; } = DateTime.Now;
        public string DeliveryAddress { get; set; }
        public decimal TotalPrice { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        public OrderStatus Status { get; set; }

        [ForeignKey("CustomerID")]
        public virtual Customer Customer { get; set; }
    }
}
