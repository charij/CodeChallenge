namespace CSharpAgent
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using PlanetWars.Shared;

    public class Agent : AgentBase
    {
        public Agent(string name, string endpoint)
            : base(name, endpoint)
        { }

        public override void Update(StatusResult gameState)
        {
            // Analyse Map Updates & Opponent Moves to update our data models

            // Recursively find the best moves i.e. not greedy, and given opponent potential moves (may be possible via gradient descent or Master method):
            //
            //  Our Best Moves
            //  Opp Best Moves
            //  
            //  Include:
            //      Attack Moves (attack valuable planets)
            //      Reinforcement Moves (Send reinforcements to our attacked planet)
            //      Defense Moves (get our ships as close to border as possible)
            //
            //      Planet Value:
            //          Ships produced by turn 200 (considering conquer time)
            //          Conquer Time (reduced if minor detour to enemy OR reinforcement)
            //          Cost to conquer (0 if enemy)
            //          Positioning (closer to enemy border is better)
            //
            //  Notes:
            //      - We need to add a timer to ensure submission is made within the time limit
            //      - May be useful to add stocastic decisions
            //      - May be useful to have adjusting weights based on opponents strategy relative to our calculated optimal one for them

            // Submit Moves
            //  Merge all commands with same source and destination planet Id


            Console.WriteLine($"[{DateTime.Now.ToShortTimeString()}] Current Turn: {gameState.CurrentTurn}");
            Console.WriteLine($"Owned Planets: {string.Join(", ", gameState.Planets.Where(p => p.OwnerId == MyId).Select(p => p.Id))}");

            // find the first planet we don't own
            var targetPlanet = gameState.Planets.FirstOrDefault(p => p.OwnerId != MyId && p.OwnerId != -1);
            if (targetPlanet == null) return;

            Console.WriteLine($"Target Planet: {targetPlanet.Id}:{targetPlanet.NumberOfShips}");

            // send available ships from each planet we own
            foreach (var planet in gameState.Planets.Where(p => p.OwnerId == MyId))
            {
                var ships = planet.NumberOfShips - 1;
                if (ships > 0)
                {
                    SendFleet(planet.Id, targetPlanet.Id, ships);
                }
            }
        }
    }
}