using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HighlandTechSolutions.Data;
using HighlandTechSolutions.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace HighlandTechSolutions.Controllers
{
    [Authorize]
    public class QuotesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public QuotesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Quotes (Business only)
        [Authorize(Roles = "Business")]
        public async Task<IActionResult> Index()
        {
            var quotes = await _context.Quotes
                .Include(q => q.User) // Include the User data to access User.Name
                .ToListAsync();
            return View(quotes); // Pass the list of quotes to the view
        }

        // GET: Quotes/Details/5
        [Authorize(Roles = "Business")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var quote = await _context.Quotes
                .Include(q => q.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (quote == null) return NotFound();

            return View(quote);
        }

        // GET: Quotes/Create (Customer only)
        [Authorize(Roles = "Customer")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Quotes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Create(QuoteFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized(); // fail gracefully
            }

            var quote = new Quote
            {
                Details = model.Details,
                UserId = user.Id,
                Status = "Pending",
                DateRequested = DateTime.UtcNow
            };

            _context.Add(quote);
            await _context.SaveChangesAsync();

            return RedirectToAction("QuoteSubmitted");
        }

        // GET: Quotes/QuoteSubmitted
        [Authorize(Roles = "Customer")]
        public IActionResult QuoteSubmitted()
        {
            return View();
        }

        // GET: Quotes/Edit/5
        [Authorize(Roles = "Business")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var quote = await _context.Quotes.FindAsync(id);
            if (quote == null) return NotFound();

            return View(quote);
        }

        // POST: Quotes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Business")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Status")] Quote quote)
        {
            if (id != quote.Id) return NotFound();

            var quoteToUpdate = await _context.Quotes.FindAsync(id);
            if (quoteToUpdate == null) return NotFound();

            quoteToUpdate.Status = quote.Status;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Quotes/Delete/5
        [Authorize(Roles = "Business")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var quote = await _context.Quotes
                .Include(q => q.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (quote == null) return NotFound();

            return View(quote);
        }

        // POST: Quotes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Business")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var quote = await _context.Quotes.FindAsync(id);
            _context.Quotes.Remove(quote);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
