using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _2280602494_DuongCongPhuoc_Mobile.Models;
using System.Security.Claims;

namespace _2280602494_DuongCongPhuoc_Mobile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly Microsoft.AspNetCore.Identity.UserManager<User> _userManager;

        public ChatApiController(ApplicationDbContext context, Microsoft.AspNetCore.Identity.UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("GetSupportInfo")]
        public async Task<IActionResult> GetSupportInfo()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 1. Check if user already has a conversation with a Staff/Admin
            // Get the last person they messaged who is NOT them.
            // Simplified: Find the most recent distinct ReceiverId (if sender is me) or SenderId (if receiver is me)
            var lastInteraction = await _context.ChatMessages
                .Where(m => m.SenderId == currentUserId || m.ReceiverId == currentUserId)
                .OrderByDescending(m => m.Timestamp)
                .Select(m => m.SenderId == currentUserId ? m.ReceiverId : m.SenderId)
                .FirstOrDefaultAsync();

            if (!string.IsNullOrEmpty(lastInteraction))
            {
                // Verify this person is still Staff/Admin? 
                // For performance, assuming if they chatted, it's valid.
                var user = await _userManager.FindByIdAsync(lastInteraction);
                if (user != null)
                {
                    return Ok(new { supportId = user.Id, supportName = user.UserName });
                }
            }

            // 2. If no history, assign a Staff member (First available)
            var staffUsers = await _userManager.GetUsersInRoleAsync("Staff");
            var assignedStaff = staffUsers.FirstOrDefault();

            if (assignedStaff == null)
            {
                // Fallback to Admin
                var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");
                assignedStaff = adminUsers.FirstOrDefault();
            }

            if (assignedStaff != null)
            {
                return Ok(new { supportId = assignedStaff.Id, supportName = assignedStaff.UserName });
            }

            return NotFound("No support staff available.");
        }

        [HttpGet("History/{otherUserId}")]
        public async Task<IActionResult> GetHistory(string otherUserId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await _userManager.FindByIdAsync(currentUserId);
            bool isStaffOrAdmin = await _userManager.IsInRoleAsync(currentUser, "Staff") || await _userManager.IsInRoleAsync(currentUser, "Admin");

            List<ChatMessage> messages;

            if (isStaffOrAdmin)
            {
                // Staff/Admin can see ALL history of this specific customer (otherUserId)
                messages = await _context.ChatMessages
                    .Where(m => m.SenderId == otherUserId || m.ReceiverId == otherUserId)
                    .OrderBy(m => m.Timestamp)
                    .ToListAsync();
            }
            else
            {
                // Customer Mode: "Unified Support Thread"
                // Return ALL messages where I am Sender/Receiver.
                // We assume these are all Support chats. 
                // To be safe, we could filter for Staff/Admin counterparts, but for this app context it's likely safe to return all.
                // This ensures if Staff A and Staff B both chatted with me, I see it as one continuous thread.
                
                messages = await _context.ChatMessages
                    .Where(m => m.SenderId == currentUserId || m.ReceiverId == currentUserId)
                    .OrderBy(m => m.Timestamp)
                    .ToListAsync();
            }

            var result = messages.Select(m => new
            {
                m.Id,
                m.SenderId,
                m.ReceiverId,
                m.Content,
                m.Timestamp,
                m.IsRead
            });

            return Ok(result);
        }

        [HttpGet("Inbox")]
        public async Task<IActionResult> GetInbox()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await _userManager.FindByIdAsync(currentUserId);
            bool isStaffOrAdmin = await _userManager.IsInRoleAsync(currentUser, "Staff") || await _userManager.IsInRoleAsync(currentUser, "Admin");

            if (isStaffOrAdmin)
            {
                // Shared Inbox for Staff/Admin: View all conversations from Users
                // Strategy: Get all messages, find distinct users involved who are NOT Staff/Admin (optional, or just group by conversation)
                // Simpler: Group by the "Other" party. 
                // Since this is a "Support" view, we want to see unique Customers.
                
                // 1. Fetch all messages
                var allMessages = await _context.ChatMessages
                    .OrderByDescending(m => m.Timestamp)
                    .ToListAsync();

                // 2. Identify unique users involved (excluding known staff if we want, but simpler to just show everyone)
                //    We need to figure out who is the "Customer" in the conversation.
                //    Correction: In a shared inbox, we want to list separate conversations per Customer.
                //    So we group by whomever is related to the message that is NOT a staff? 
                //    Actually, simpler: Just show all distinct users that have ever sent/received a message.
                //    But we need to merge (Customer->StaffA) and (Customer->StaffB) into one entry "Customer".
                
                //    So we simply group by the User who is NOT the current viewer? 
                //    No, for Staff A, seeing Customer->StaffB, the "Other" is Customer.
                //    So we iterate messages and pick the ID that is NOT a Staff/Admin ID? 
                //    That requires checking roles for everyone. Expensive.
                
                //    Alternative: We assume the "Customer" is the one who initiated... no.
                //    Alternative: We group by BOTH Sender and Receiver, then distinct the set of IDs, then remove IDs that are Staff/Admin.
                
                //    Let's try a hybrid approach efficient enough for small scale:
                //    Get all unique UserIds in ChatMessages.
                var uniqueUserIds = allMessages.Select(m => m.SenderId).Union(allMessages.Select(m => m.ReceiverId)).Distinct().ToList();
                
                //    Filter out Staff/Admins from this list
                var customerIds = new List<string>();
                foreach(var uid in uniqueUserIds) {
                    var u = await _userManager.FindByIdAsync(uid);
                    if(u != null && !await _userManager.IsInRoleAsync(u, "Staff") && !await _userManager.IsInRoleAsync(u, "Admin")) {
                        customerIds.Add(uid);
                    }
                }

                //    Now for each CustomerId, get their latest message
                var conversations = new List<dynamic>();
                foreach(var custId in customerIds) {
                    var lastMsg = allMessages.FirstOrDefault(m => m.SenderId == custId || m.ReceiverId == custId);
                    if(lastMsg != null) {
                        conversations.Add(new { UserId = custId, LastMessage = lastMsg });
                    }
                }
                
                // Sort by timestamp
                conversations = conversations.OrderByDescending(c => c.LastMessage.Timestamp).ToList();

                // Map to result
                 var users = await _context.Users
                    .Where(u => customerIds.Contains(u.Id))
                    .Select(u => new { u.Id, FullName = u.UserName, u.UserName, u.Initials })
                    .ToDictionaryAsync(u => u.Id);

                var result = conversations.Select(c => new
                {
                    UserId = c.UserId,
                    UserName = users.ContainsKey(c.UserId) ? users[c.UserId].FullName : "Unknown",
                    LastMessage = c.LastMessage.Content,
                    Timestamp = c.LastMessage.Timestamp,
                    UnreadCount = 0 
                });

                return Ok(result);
            }
            else
            {
                // Logic cũ cho Customer (chỉ xem tin nhắn của mình)
                // 1. Fetch all messages involving current user
                var allMessages = await _context.ChatMessages
                    .Where(m => m.SenderId == currentUserId || m.ReceiverId == currentUserId)
                    .OrderByDescending(m => m.Timestamp)
                    .ToListAsync();

                // 2. Group in memory
                var conversations = allMessages
                    .GroupBy(m => m.SenderId == currentUserId ? m.ReceiverId : m.SenderId)
                    .Select(g => new
                    {
                        UserId = g.Key,
                        LastMessage = g.First()
                    })
                    .ToList();

                var userIds = conversations.Select(c => c.UserId).ToList();
                var users = await _context.Users
                    .Where(u => userIds.Contains(u.Id))
                    .Select(u => new { u.Id, FullName = u.UserName, u.UserName, u.Initials }) 
                    .ToDictionaryAsync(u => u.Id);

                var result = conversations.Select(c => new
                {
                    UserId = c.UserId,
                    UserName = users.ContainsKey(c.UserId) ? users[c.UserId].FullName : "Unknown",
                    LastMessage = c.LastMessage.Content,
                    Timestamp = c.LastMessage.Timestamp,
                    UnreadCount = 0 
                });

                return Ok(result);
            }
        }
    }
}
