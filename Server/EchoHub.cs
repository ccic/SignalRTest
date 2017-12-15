namespace SignalRServer
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.SignalR;

    public class EchoHub : Hub
    {
        public async Task Ping(long payload1, string payload2)
        {
            // simulate a 500ms server processing time
            await Task.Delay(500);
            await Clients.Client(Context.ConnectionId).InvokeAsync("Pong", payload1, payload2);
        }
    }
}