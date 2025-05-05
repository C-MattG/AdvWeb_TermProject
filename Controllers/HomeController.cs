using HighlandTechSolutions.Data;
using HighlandTechSolutions.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace HighlandTechSolutions.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        // Constructor with ApplicationDbContext
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        //Dashboard action method that fetches entity counts
        public IActionResult Dashboard()
        {
            // Fetch counts for the entities
            var dashboardData = new DashboardViewModel
            {
                TotalServices = _context.Services.Count(),
                TotalQuotes = _context.Quotes.Count(),
                TotalAppointments = _context.Appointments.Count(),
            };

            return View(dashboardData);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}


