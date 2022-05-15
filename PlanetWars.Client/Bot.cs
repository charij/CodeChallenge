namespace PlanetWars.Client
{
    using PlanetWars.Common.Data;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// AI Bot
    /// </summary>
    public class Bot
    {
        private readonly Player profile;
        private readonly GameDetails gameState;

        /// <summary>
        /// Creates an instance of this object
        /// </summary>
        /// <param name="profile">The player profile of the logged on client</param>
        /// <param name="gameState">Contains all relevant information for the game</param>
        public Bot(Player profile, GameDetails gameState)
        {
            this.profile = profile;
            this.gameState = gameState;
        }

        /// <summary>
        /// Current Plan is to send all spare ships from planets we own to a single one that we don't
        /// </summary>
        /// <returns>Commands for the turn</returns>
        public Command[] CalculateMovesForTurn()
        {
            var currentTurn  = gameState.History.Count - 1;
            var currentState = gameState.History[currentTurn];
            var moves = new List<Command>();

            var myPlanets    = currentState.Planets.Where(p => p.OwnerId == profile.Id);
            var targetPlanet = currentState.Planets.Where(p => p.OwnerId != profile.Id).FirstOrDefault();

            if (!myPlanets.Any() || targetPlanet == null)
            {
                return moves.ToArray();
            }

            foreach (var planet in myPlanets)
            {
                var spareShips = planet.ShipCount - 1;
                if (spareShips > 0)
                {
                    moves.Add(new Command
                    {
                        SourcePlanetId = planet.Id,
                        TargetPlanetId = targetPlanet.Id,
                        ShipCount = spareShips
                    });
                }
            }

            return moves.ToArray();
        }
    }
}