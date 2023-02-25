using MessagePack;
using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Data;

namespace WebApplication1.Models
{
    public class UniqueEmailAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var customer = (Customer)validationContext.ObjectInstance;

            // Check if the email is unique
            using (var dbContext = new ApplicationDbContext())
            {
                if (dbContext.Customer.Any(c => c.Email == customer.Email && c.Id != customer.Id))
                {
                    return new ValidationResult("Email already exists.");
                }
            }

            return ValidationResult.Success;
        }
    }
    public class Customer : IdentityUser
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int Id { get; set; }
        [Microsoft.Build.Framework.Required]
        public string Name { get; set; }
        [Microsoft.Build.Framework.Required, EmailAddress, UniqueEmail]
        public string Email { get; set; }
        [Microsoft.Build.Framework.Required]
        public string Password { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        // create the admin account before initializing IsAdmin to false
        public bool IsAdmin { get; set; } = false;
        public List<Order> Orders { get; set; } = new List<Order>();
        /*
        public int ConcurrencyStamp = 0;
        public int LockoutEnd = 0;
        public string NormalizedEmail = " ";
        public string NormalizedUserName = " ";
        public string PasswordHash = " ";
        public string UserName = " ";
        */
    }
}
