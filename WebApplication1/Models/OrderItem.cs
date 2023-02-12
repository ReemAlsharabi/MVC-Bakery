using MessagePack;
using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
	public class OrderItem
	{
        [System.ComponentModel.DataAnnotations.Key]
        public int Id { get; set; }
		[Required]
		[ForeignKey("OrderId")]
		public int OrderId { get; set; }
		[ForeignKey("ProductId")]
		public int ProductId { get; set; }
		public int Quantity { get; set; }
		public decimal Price { get; set; }
	}
}
