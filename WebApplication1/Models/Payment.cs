using MessagePack;
using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class Payment
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int Id { get; set; }
        [Required]
        public string PaymentMethod { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public DateTime Date { get; set; } = DateTime.Now;

        [ForeignKey("Order")]
        public int OrderId { get; set; }
        [Required]
        public virtual Order Order { get; set; }
    }
}
