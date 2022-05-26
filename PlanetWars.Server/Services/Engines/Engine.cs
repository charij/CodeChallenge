namespace PlanetWars.Common
{
    using PlanetWars.Common.Comm;
    using PlanetWars.Common.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Engine
    {
        private readonly Game gameDetails;
        private readonly Dictionary<Guid, CommandRequest[]> commandBuffer = new();
        private readonly object commandLock = new();

        public Engine(GameSettings settings, List<string> players)
        {
            gameDetails = new Game 
            { 
                Id = Guid.NewGuid().ToString(),
                Players = players,
                Settings = settings
            };
            gameDetails.History.Add(GenerateInitialState());
        }

        public State GenerateInitialState()
        {
            var settings = gameDetails.Settings;
            var rng = new Random(settings.Seed);
            var state = new State();
            var planetsPerSlice = rng.Next(settings.MinPlanetCount, settings.MaxPlanetCount);

            for (var i = 0; i < planetsPerSlice; i++)
            {
                var shipCount = rng.Next(settings.MinPlanetShips, settings.MaxPlanetShips);
                int growthRate = rng.Next(settings.MinPlanetGrowthRate, settings.MaxPlanetGrowthRate);
                int r = 0;
                double t;
                Point p;

                do
                {
                    r = rng.Next(settings.MinRadius, settings.MaxRadius);
                    t = rng.NextDouble() * 2 * Math.PI / gameDetails.Players.Count;
                    p = new Point
                    {
                        X = (int)Math.Round(r * Math.Cos(t)),
                        Y = (int)Math.Round(r * Math.Sin(t))
                    };
                }
                while (state.Planets.Any(i => p.Distance(i.Position) < 2 * settings.MaxPlanetGrowthRate));

                for (var j = 0; j < gameDetails.Players.Count; j++)
                {
                    state.Planets.Add(new Planet
                    {
                        Id = Guid.NewGuid(),
                        OwnerId = i == 0 ? gameDetails.Players[j].Id : Guid.Empty,
                        GrowthRate = rng.Next(settings.MinPlanetGrowthRate, settings.MaxPlanetGrowthRate),
                        ShipCount = rng.Next(settings.MinPlanetShips, settings.MaxPlanetShips),
                        Position = new Point
                        {
                            X = (int)Math.Round(r * Math.Cos((t + (Math.PI * 2 * j / gameDetails.Players.Count)) % (Math.PI * 2))),
                            Y = (int)Math.Round(r * Math.Sin((t + (Math.PI * 2 * j / gameDetails.Players.Count)) % (Math.PI * 2)))
                        }
                    });
                }
            }

            var isCenterPlanet = rng.Next(0, 1) == 1;
            if (isCenterPlanet)
            {
                state.Planets.Add(new Planet
                {
                    Id = Guid.NewGuid(),
                    OwnerId = Guid.Empty,
                    GrowthRate = rng.Next(settings.MinPlanetGrowthRate, settings.MaxPlanetGrowthRate),
                    ShipCount = rng.Next(settings.MinPlanetShips, settings.MaxPlanetShips),
                    Position = new Point { X = 0, Y = 0 }
                });
            }

            return state;
        }

        public string SubmitCommands(Guid playerId, CommandRequest[] commands)
        {
            var errors = string.Empty;

            lock (commandLock)
            {
                var gameState = gameDetails.History.Last();

                if (commandBuffer.ContainsKey(playerId))
                {
                    errors = Environment.NewLine + $"Player {playerId} has already submitted commands this turn.";
                }
                else
                {
                    var planets = gameState.Planets.ToDictionary(i => i.Id, i => new Planet(i));

                    foreach (var command in commands)
                    {
                        if (!planets.TryGetValue(command.SourcePlanetId, out var sourcePlanet))
                        {
                            errors = string.Join(Environment.NewLine, errors, $"Source planet {command.SourcePlanetId} does not exist.");
                        }
                        else
                        if (!planets.TryGetValue(command.TargetPlanetId, out var targetPlanet))
                        {
                            errors = string.Join(Environment.NewLine, errors, $"Target planet {command.TargetPlanetId} does not exist.");
                        }
                        else
                        if (sourcePlanet.OwnerId != playerId)
                        {
                            errors = string.Join(Environment.NewLine, errors, $"Source planet {command.SourcePlanetId} is not owned by player {playerId}.");
                        }
                        else
                        if (sourcePlanet.ShipCount < command.ShipCount)
                        {
                            errors = string.Join(Environment.NewLine, errors, $"Source planet {command.SourcePlanetId} does not enough ships for {command.ShipCount}");
                        }
                        else
                        {
                            sourcePlanet.ShipCount -= command.ShipCount;
                        }
                    }

                    if (string.IsNullOrEmpty(errors))
                    {
                        commandBuffer[playerId] = commands;
                    }
                }
            }

            return errors;
        }

        public State ProcessTurn()
        {
            lock (commandLock)
            {
                var gameState = gameDetails.History.Last();

                // Grow Planets
                foreach (var planet in gameState.Planets)
                {
                    if (planet.OwnerId != Guid.Empty)
                    {
                        planet.ShipCount += planet.GrowthRate;
                    }
                }

                // Send Fleets
                foreach (var (playerId, commands) in commandBuffer)
                {
                    foreach (var command in commands)
                    {
                        gameState.Planets.Find(i => i.Id == command.SourcePlanetId).ShipCount -= command.ShipCount;
                        gameState.Fleets.Add(new Fleet 
                        {
                            Id = Guid.NewGuid(),
                            OwnerId = playerId,
                            SourcePlanetId = command.SourcePlanetId,
                            TargetPlanetId = command.TargetPlanetId,
                            ShipCount = command.ShipCount,
                            TurnCreated = gameState.TurnNumber
                        });
                    }
                }

                // Planet Battles
                var completedFleets = gameState.Fleets
                    .Where(i => gameState.TurnNumber - i.TurnCreated >= i.TravelTime)
                    .GroupBy(i => i.TargetPlanetId);

                foreach (var fleet in completedFleets)
                {
                    var targetPlanet = gameState.Planets.Find(i => i.Id == fleet.Key);
                    var playerFleets = fleet.GroupBy(i => i.OwnerId).Select(i => new { OwnerId = i.Key, ShipCount = i.Sum(j => j.ShipCount) });

                    // Apply reinforcements
                    var reinforcements = playerFleets.FirstOrDefault(i => i.OwnerId == targetPlanet.OwnerId);
                    if (reinforcements != null)
                    {
                        targetPlanet.ShipCount += reinforcements.ShipCount;
                    }

                    // Apply main attacking force
                    var mainBattleFleet = playerFleets.Where(i => i.OwnerId != targetPlanet.OwnerId).OrderByDescending(i => i.ShipCount).FirstOrDefault();
                    if (mainBattleFleet != null)
                    {
                        targetPlanet.ShipCount -= mainBattleFleet.ShipCount;
                        if (targetPlanet.ShipCount == 0)
                        {
                            targetPlanet.OwnerId = Guid.Empty;
                        }
                        else
                        if (targetPlanet.ShipCount < 0)
                        {
                            targetPlanet.OwnerId = mainBattleFleet.OwnerId;
                        }
                    }

                    // cleanup
                    var fleetIds = fleet.Select(i => i.Id).ToHashSet();
                    gameState.Fleets.RemoveAll(i => fleetIds.Contains(i.Id));
                }

                // Check game over conditions
                gameState.IsGameOver =
                       gameState.TurnNumber >= gameDetails.Settings.TurnLimit
                    || gameState.Planets.Where(i => i.OwnerId != Guid.Empty).Distinct().Count() <= 1;

                // Complete Turn
                gameState.TurnNumber += 1;
                return gameState;
            }
        }
    }
}