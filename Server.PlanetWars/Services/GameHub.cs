namespace Server.PlanetWars.Services
{
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IGameClient 
    {
        
    }

    public class GameHub : Hub
    {
        private readonly ILogger<GameHub> _logger;

        public readonly Dictionary<Guid, List<IHubCallerClients<IGameClient>>> AwaitingClients = new();

        public readonly Dictionary<Guid, List<IHubCallerClients<IGameClient>>> ActiveClients = new();

        public GameHub(ILogger<GameHub> logger)
        {
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
        }

        public async Task ProcessClientMessage(string user, string message)
        {
            // process an incoming message from a connected client
            _logger.LogInformation($"{DateTime.Now.ToString("hh:mm:ss.fff")}  MyHub.ProcessClientMessage({user}, {message})");

        }
    }
}