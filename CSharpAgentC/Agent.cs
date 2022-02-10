﻿using System;
using System.Collections.Generic;
using System.Linq;
using PlanetWars.Shared;

namespace CSharpAgent
{
    public class Agent : AgentBase
    {
        public Agent(string name, string endpoint) : base(name, endpoint) { }

        public int Value(StatusResult gs, Planet planet, IEnumerable<Planet> myPlanets)
        {
            var notMyPlanets = gs.Planets.Where(i => i.OwnerId != MyId);
            var oppPlanets = gs.Planets.Where(i => i.OwnerId == OppId);
            var neutralPlanets = gs.Planets.Where(i => i.OwnerId == -1);

            var availableShips = myPlanets.Sum(i => i.NumberOfShips);

            if (planet.OwnerId == OppId)
                return (200 - gs.CurrentTurn) * planet.GrowthRate * 2 * (availableShips / Math.Max(1, planet.NumberOfShips));

            if (planet.OwnerId == MyId)
                return (200 - gs.CurrentTurn) * planet.GrowthRate * 5 * (availableShips / Math.Max(1, planet.NumberOfShips));

            else
                return (((200 - gs.CurrentTurn) * planet.GrowthRate) - planet.NumberOfShips) * (availableShips / Math.Max(1, planet.NumberOfShips));
        }

        public int ShipsNeeded(StatusResult gs, Planet source, Planet target)
        {
            var oppFleets = gs.Fleets.Where(i => i.OwnerId == OppId && i.SourcePlanetId == target.Id).Sum(i => i.NumberOfShips);
            var myFleets = gs.Fleets.Where(i => i.OwnerId == MyId && i.SourcePlanetId == target.Id).Sum(i => i.NumberOfShips)
                         + _pendingMoveRequests.Where(i => i.DestinationPlanetId == target.Id).Sum(i => i.NumberOfShips);

            return (target.OwnerId == OppId
                ? (target.NumberOfShips + (target.GrowthRate * (int)Math.Ceiling(source.Position.Distance(target.Position))) + 1) + oppFleets - myFleets
                : (target.NumberOfShips) - (oppFleets + myFleets)) + 1;
        }

        public override void Update(StatusResult gs)
        {
            if (gs.CurrentTurn == 0)
            {
                Console.WriteLine($"Turn {gs.CurrentTurn}\t Biding my time!");
                return;
            }

            if (gs.Planets.All(i => i.OwnerId == MyId))
            {
                Console.WriteLine($"Turn {gs.CurrentTurn}\t We won!");
                return;
            }

            var myPlanets = gs.Planets.Where(i => i.OwnerId == MyId);

            foreach (var target in gs.Planets.OrderByDescending(i => Value(gs, i, myPlanets)))
            {
                foreach (var source in myPlanets.OrderBy(i => i.Position.Distance(target.Position)))
                {
                    var fleets = Math.Min(ShipsNeeded(gs, source, target), source.NumberOfShips - _pendingMoveRequests.Where(i => i.SourcePlanetId == source.Id).Sum(i => i.NumberOfShips));
                    SendFleet(source.Id, target.Id, fleets);
                }
            }
        }
    }
}