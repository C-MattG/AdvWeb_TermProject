using HighlandTechSolutions.Data;
using HighlandTechSolutions.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HighlandTechSolutions
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure DB and Identity
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login";
            });

            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            var app = builder.Build();

            // Seed roles, users, and services
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                await SeedRolesAsync(services);
                await SeedUsersAsync(services);
                await SeedServicesAsync(services);
            }


            // Configure middleware
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }

        // Seed Roles
        private static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            string[] roles = { "Business", "Customer" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        private static async Task SeedUsersAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            //Seed a business user
            var adminEmail = "admin@highlandtech.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    Name = "Jeff Roach",
                    ContactEmail = adminEmail,
                    PhoneNumber = "423-579-4217",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(adminUser, "Graduation2025!");
                await userManager.AddToRoleAsync(adminUser, "Business");
            }

            //Seed 3 customers
            var customers = new List<(string Name, string Email, string Phone)>
            {
                ("John Public", "john.public@mail.com", "555-000-0001"),
                ("Beatrice Bellow", "beatrice.bellow@mail.com", "555-000-0002"),
                ("Alex Green", "alex.green@mail.com", "555-000-0003")
            };

            foreach (var (name, email, phone) in customers)
            {
                var user = await userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        Name = name,
                        ContactEmail = email,
                        PhoneNumber = phone,
                        EmailConfirmed = true
                    };
                    await userManager.CreateAsync(user, "GodspeedGoBucs2025!");
                    await userManager.AddToRoleAsync(user, "Customer");
                }
            }
        }

        //Seed services
        private static async Task SeedServicesAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var servicesToSeed = new List<Service>
            {
                new Service
                {
                    Name = "Diagnostic Service",
                    Description = "Comprehensive troubleshooting to identify issues with a personal computer.",
                    Price = 40m
                },
                new Service
                {
                    Name = "Performance Optimization",
                    Description = "Enhancing the speed and performance of device by optimizing software utilization.",
                    Price = 25m
                },
                new Service
                {
                    Name = "Software Installation",
                    Description = "Installation of software. (1 unit)",
                    Price = 35m
                },
                new Service
                {   
                    Name = "Virus & Malware Removal",
                    Description = "Clean up and protect your device from malicious software. Includes free antivirus software installation.",
                    Price = 50m
                },
                new Service
                {
                    Name = "Scam Clean-up",
                    Description = "Remove access to computer from outside/malicious sources after an attempted scam.",
                    Price = 20m
                },
                new Service
                {
                    Name = "Hardware Component Replacement",
                    Description = "Replace non-functioning component with new component.",
                    Price = 80m
                },
                new Service
                {
                    Name = "Basic Data Recovery",
                    Description = "Recover data from a drive.",
                    Price = 50m
                },
                new Service
                {
                    Name = "Advanced Data Recovery",
                    Description = "Use digital forensics tools to recover deleted or corrupted data.",
                    Price = 240m
                },
                new Service
                {
                    Name = "Laptop Screen Replacement",
                    Description = "Repair or replace laptop screen.",
                    Price = 120m
                },
                new Service
                {
                    Name = "Smart Home Device Setup",
                    Description = "Installation and configuration of smart home devices (cameras, smart lighting, electronic locks). Pricing may vary.",
                    Price = 50m
                },
                new Service
                {
                    Name = "Computer Build",
                    Description = "Build a custom desktop computer from pre-purchased parts. Pricing may vary.",
                    Price = 200m
                },
                new Service
                {
                    Name = "Brief Consultation",
                    Description = "Call now! 423-579-4217",
                    Price = 0m
                }
            };

            foreach (var service in servicesToSeed)
            {
                bool exists = await context.Services.AnyAsync(s => s.Name == service.Name);
                if (!exists)
                {
                    context.Services.Add(service);
                }
            }

            await context.SaveChangesAsync();
        }

    }
}
