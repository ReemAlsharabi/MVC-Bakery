namespace WebApplication1.Models
{
    public class AccessManager
    {
        public static bool IsAdmin(Customer customer)
        {
            return customer.IsAdmin;
        }
    }
}
