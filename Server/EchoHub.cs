namespace SignalRServer
{
    using Microsoft.AspNetCore.SignalR;

    public class EchoHub : Hub
    {
        public void Ping(string name)
        {
            Clients.Client(Context.ConnectionId).InvokeAsync("Pong", "Hello " + name);
        }
    }
}