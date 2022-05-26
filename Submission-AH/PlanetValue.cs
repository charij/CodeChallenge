using System;

namespace CSharpAgent
{
    public class PlanetValue
    {
        int _growthRate = 0;
        int _stepsLeft = 0;
        int _neutralShips = 0;

        public PlanetValue(int growthRate, int stepsLeft, int neutralShips)
        {
            _growthRate = growthRate;
            _stepsLeft = stepsLeft;
            _neutralShips = neutralShips;    
        }

        public int TotalProduction { get; set; }
        public int ForUs { get; private set; }
        public int ForEnemy { get; private set; }
        public int Saldo 
        { 
            get 
            {
                return ForUs - ForEnemy;
            } 
        }

        public void CalculateValue(
            int incomingEnemyShips, 
            int incomingOurShips,
            double nearestEnemyPlanet,
            double nearestOurPlanet)
        {
            TotalProduction = _stepsLeft * _growthRate - _neutralShips;

            // TODO: better distance adjustment. Close planets will start producing SOONER. 
            // This is not taken in account
            
            var balance = incomingOurShips - incomingEnemyShips;
            
            ForUs = TotalProduction + balance - Convert.ToInt32(nearestEnemyPlanet);
            ForEnemy = TotalProduction - balance - Convert.ToInt32(nearestOurPlanet);
        }
    }
}