using System.Linq;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Sessions
{
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _dbContext;

        public CustomerService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Customer Login(string email, string password)
        {
            return _dbContext.Customer.FirstOrDefault(x => x.Email == email && x.Password == password);
        }
    }
}