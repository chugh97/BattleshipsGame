namespace Battleships.Game
{
    /// <summary>
    /// Represents a battle ship which takes a size of 5 Cells on the board
    /// </summary>
    public class BattleShip : Ship
    {
        private const int SIZE = 5;

        public BattleShip()
        {
        }

        /// <summary>
        /// Size of Battle ship
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