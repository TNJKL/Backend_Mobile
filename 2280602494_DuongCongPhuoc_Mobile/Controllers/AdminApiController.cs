using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _2280602494_DuongCongPhuoc_Mobile.Models;
using System.Text.Json;

namespace _2280602494_DuongCongPhuoc_Mobile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly string _historyFilePath;

        public AdminApiController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _historyFilePath = Path.Combine(env.ContentRootPath, "payment_history.json");
        }

        [HttpGet("DashboardStats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            try
            {
                // 1. Total Events
                var totalEvents = await _context.Events.CountAsync();

                // 2. Total Users (Customer base)
                var totalUsers = await _context.Users.CountAsync();

                // 3. Upcoming Events (Next 30 Days)
                var upcomingEvents = await _context.Events
                    .Where(e => e.StartTime >= DateTime.Now && e.StartTime <= DateTime.Now.AddDays(30))
                    .CountAsync();

                // 4. Pending Approvals
                var pendingApprovals = await _context.EventVendors.CountAsync(ev => ev.Status == "Pending");

                // 5. Total Vendors
                var totalVendors = await _context.Vendors.CountAsync();

                // 3. Revenue from payment_history.json
                double totalRevenue = 0;
                if (System.IO.File.Exists(_historyFilePath))
                {
                    var json = await System.IO.File.ReadAllTextAsync(_historyFilePath);
                    var transactions = JsonSerializer.Deserialize<List<PaymentTransaction>>(json);
                    if (transactions != null)
                    {
                        totalRevenue = transactions
                            .Where(t => t.Status == "Success")
                            .Sum(t => t.Amount);
                    }
                }

                return Ok(new
                {
                    totalEvents,
                    totalUsers,
                    upcomingEvents,
                    pendingApprovals,
                    totalVendors,
                    totalRevenue
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error getting dashboard stats: {ex.Message}");
            }
        }
    }
}
