namespace Battleships.Game
{
    /// <summary>
    /// To Store Game Board Coordinates A1 to I10
    /// </summary>
    public class Coordinate
    {
        /// <summary>
        /// Letter - e.g. A-J
        /// </summary>
        public string Letter { get; set; }
        
        /// <summary>
        /// Number e.g 1 to 10
        /// </summary>
        public int Number { get; set; }
    }
}