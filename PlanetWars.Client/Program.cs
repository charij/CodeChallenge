namespace PlanetWars.Client
{
    using PlanetWars.Common.Comm;
    using Microsoft.AspNetCore.SignalR.Client;
    using System;
    using System.Threading;

    public class Program
    {
        public async static void Main(string[] args)
        {
            try
            {
                var logon = new Logon(args);

                var exitEvent = new ManualResetEvent(false);
                var endpoint = args?[0] ?? "http://127.0.0.1:8088/";
                var preferences = new Preferences
                {
                    Id   = args.Length >= 2 ? Guid.Parse(args[1]) : Guid.NewGuid(),
                    Name = args.Length >= 3 ? args?[2] : "Anonymous",
                    Game = args.Length >= 4 ? Guid.Parse(args[3]) : Guid.Parse("")
                };

                var bot = new Bot();

                Console.CancelKeyPress += (sender, eventArgs) =>
                {
                    eventArgs.Cancel = true;
                    exitEvent.Set();
                };

                var connection = new HubConnectionBuilder()
                    .WithUrl(endpoint)
                    .WithAutomaticReconnect()
                    .Build();

                connection.On<string>("Update", async (state) => await connection.InvokeAsync("Submit", await bot.Process(state)));
                connection.On<string>("GameOver", (message) => { Console.WriteLine(message); exitEvent.Set(); });
                
                Console.WriteLine("Connecting to game server...");
                await connection.StartAsync();

                Console.WriteLine("Requesting game...");
                await connection.InvokeAsync("SetPreferences", preferences);

                Console.WriteLine("awaiting game start...");
                exitEvent.WaitOne();

                Console.WriteLine("Disconnecting from game server.");                
                await connection.StopAsync();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }
    }
}