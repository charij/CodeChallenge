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

    public class planetDistance
    {
        public planetDistance(int planetId, double dist)
        {
            this.planetId = planetId;
            this.dist = dist;
        }
        public int planetId { get; set; }
        public double dist { get; set; }

    };

    public class ownedPlanets
    {
        public ownedPlanets(Planet planet, int attackingShips, List<planetDistance> attackablePlanets)
        {
            this.planet = planet;
            this.attackingShips = attackingShips;
            this.attackablePlanets = attackablePlanets;
        }
        public Planet planet { get; set; }
        public int attackingShips { get; set; }
        public List<planetDistance> attackablePlanets { get; set; }

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
            var ownedPlanets = new List<ownedPlanets>();

            Console.WriteLine($"Target Planet: {targetPlanet.Id}:{targetPlanet.NumberOfShips}");
            foreach (var planet in gs.Planets.Where(p => p.OwnerId == MyId))
            {
                var attackingShips = 0;
                List<planetDistance> attackablePlanets = new List<planetDistance>();
                foreach (var fleet in gs.Fleets.Where(p => (p.OwnerId == opplayer) && (p.DestinationPlanetId == planet.Id)))
                {
                    attackingShips += fleet.NumberOfShips;
                }

                foreach (var enemyplanet in gs.Planets.Where(p => p.OwnerId != MyId))
                {
                    var x = (enemyplanet.Position.X - planet.Position.X);
                    var y = (enemyplanet.Position.Y - planet.Position.Y);

                    var dist = Math.Sqrt((x * x) + (y * y));
                    var planetDist = new planetDistance(enemyplanet.Id, dist);
                    attackablePlanets.Add(planetDist);
                }

                attackablePlanets = attackablePlanets.OrderBy(o => o.dist).ToList();
                ownedShips += planet.NumberOfShips;
                ownedPlanets.Add(new ownedPlanets(planet, attackingShips, attackablePlanets));
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
                foreach (var owned in ownedPlanets)
                {
                    var minHp = owned.planet.GrowthRate + 1;
                    if ((owned.planet.NumberOfShips > minHp) && (owned.attackingShips < minHp))
                    {
                        int shipsToSend = owned.planet.NumberOfShips - minHp;

                        if (targetList[i].planet.NumberOfShips < owned.planet.NumberOfShips - 1)
                        {
                            shipsToSend = targetList[i].planet.NumberOfShips + 1;
                        };

                        if (owned.planet.NumberOfShips >= minHp)
                        {
                            Console.WriteLine($"planet ships {owned.planet.NumberOfShips} Amount sending {shipsToSend}");
                            SendFleet(owned.planet.Id, targetList[i].planet.Id, shipsToSend);                           
                        };
                    };

                };
            }


        }
    }
}