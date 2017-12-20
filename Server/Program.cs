namespace SignalRServer
{
    using System;
    using System.Linq;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;

    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: server [server_procesing_time] [group_size]");
                return;
            }
            PingHub.Delay = int.Parse(args[0]);
            PingHub.MaxGroupSize = int.Parse(args[1]);
            BuildWebHost(args.Skip(1).ToArray()).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args.Skip(2).ToArray())
                .UseStartup<Startup>()
                .Build();
    }
}
