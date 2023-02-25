using System.Data.Entity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Sessions;
using System.Data.Entity.Infrastructure;
using Microsoft.AspNetCore.Authorization;

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
        //[Authorize]
        public async Task<IActionResult> Index()
        {
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
        
        //[Authorize]
        public IActionResult Welcome()
        {
            ViewBag.Message = GetCustomerProfile().Name;
            return View();
        }
        //[Authorize]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("email");
            return RedirectToAction("Index", "Home");
        }
        // GET: Customers/Details/5
        //[Authorize]
        public IActionResult Details(int? id)
        {
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
        public async Task<IActionResult> Create([Bind("Id,Name,Email,Password,Address,PhoneNumber,IsAdmin")] Customer customer)
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
        //[Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
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
        //[Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,Password,Address,PhoneNumber")] Customer customer)
        {
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
        //[Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
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
        //[Authorize]
        public Customer GetCustomerProfile()
        {
            // Retrieve the user's profile data from the database
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