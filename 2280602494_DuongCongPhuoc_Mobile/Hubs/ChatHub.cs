using Microsoft.AspNetCore.SignalR;
using _2280602494_DuongCongPhuoc_Mobile.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace _2280602494_DuongCongPhuoc_Mobile.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _context;

        public ChatHub(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SendMessage(string receiverId, string content)
        {
            var senderId = Context.UserIdentifier;
            
            // Save to DB
            var message = new ChatMessage
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = content,
                Timestamp = DateTime.Now,
                IsRead = false
            };

            _context.ChatMessages.Add(message);
            await _context.SaveChangesAsync();

            // Send to Receiver
            await Clients.User(receiverId).SendAsync("ReceiveMessage", senderId, content, message.Timestamp);
            
            // Send back to Sender (to confirm sent/update UI if needed, though usually UI updates optimistically)
            await Clients.Caller.SendAsync("ReceiveMessage", senderId, content, message.Timestamp);
        }
    }
}
