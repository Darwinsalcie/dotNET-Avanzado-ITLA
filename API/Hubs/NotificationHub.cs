using Microsoft.AspNetCore.SignalR;

namespace API.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task SendMsj(string msj) 
            => await Clients.All.SendAsync("ReceiveNotificacion", msj);
    }
}
