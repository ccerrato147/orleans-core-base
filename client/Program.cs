using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using Orleans.Runtime.Configuration;
using interfaces;

namespace client
{
    public class Program
    {
        static int Main(string[] args)
        {
            return RunMainAsync().Result;
        }

        private static async Task<int> RunMainAsync()
        {
            try
            {
                using(var client = await StartClientWithRetries())
                {
                    await DoClientWork(client);
                    Console.ReadKey();
                }
                return 0;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return 1;
            }
        }

        public static async Task<IClusterClient> StartClientWithRetries(int initializeAttemptsBeforeFailing = 5)
        {
            int attempt = 0;
            IClusterClient client;
            while(true)
            {
                try
                {
                    var config = ClientConfiguration.LocalhostSilo();
                    client = new ClientBuilder()
                        .UseConfiguration(config)
                        .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(ISimpleText).Assembly).WithReferences())
                        .ConfigureLogging(logging => logging.AddConsole())
                        .Build();
                    
                    await client.Connect();
                    Console.WriteLine("Client successfully connect to silo host");
                    break;
                }
                catch(SiloUnavailableException)
                {
                    attempt++;
                    Console.WriteLine($"Attempt {attempt} of {initializeAttemptsBeforeFailing} failed to initialize the Orleans client.");
                    if(attempt > initializeAttemptsBeforeFailing)
                    {
                        throw;
                    }
                    await Task.Delay(TimeSpan.FromSeconds(4));
                }
            }

            return client;
        }

        private static async Task DoClientWork(IClusterClient client)
        {
            var simple = client.GetGrain<ISimpleText>(0);
            var text = await simple.GetString("Base project");
            Console.WriteLine("\n\n{0}\n\n", text);
        }
    }
}
