using HighlandTechSolutions.Data;
using HighlandTechSolutions.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HighlandTechSolutions.Controllers
{
    [Authorize]
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AppointmentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Appointments (Business only)
        [Authorize(Roles = "Business")]
        public async Task<IActionResult> Index()
        {
            var appointments = await _context.Appointments
                .Include(a => a.User)
                .Include(a => a.AppointmentServices)
                    .ThenInclude(asvc => asvc.Service)
                .Where(a => a.IsDeleted == false)  // Only fetch appointments that are not deleted
                .ToListAsync();

            return View(appointments);
        }

        // GET: Appointments/Details/5
        [Authorize(Roles = "Business")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var appointment = await _context.Appointments
                .Include(a => a.User)
                .Include(a => a.AppointmentServices)
                    .ThenInclude(asvc => asvc.Service)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (appointment == null) return NotFound();

            return View(appointment);
        }

        // GET: Appointments/Create
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Create()
        {
            var services = await _context.Services.ToListAsync();
            var viewModel = new AppointmentFormViewModel
            {
                AvailableServices = services.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                }).ToList(),
                AvailableTimes = GenerateTimeSlots()
            };
            return View(viewModel);
        }

        // POST: Appointments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Create(AppointmentFormViewModel model)
        {
            model.Time = TimeSpan.Parse(Request.Form["Time"]);

            if (!ModelState.IsValid)
            {
                model.AvailableServices = await _context.Services
                    .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name })
                    .ToListAsync();
                model.AvailableTimes = GenerateTimeSlots();
                return View(model);
            }

            // Prevent weekend scheduling (from server-side)
            if (model.Date.DayOfWeek == DayOfWeek.Saturday || model.Date.DayOfWeek == DayOfWeek.Sunday)
            {
                ModelState.AddModelError("Date", "Appointments are only available Monday through Friday.");
            }

            var userId = _userManager.GetUserId(User);

            var appointment = new Appointment
            {
                UserId = userId,
                Date = model.Date,
                Time = model.Time,
                AppointmentServices = model.SelectedServiceIds.Select(sid => new AppointmentService
                {
                    ServiceId = sid
                }).ToList()
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Confirmation");
        }

        public IActionResult Confirmation()
        {
            return View();
        }

        // GET: Appointments/Edit/5
        [Authorize(Roles = "Business")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var appointment = await _context.Appointments
                .Include(a => a.AppointmentServices)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null) return NotFound();

            var viewModel = new AppointmentFormViewModel
            {
                Id = appointment.Id,
                Date = appointment.Date,
                Time = appointment.Time,
                SelectedServiceIds = appointment.AppointmentServices.Select(a => a.ServiceId).ToList(),
                AvailableServices = await _context.Services
                    .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name })
                    .ToListAsync(),
                AvailableTimes = GenerateTimeSlots()
            };

            return View(viewModel);
        }

        // POST: Appointments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Business")]
        public async Task<IActionResult> Edit(int id, AppointmentFormViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                model.AvailableServices = await _context.Services
                    .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name })
                    .ToListAsync();
                model.AvailableTimes = GenerateTimeSlots();
                return View(model);
            }

            var appointment = await _context.Appointments
                .Include(a => a.AppointmentServices)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null) return NotFound();

            appointment.Time = model.Time;

            // Update services
            appointment.AppointmentServices.Clear();
            foreach (var sid in model.SelectedServiceIds)
            {
                appointment.AppointmentServices.Add(new AppointmentService
                {
                    AppointmentId = appointment.Id,
                    ServiceId = sid
                });
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Appointments/Delete/5
        [Authorize(Roles = "Business")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var appointment = await _context.Appointments
                .Include(a => a.User)
                .Include(a => a.AppointmentServices)
                    .ThenInclude(a => a.Service)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (appointment == null) return NotFound();

            return View(appointment);
        }

        // POST: Appointments/Delete/5 (Business only)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Business")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                // Mark the appointment as deleted instead of deleting it from the DB
                appointment.IsDeleted = true;
                await _context.SaveChangesAsync();
            }

            // Redirect to the appropriate page (Index after deletion)
            return RedirectToAction("Index", "Appointments");
        }

        // GET: Appointments/History (Business only)
        [Authorize(Roles = "Business")]
        public async Task<IActionResult> History()
        {
            var appointments = await _context.Appointments
                .Include(a => a.User)
                .Include(a => a.AppointmentServices)
                    .ThenInclude(asvc => asvc.Service)
                .Where(a => a.IsDeleted == false || a.IsDeleted == true)  // Include both active and deleted 
                .OrderByDescending(a => a.Date)
                .ToListAsync();

            return View(appointments);
        }

        // Generate Time Slots from 10am to 6pm
        private List<SelectListItem> GenerateTimeSlots()
        {
            var slots = new List<SelectListItem>();
            for (int hour = 10; hour <= 18; hour++) // 10am to 6pm
            {
                var time = new TimeSpan(hour, 0, 0);
                slots.Add(new SelectListItem
                {
                    Value = time.ToString(),
                    Text = DateTime.Today.Add(time).ToString("h:mm tt")
                });
            }
            return slots;
        }

        // Get Available Times based on date selection (Excludes weekends)
        [HttpGet]
        public async Task<IActionResult> GetAvailableTimes(DateTime date)
        {
            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
            {
                return Json(new List<string>());
            }

            var takenTimes = await _context.Appointments
                .Where(a => a.Date.Date == date.Date)
                .Select(a => a.Time)
                .ToListAsync();

            var allSlots = new List<string>();
            for (int hour = 10; hour <= 18; hour++)
            {
                var slot = new TimeSpan(hour, 0, 0);
                if (!takenTimes.Contains(slot))
                {
                    allSlots.Add(slot.ToString(@"hh\:mm"));
                }
            }

            return Json(allSlots);
        }

        // Order Tracking for Customers
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> OrderTracking()
        {
            var userId = _userManager.GetUserId(User);

            var appointments = await _context.Appointments
                .Where(a => a.UserId == userId)
                .Include(a => a.AppointmentServices)
                    .ThenInclude(asvc => asvc.Service)
                .Select(a => new AppointmentTrackingViewModel
                {
                    Date = a.Date,
                    Time = a.Time,
                    Status = a.Status,
                    Services = a.AppointmentServices.Select(s => s.Service.Name).ToList()
                })
                .ToListAsync();

            return View(appointments);
        }
    }
}
