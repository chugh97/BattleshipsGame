using System;

namespace Battleships.Game
{
    /// <summary>
    /// Stores all information about each cell on the game board
    /// </summary>
    public class Cell
    {
        public int PosX { get; set; }
        public int PosY { get; set; }

        /// <summary>
        /// Display name is of format A1, J10
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Depicts if a cell has a ship on it
        /// </summary>
        public bool HasShip { get; set; }

        /// <summary>
        /// Depicts if a ship on the cell has been hit by player
        /// </summary>
        public bool IsHit { get; set; }

        /// <summary>
        /// Type of Ship Battleship or Destoyer in our case
        /// </summary>
        public Type ShipType { get; set; }

        /// <summary>
        /// Depicts if a cell has no Ship (so it's sea) - IsSea is then true else IsSea is false if there is a ship
        /// </summary>
        public bool IsSea { get; set; }
    }
}