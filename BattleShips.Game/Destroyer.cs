namespace Battleships.Game
{
    /// <summary>
    /// Type of ship is a Destroyer
    /// </summary>
    public class Destroyer : Ship
    {
        private const int SIZE = 4;

        public Destroyer()
        {
        }

        /// <summary>
        /// Size of Ship is 4 Cells on the Board
        /// </summary>
        public override int Size
        {
            get
            {
                return SIZE;
            }
        }
    }
}