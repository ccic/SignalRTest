namespace Client
{
    using System;
    using Microsoft.AspNetCore.SignalR.Client;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using System.Threading;
    using System.Collections.Generic;
    using System.Linq;

    class Program
    {
        struct Record
        {
            public DateTime when;
            public TimeSpan connect;
            public TimeSpan total;
        }

        static async Task<Record> Ping()
        {
            DateTime date = DateTime.Now;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            TimeSpan recv;
            // var connection = new HubConnectionBuilder().WithUrl("http://localhost:50137/ping").WithConsoleLogger().Build();
            var connection = new HubConnectionBuilder().WithUrl("http://localhost:50137/ping").Build();
            AutoResetEvent e = new AutoResetEvent(false);
            TimeSpan connect;
            connection.On<string>("Pong", data =>
            {
                recv = sw.Elapsed;
                e.Set();
            });

            await connection.StartAsync();
            connect = sw.Elapsed;
            await connection.InvokeAsync("Ping", "World");
            return new Record
            {
                when = date,
                connect = connect,
                total = sw.Elapsed

            };
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Start at " + DateTime.Now);
            var total = 5000;
            var ts = new Task<Record>[200];
            // init
            for (var i = 0; i < ts.Length; i++) ts[i] = Ping();
            int running = ts.Length, remaining = total - running;
            var records = new List<Record>();
            while (running > 0)
            {
                var i = Task.WaitAny(ts);
                records.Add(ts[i].Result);
                if (remaining > 0)
                {
                    ts[i] = Ping();
                    remaining--;
                }
                else running--;
            }
            var grouped = records.GroupBy(_ => new DateTime(_.when.Year, _.when.Month, _.when.Day, _.when.Hour, _.when.Minute, _.when.Second));
            foreach (var g in grouped)
            {
                var c = g.Select(_ => _.connect).Average(_ => _.TotalMilliseconds);
                var s = g.Select(_ => _.total - _.connect).Average(_ => _.TotalMilliseconds);
                Console.WriteLine("Time: {0}, count: {1}, connect: {2}, send: {3}", g.Key, g.Count(), c, s);
            }
            Console.WriteLine("End at " + DateTime.Now);
        }
    }
}
