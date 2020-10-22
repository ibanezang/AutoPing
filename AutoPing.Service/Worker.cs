using System;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AutoPing.Service
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var who = "www.google.com";
                var sleepTime = 5000;

                Ping pingSender = new Ping();

                // Create a buffer of 32 bytes of data to be transmitted.
                string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
                byte[] buffer = Encoding.ASCII.GetBytes(data);

                // Wait 12 seconds for a reply.
                int timeout = 12000;

                // Set options for transmission:
                // The data can go through 64 gateways or routers
                // before it is destroyed, and the data packet
                // cannot be fragmented.
                PingOptions options = new PingOptions(64, true);

                while (true)
                {
                    _logger.Log(LogLevel.Information, $"Time to live: {options.Ttl}");
                    _logger.Log(LogLevel.Information, $"Don't fragment: {options.DontFragment}");

                    // Send the ping asynchronously.
                    // Use the waiter as the user token.
                    // When the callback completes, it can wake up this thread.
                    var reply = await pingSender.SendPingAsync(who, timeout, buffer, options);

                    Console.WriteLine("ping status: {0}", reply.Status);
                    if (reply.Status == IPStatus.Success)
                    {
                        _logger.Log(LogLevel.Information, $"Address: {reply.Address}");
                        _logger.Log(LogLevel.Information, $"RoundTrip time: {reply.RoundtripTime}");
                    }

                    Thread.Sleep(sleepTime);
                }
            }
        }
    }
}
