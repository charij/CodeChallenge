using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlanetWars.Shared;

namespace CSharpAgent
{
    public class planetCost
    {
        public planetCost(Planet planet, int cost)
        {
            this.planet = planet;
            this.cost = cost;
        }
        public Planet planet { get; set; }
        public int cost { get; set; }

    };

    public class Agent : AgentBase
    {
        public Agent(string name, string endpoint) : base(name, endpoint) { }

        public override void Update(StatusResult gs)
        {
            // do cool ai stuff
            Console.WriteLine($"[{DateTime.Now.ToShortTimeString()}]Current Turn: {gs.CurrentTurn}");
            Console.WriteLine($"My ID: {MyId}");
            Console.WriteLine($"Owned Planets: {string.Join(", ", gs.Planets.Where(p => p.OwnerId == MyId).Select(p => p.Id))}");
            var opplayer = 0;
            int ownedShips = 0;
            if (gs.PlayerA == MyId)
            {
                opplayer = gs.PlayerB;
            }
            else
            {
                opplayer = gs.PlayerA;
            };

            // find the first planet we don't own
            var targetPlanet = gs.Planets.FirstOrDefault(p => p.OwnerId != MyId);
            if (targetPlanet == null) return; // WE OWN IT ALLLLLLLLL

            var targetList = new List<planetCost>();

            Console.WriteLine($"Target Planet: {targetPlanet.Id}:{targetPlanet.NumberOfShips}");
            foreach (var planet in gs.Planets.Where(p => p.OwnerId == MyId))
            {
                ownedShips += planet.NumberOfShips;
            };

            // send half rounded down of our ships from each planet we do own
            foreach (var planet in gs.Planets.Where(p => (p.OwnerId != MyId) && (p.NumberOfShips < ownedShips)))
            {
                int planetValue = (200 - gs.CurrentTurn) * planet.GrowthRate;
                if (planet.OwnerId != opplayer)
                {
                    planetValue -= planet.NumberOfShips;
                };
                targetList.Add(new planetCost(planet, planetValue));
            }

            targetList = targetList.OrderBy(o => o.cost).ToList();

            for (int i = targetList.Count - 1; i >= 0; i--)
            {
                foreach (var planet in gs.Planets.Where(p => (p.OwnerId == MyId)))
                {
                    var minHp = planet.GrowthRate + 1;
                    if (planet.NumberOfShips > minHp)
                    {
                        int shipsToSend = planet.NumberOfShips - minHp;

                        if (targetList[i].planet.NumberOfShips < planet.NumberOfShips - 1)
                        {
                            shipsToSend = targetList[i].planet.NumberOfShips + 1;
                        };

                        if (planet.NumberOfShips >= minHp)
                        {
                            Console.WriteLine($"planet ships {planet.NumberOfShips} Amount sending {shipsToSend}");
                            SendFleet(planet.Id, targetList[i].planet.Id, shipsToSend);                           
                        };
                    };

                };
            }


        }
    }
}