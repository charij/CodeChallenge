using PlanetWars.Shared;
using System;
using System.Linq;

namespace CSharpAgent
{
    public class AgentPlanet : Planet
    {
        private int TurnsLeft = 200;
        private int _ourOwnerId;
        StatusResult _gameState;

        public AgentPlanet(Planet planet, int ourOwnerId, StatusResult gs)
        {
            _gameState = gs;
            _ourOwnerId = ourOwnerId;

            Id = planet.Id;
            NumberOfShips = planet.NumberOfShips;
            GrowthRate = planet.GrowthRate;
            Size = planet.Size;
            Position = planet.Position;
            OwnerId = planet.OwnerId;


            CalculateValue();
        }

        public void CalculateValue()
        {
            int incomingEnemyShips = _gameState.Fleets
                .Where(x => x.OwnerId != _ourOwnerId && x.DestinationPlanetId == Id)
                .Sum(x => x.NumberOfShips);

            int incomingOurShips = _gameState.Fleets
                .Where(x => x.OwnerId == _ourOwnerId && x.DestinationPlanetId == Id)
                .Sum(x => x.NumberOfShips);

            double nearestEnemyPlanet = _gameState.Planets
                .Where(x => x.OwnerId != -1 && x.OwnerId != _ourOwnerId)
                .Min(x => x.Position.Distance(Position));

            double nearestOurPlanet = _gameState.Planets
                .Where(x => x.OwnerId == _ourOwnerId)
                .Min(x => GetDistanceTo(x));

            var ships = OwnerId == -1 ? NumberOfShips : 0;
            
            Value = new PlanetValue(GrowthRate, TurnsLeft, ships);
            Value.CalculateValue(incomingEnemyShips, incomingOurShips, nearestEnemyPlanet, nearestOurPlanet);
        }

        
        public void SetShipsRequiredForDefence()
        {
            if (OwnerId != _ourOwnerId)
            {
                ShipsRequiredForDefence = 0;
                return;
            }

            int incomingEnemyShips = _gameState.Fleets
                .Where(x => x.OwnerId != _ourOwnerId && x.DestinationPlanetId == Id)
                .Sum(x => x.NumberOfShips);

            int incomingOurShips = _gameState.Fleets
                .Where(x => x.OwnerId == _ourOwnerId && x.DestinationPlanetId == Id)
                .Sum(x => x.NumberOfShips);

            ShipsRequiredForDefence = NumberOfShips + GrowthRate - incomingEnemyShips + incomingOurShips + 1;

        }

        public void SetShipsRequiredToConquer()
        {
            if (OwnerId == _ourOwnerId)
            {
                ShipsRequiredForDefence = 0;
                return;
            }



            ShipsRequiredToConquer = Status == PlanetStatus.Hostile
                ? NumberOfShips + GrowthRate + IncomingHostileShips - IncomingOurShips + 1
                : NumberOfShips + IncomingHostileShips - IncomingOurShips + 1;

        }

        int IncomingHostileShips 
        {
            get
            {
                return _gameState.Fleets
                    .Where(x => x.OwnerId != _ourOwnerId && x.DestinationPlanetId == Id)
                    .Sum(x => x.NumberOfShips);
            }
        } 

        int IncomingOurShips { 
            get
            {
                return _gameState.Fleets
                    .Where(x => x.OwnerId == _ourOwnerId && x.DestinationPlanetId == Id)
                    .Sum(x => x.NumberOfShips);
            }
        } 

        public double GetDistanceTo(Planet target)
        {
            return Position.Distance(target.Position);
        }

        public void CalculateExcessShips(int newShipsSentOut)
        {
            if (Status != PlanetStatus.Friendly)
            {
                ExcessShips = 0;
                return;
            }

            var excess = NumberOfShips + GrowthRate - IncomingHostileShips - newShipsSentOut;

            ExcessShips = Math.Min(0, excess);
        }

        public PlanetStatus Status
        {
            get
            {
                if (OwnerId == -1) return PlanetStatus.Neutral;
                if (OwnerId == _ourOwnerId) return PlanetStatus.Friendly;

                return PlanetStatus.Hostile;
            }
        }

        public PlanetValue Value { get; private set; }
        public int ExcessShips { get; private set; }
        public int ShipsRequiredForDefence { get; private set; }
        public int ShipsRequiredToConquer { get; private set; }

    }
}