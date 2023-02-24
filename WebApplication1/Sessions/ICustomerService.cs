using WebApplication1.Models;

namespace WebApplication1.Sessions
{
    public interface ICustomerService
    {
        Customer Login(string email, string password);
    }
}