using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Robot.Common
{
    
    public class Variant
    {
        private static Variant _instance;


        /// <summary>
        /// Максимальне значення енергії станції
        /// </summary>
        public readonly int MaxStationEnergy = 1000;

        /// <summary>
        /// Максимальне значення випадкової кількості енергії, що генерує станція
        /// </summary>
        internal readonly int MaxEnergyGrowth = 10;
        
        /// <summary>
        /// Мінімальне значення випадкової кількості енергії, що генерує станція
        /// </summary>
        internal readonly int MinEnergyGrowth = 5;

        /// <summary>
        /// Кількість станцій на учасника
        /// </summary>
        internal readonly int EnergyStationForAttendant = 5;

        /// <summary>
        /// Відстань на якій роботи можуть збирати енерцію.
        /// 0 - при необхіжності бути на станції для її збирання
        /// </summary>
        internal readonly int CollectingDistance = 0;

        /// <summary>
        /// Максимальне значення енергії, що збирає робот
        /// з одної станції
        /// </summary>
        internal readonly int MaxEnergyCanCollect = 200;


        /// <summary>
        /// втрати при створенні нового робота
        /// </summary>
        internal readonly int EnergyLossToCreateNewRobot = 200;

        /// <summary>
        /// Втрати при атаці на іншого робота
        /// </summary>
        internal readonly int AttackEnergyLoss = 10;

        /// <summary>
        /// Втрати при атаці на іншого робота
        /// </summary>
        internal readonly double StoleRateEnergyAtAttack = 0.0;

        /// <summary>
        /// Secure initialization for the second time
        /// </summary>
        /// <param name="variantNum"></param>
        public static void Initialize(int variantNum) 
        {
            if (_instance != null) throw new Exception("Варіант вже ініціалізований");
            _instance = new Variant(variantNum);
        }

        public static Variant GetInstance()
        {
            return _instance;
        }

        //If more needed start energy can be added
        private Variant(int variantNum)
        {

            switch (variantNum)
            {
                case 1:
                    MaxEnergyGrowth = 40;
                    MinEnergyGrowth = 20;
                    MaxStationEnergy = 5000;
                    CollectingDistance = 1;
                    
                    EnergyStationForAttendant = 5;
                    EnergyLossToCreateNewRobot = 100;
                    break;
                case 2:
                    EnergyStationForAttendant = 5;
                    MaxEnergyGrowth =  100;
                    MinEnergyGrowth = 50;
                    
                    MaxStationEnergy = 20000;
                    CollectingDistance = 2;
                    MaxEnergyCanCollect = 40;
                    EnergyLossToCreateNewRobot = 100;
                    break;
                
                case 3:
                    EnergyStationForAttendant = 2;
                    MaxEnergyGrowth = 100;
                    MinEnergyGrowth = 50;
                    MaxStationEnergy = 10000;
                    
                    StoleRateEnergyAtAttack = 0.05;
                    AttackEnergyLoss = 50;
                    break;
                case 4:
                    EnergyStationForAttendant = 2;
                    MaxEnergyGrowth = 30;
                    MinEnergyGrowth = 10;
                    CollectingDistance = 2;
                    
                    StoleRateEnergyAtAttack = 0.1;
                    AttackEnergyLoss = 30;
                    break;
                case 5:
                    EnergyStationForAttendant = 5;
                    MaxEnergyGrowth = 40;
                    MinEnergyGrowth = 20;
                    
                    StoleRateEnergyAtAttack = 0.05;
                    AttackEnergyLoss = 50;
                    CollectingDistance = 1;
                    EnergyLossToCreateNewRobot = 100;
                    break;
                case 6:
                    EnergyStationForAttendant = 5;
                    MaxEnergyGrowth =  100;
                    MinEnergyGrowth = 50;
                    MaxStationEnergy = 20000;
                    CollectingDistance = 2;
                    MaxEnergyCanCollect = 40;
                    EnergyLossToCreateNewRobot = 50;
                    StoleRateEnergyAtAttack = 0.1;
                    AttackEnergyLoss = 20;
                    break;
                case 7:
                    EnergyStationForAttendant = 2;
                    MaxEnergyGrowth =  100;
                    MinEnergyGrowth = 50;
                    CollectingDistance = 3;
                    
                    
                    EnergyLossToCreateNewRobot = 50;

                    break;
                case 8:
                    EnergyStationForAttendant = 2;
                    MaxEnergyGrowth =  30;
                    MinEnergyGrowth = 10;
                    StoleRateEnergyAtAttack = 0.05;
                    AttackEnergyLoss = 30;
                    CollectingDistance = 2;
                    MaxEnergyCanCollect = 300;
                    break;
                default:
                    throw new Exception("Not supported variant");
                    
            }

        }
    }
}
