using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlanetWars.Shared;

namespace CSharpAgent
{
    public class Agent : AgentBase
    {
        public Agent(string name, string endpoint) : base(name, endpoint){}

        public override void Update(StatusResult gs)
        {
            // do cool ai stuff
            Console.WriteLine($"[{DateTime.Now.ToShortTimeString()}]Current Turn: {gs.CurrentTurn}");
            Console.WriteLine($"My ID: {MyId}");
            Console.WriteLine($"Owned Planets: {string.Join(", ", gs.Planets.Where(p => p.OwnerId == MyId).Select(p =>  p.Id))}");

            // find the first planet we don't own
            var targetPlanet = gs.Planets.FirstOrDefault(p => p.OwnerId != MyId);
            if (targetPlanet == null) return; // WE OWN IT ALLLLLLLLL

            Console.WriteLine($"Target Planet: {targetPlanet.Id}:{targetPlanet.NumberOfShips}");

            var turnsWaitedBigBoy = 0;
            var underAttackTurns = 0;
            
            // send half rounded down of our ships from each planet we do own
            foreach (var planet in gs.Planets.Where(p => p.OwnerId == MyId))
            {
                

                var planetsWeOwn = gs.Planets.Where(p => p.OwnerId == MyId);

                var bigBoy = gs.Planets.OrderBy(x => x.GrowthRate).Last();
                
                var notOwnedPlanets = gs.Planets.Where(p => p.OwnerId != MyId);
                
                var ships = (int)Math.Floor(planet.NumberOfShips / 4.0);

                var largestEnemyAttackFleet = gs.Fleets.Where(x => x.OwnerId != MyId).OrderBy(x => x.NumberOfShips).FirstOrDefault();

                var shipsAttackingUs = gs.Fleets.Where(x => x.OwnerId != MyId)
                    .Where(x => x.DestinationPlanetId == planet.Id).Select(x => x.NumberOfShips).Sum();
                
                
                
                if (shipsAttackingUs > 0)
                {
                    var differenceInShips = planet.NumberOfShips - shipsAttackingUs;
                    if (differenceInShips < 1)
                    {
                        continue;
                    }

                    ships = differenceInShips;
                }
                
                // if planet that the enemy is attacking is not ours
                
                var enemyAttackingNeutralPlanet = notOwnedPlanets.Any(x => largestEnemyAttackFleet != null && x.Id == largestEnemyAttackFleet.DestinationPlanetId);
                
                
                

                if (largestEnemyAttackFleet != null && enemyAttackingNeutralPlanet)
                {
                    targetPlanet = gs.Planets.First(x => x.Id == largestEnemyAttackFleet.SourcePlanetId);

                    ships = planet.NumberOfShips /2;
                    
                    var numberOfShips = targetPlanet.NumberOfShips;

                    if (ships > numberOfShips)
                    {
                        ships -= numberOfShips;
                    }
                }
                else
                {
                    var distances = new Dictionary<Planet, double>();

                    double shortestDistance = double.MaxValue;
            
                    foreach (var notOwnedPlanet in notOwnedPlanets)
                    {
                        var distance = planet.Position.Distance(notOwnedPlanet.Position);
                    
                        distances.Add(notOwnedPlanet, distance);
                    
                        // distances[planet] = distance;
                    
                    }

                    var planetDistances = distances.OrderBy(x => x.Value).ToList();

                    if (planetDistances.Count() > 1)
                    {
                        targetPlanet = planetDistances[0].Key.NumberOfShips < planetDistances[1].Key.NumberOfShips
                            ? planetDistances[0].Key
                            : planetDistances[1].Key;

                        var targetPlanetNumberOfShips = ships - targetPlanet.NumberOfShips;

                        if (targetPlanetNumberOfShips > 0)
                        {
                            ships = targetPlanetNumberOfShips;
                        }
                        
                    }
                    else
                    {
                        targetPlanet = planetDistances.First().Key;
                    }
                        
                }
                
                
                
                
                
                if (ships > 0)
                {
                    SendFleet(planet.Id, targetPlanet.Id, ships);
                }
            }
        }
    }
}
