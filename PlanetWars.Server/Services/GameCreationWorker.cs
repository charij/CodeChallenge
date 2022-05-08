namespace PlanetWars.Server.Services
{
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using System.Threading;
    using System.Threading.Tasks;

    public class GameCreationWorker : BackgroundService
    {
        private readonly ILogger<GameCreationWorker> _logger;
        private readonly IHubContext<GameHub> _signalRHub;

        public GameCreationWorker(ILogger<GameCreationWorker> logger, IHubContext<GameHub> signalRHub)
        {
            _logger = logger;
            _signalRHub = signalRHub;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);

                //var clients = ;
                //while ()
                //{
                //    _logger.LogInformation($"Requesting Match: {} vs {}");
                //    await _signalRHub.Clients.All.SendAsync("ReceiveMessage", "Server", "ping");
                //}

            }
        }
    }
}