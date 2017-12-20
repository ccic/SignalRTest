namespace SignalRServer
{
    using System;
    using System.Threading.Tasks;
    using System.Collections.Concurrent;
    using Microsoft.AspNetCore.SignalR;

    public class PingHub : Hub
    {
        static ConcurrentDictionary<string, string> groups = new ConcurrentDictionary<string, string>();

        static int currentGroup = 0;

        static int currentGroupCount = 0;

        public static int Delay { get; set; }

        public static int MaxGroupSize { get; set; }

        public override Task OnConnectedAsync()
        {
            if (MaxGroupSize == 0) return Task.CompletedTask;
            lock (this)
            {
                var g = currentGroup;
                currentGroupCount++;
                if (currentGroupCount >= MaxGroupSize)
                {
                    currentGroup++;
                    currentGroupCount = 0;
                }
                if (!groups.TryAdd(Context.ConnectionId, g.ToString())) throw new InvalidOperationException();
            }

            return Groups.AddAsync(Context.ConnectionId, groups[Context.ConnectionId]);
        }

        public async Task Ping(long payload1, string payload2)
        {
            // simulate server processing time
            await Task.Delay(Delay);
            await Task.WhenAll(
                Clients.Client(Context.ConnectionId).InvokeAsync("Pong", payload1, payload2),
                MaxGroupSize > 0 ? Clients.Group(groups[Context.ConnectionId]).InvokeAsync("Cc", payload1, payload2) : Task.CompletedTask);
        }
    }
}