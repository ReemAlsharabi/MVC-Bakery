using System.Data.Entity;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Sessions;
using System.Data.Entity.Infrastructure;

namespace WebApplication1.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICustomerService _customerService;
        public CustomersController(ApplicationDbContext context, ICustomerService customerService)
        {
            _context = context;
            _customerService = customerService;
        }
        // GET: Customers
        public async Task<IActionResult> Index()
        {
            if (!Loggedin())
                return RedirectToAction("Index", "Home");
            /*
            if (GetCustomerProfile().IsAdmin)
            {
                var customers = await _context.Customer.AsQueryable().ToListAsync();
                return View(customers);
            }
            */
            // Redirect the customer to their own profile details
            int? customerId = GetCustomerProfile().Id;
            if (customerId == null)
                return RedirectToAction("Index", "Home");
            return RedirectToAction("Details", "Customers", new { id = customerId });
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var customer = _customerService.Login(email, password);
            if (customer != null)
            {
                // Login successful
                HttpContext.Session.SetString("email", email);
                return RedirectToAction("Welcome", "Customers");
            }
            else
            {
                // Login failed
                ModelState.AddModelError("", "Invalid email or password");
                ViewBag.Message = "Invalid Login";
                return View();
            }
        }
        public IActionResult Welcome()
        {
            if (Loggedin())
            {
                ViewBag.Message = GetCustomerProfile().Name;
                return View();
            }
            return RedirectToAction("Index", "Home");

        }
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("email");
            return RedirectToAction("Index", "Home");
        }
        // GET: Customers/Details/5
        public IActionResult Details(int? id)
        {
            if (!Loggedin())
                return RedirectToAction("Index", "Home");

            if (id == null || _context.Customer == null)
            {
                return NotFound();
            }

            var customer = _context.Customer
                .FirstOrDefault(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }
        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }
        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,Password,Address,PhoneNumber")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }
        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!Loggedin())
                return RedirectToAction("Index", "Home");
            if (id == null || _context.Customer == null)
            {
                return NotFound();
            }

            var customer = await _context.Customer.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }
        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,Password,Address,PhoneNumber")] Customer customer)
        {
            if (!Loggedin())
                return RedirectToAction("Index", "Home");
            if (id != customer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }
        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!Loggedin())
                return RedirectToAction("Index", "Home");
            if (id == null || _context.Customer == null)
            {
                return NotFound();
            }

            var customer = await _context.Customer
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }
        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Customer == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Customer'  is null.");
            }
            var customer = await _context.Customer.FindAsync(id);
            if (customer != null)
            {
                _context.Customer.Remove(customer);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public bool Loggedin()
        {
            string email = HttpContext.Session.GetString("email");
            if (email != null)
                return true;
            return false;
        }
        public Customer GetCustomerProfile()
        {
            // Retrieve the user's profile data from the database
            if (!Loggedin())
                return null;
            Customer customer = _context.Customer.FirstOrDefault(c => c.Email == HttpContext.Session.GetString("email"));
            Customer customerProfile = new Customer
            {
                Name = customer.Name,
                Id = customer.Id,
                Address = customer.Address,
                IsAdmin = customer.IsAdmin
            };
            return customerProfile;
        }
        private bool CustomerExists(int id)
        {
          return _context.Customer.Any(e => e.Id == id);
        }
    }
}
