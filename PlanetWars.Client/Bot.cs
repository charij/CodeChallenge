namespace PlanetWars.Client
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class Bot
    {
        private readonly Settings settings;

        public Bot(Settings settings) 
        {
            this.settings = settings;
        }

        public Task<string> Process(string gameState, CancellationToken token)
        {
            var gs = JsonConvert.DeserializeObject<State>(gameState);
            var moves = new List<MoveRequest>();

            Console.WriteLine($"[{DateTime.Now.ToShortTimeString()}]Current Turn: {gs.CurrentTurn}");
            Console.WriteLine($"Owned Planets: {string.Join(", ", gs.Planets.Where(p => p.OwnerId == gs.MyId).Select(p => p.Id))}");

            var targetPlanet = gs.Planets.FirstOrDefault(p => p.OwnerId != gs.MyId);
            if (targetPlanet != null)
            {
                Console.WriteLine($"Target Planet: {targetPlanet.Id}:{targetPlanet.NumberOfShips}");

                foreach (var planet in gs.Planets.Where(p => p.OwnerId == gs.MyId))
                {
                    var ships = (int)Math.Floor(planet.NumberOfShips / 2.0);
                    if (ships > 0)
                    {
                        moves.Add(new MoveRequest
                        {
                            SourcePlanetId = planet.Id,
                            DestinationPlanetId = targetPlanet.Id,
                            NumberOfShips = ships
                        });
                    }
                }
            }

            return Task.FromResult(JsonConvert.SerializeObject(moves));
        }
    }
}