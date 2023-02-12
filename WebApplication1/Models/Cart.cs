using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
	public class Cart
	{
		[System.ComponentModel.DataAnnotations.Key]
		public int Id { get; set; }
		[Required]
		public int Quantity { get; set; }
		[ForeignKey("CustomerId")]
		public int CustomerId { get; set; }
		[ForeignKey("ProductId")]
		public int ProductId { get; set; }
	}
}
