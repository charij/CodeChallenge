namespace PlanetWars.Client
{
    using PlanetWars.Common.Comm;
    using PlanetWars.Common.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// AI Bot
    /// </summary>
    public class Bot
    {
        private readonly Player profile;
        private readonly Game gameState;

        /// <summary>
        /// Creates an instance of this object
        /// </summary>
        /// <param name="profile">The player profile of the logged on client</param>
        /// <param name="gameState">Contains all relevant information for the game</param>
        public Bot(Player profile, Game gameState)
        {
            this.profile = profile;
            this.gameState = gameState;
        }

        /// <summary>
        /// Current Plan is to send all spare ships from planets we own to a single one that we don't
        /// </summary>
        /// <returns>Commands for the turn</returns>
        public CommandRequest[] CalculateMovesForTurn()
        {
            var currentTurn = gameState.History.Count - 1;
            var currentState = gameState.History[currentTurn];
            var moves = new List<CommandRequest>();

            var myPlanets = currentState.Planets.Where(p => p.OwnerId == profile.Id);
            var notMyPlanets = currentState.Planets.Where(p => p.OwnerId != profile.Id);

            var targetPlanet = notMyPlanets.OrderBy(i => myPlanets.Min(j => j.Position.Distance(i.Position))).First();

            Console.WriteLine($"[{DateTime.Now.ToShortTimeString()}] Current Turn: {currentTurn}");
            Console.WriteLine($"Owned Planets: {string.Join(", ", currentState.Planets.Where(p => p.OwnerId == profile.Id).Select(p => p.Id))}");
            Console.WriteLine($"Target Planet: {targetPlanet.Id}:{targetPlanet.ShipCount}");

            // send available ships from each planet we own
            foreach (var planet in currentState.Planets.Where(p => p.OwnerId == profile.Id))
            {
                var ships = planet.ShipCount - 1;
                if (ships > 0)
                {
                    moves.Add(new CommandRequest
                    {
                        SourcePlanetId = planet.Id,
                        TargetPlanetId = targetPlanet.Id,
                        ShipCount = ships,
                    });
                }
            }

            return moves.ToArray();
        }
    }
}