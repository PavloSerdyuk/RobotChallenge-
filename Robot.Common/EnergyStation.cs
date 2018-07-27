namespace Robot.Common
{
    public sealed class EnergyStation
    {


        /// <summary>
        /// How many energy is restored for the one full round. Full round is when all robots do a move. 
        /// </summary>
        public int RecoveryRate { get; set; }


        /// <summary>
        /// Station energy at this moment
        /// </summary>
        public int Energy { get; set; }

        /// <summary>
        /// Stationposition
        /// </summary>
        public Position Position { get; set; }


    }
}
