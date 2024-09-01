using Microsoft.AspNetCore.SignalR;
using TodoWeb.Data;
using TodoWeb.Models;

namespace TodoWeb
{
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _context;

        public ChatHub(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task SendMessage(int projectId, string message)
        {

            var userId = Context.UserIdentifier;
            var nameUser = _context.Users.Find(userId).Name;

            var newMessage = new Message
            {
                IdProject = projectId,
                IdUser = userId,
                Content = message,
                //TimeStamp = Utils.Utils.GetJktTime(),
                TimeStamp = DateTime.UtcNow,
            };

            _context.Messages.Add(newMessage);
            _context.SaveChanges();

            await Clients.Groups(projectId.ToString()).SendAsync("ReceiveMessage", nameUser, message);
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;

            var userProjects = _context.Participants
                .Where(p => p.UserId == userId)
                .Select(p => p.ProjectId)
                .ToList();

            foreach (var projectId in userProjects) {
                await Groups.AddToGroupAsync(Context.ConnectionId, projectId.ToString());
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier;

            var userProjects = _context.Participants
                .Where(p => p.UserId == userId)
                .Select(p => p.ProjectId)
                .ToList();

            foreach (var projectId in userProjects) { 
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, projectId.ToString());
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
