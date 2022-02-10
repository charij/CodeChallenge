using System;
using System.Collections.Generic;
using System.Linq;
using PlanetWars.Shared;

namespace CSharpAgent
{
    public class Agent : AgentBase
    {
        internal class ScheduleMove
        {
            public int Turn { get; set; }
            public int SourceId { get; set; }
            public int TargetId { get; set; }
            public int NumberOfShips { get; set; }
        }

        internal class PlanetPlan
        {
            public int ReservedShips { get; set; }
        }

        public Agent(string name, string endpoint) : base(name, endpoint) { }

        public int Value(StatusResult gs, Planet planet, Planet[] OurPlanets)
        {
            return planet.OwnerId == 0
                ? ((200 - gs.CurrentTurn) * planet.GrowthRate) - planet.NumberOfShips
                : ((200 - gs.CurrentTurn) * planet.GrowthRate) * 2;
        }

        private StatusResult previousState;
        private List<ScheduleMove> scheduledMoves = new List<ScheduleMove>();
        private Dictionary<int, PlanetPlan> planetPlans;

        public override void Update(StatusResult gs)
        {
            if (gs.CurrentTurn == 0)
            {
                Console.WriteLine($"Turn {gs.CurrentTurn}\t Biding my time!");
                previousState = gs;
                planetPlans = gs.Planets.ToDictionary(i => i.Id, i => new PlanetPlan());
                return;
            }

            if (gs.Planets.All(i => i.OwnerId == MyId))
            {
                Console.WriteLine($"Turn {gs.CurrentTurn}\t We won!");
                return;
            }

            // get opponents moves & counter based on value
            var previousFleets = previousState.Fleets.Select(i => i.Id);
            var opponentMoves = gs.Fleets.Where(i => !previousFleets.Contains(i.Id));
            foreach (var move in opponentMoves)
            {
                // add scheduled moves

            }

            ExecuteScheduledMoves(gs);

            // move all remaining ships to defend weakest & attack most valuable
            var myPlanets = gs.Planets.Where(i => i.OwnerId == MyId);
            foreach (var planet in myPlanets)
            {

            }

            previousState = gs;
        }

        private void ExecuteScheduledMoves(StatusResult gs)
        {
            var moves = scheduledMoves.Where(i => gs.CurrentTurn == i.Turn).ToArray();
            foreach (var scheduleMove in moves)
            {
                var planet = gs.Planets.First(i => i.Id == scheduleMove.SourceId);

                if (planet.OwnerId != MyId)
                {
                    Console.WriteLine("\tLost the planet!");
                }
                else 
                if (planet.NumberOfShips < scheduleMove.NumberOfShips)
                {
                    Console.WriteLine("\tNot enough ships to send!");
                }
                else
                {
                    SendFleet(scheduleMove.SourceId, scheduleMove.TargetId, scheduleMove.NumberOfShips);
                }

                scheduledMoves.Remove(scheduleMove);
            }
        }
    }
}
