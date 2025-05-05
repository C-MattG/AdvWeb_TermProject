using HighlandTechSolutions.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


//I tried for hours and can't get my review page working
//Scrapping functionality
public class ReviewsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ReviewsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Reviews
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var reviews = await _context.Reviews
            .Include(r => r.User) //include user info
            .ToListAsync();

        return View(reviews); //list of reviews to the view
    }
}
