using System.Collections.Generic;

namespace Battleships.Game
{
    /// <summary>
    /// Board Class which is a dictionary of string, cell A1, Cell Info for that cell
    /// </summary>
    public class Board
    {
        /// <summary>
        /// Cells on the Board. Typically in thi of case consists of dictionary with 100 keys from A1..A10,B1..B10, till J1-J10
        /// </summary>
        public Dictionary<string,Cell> Cells { get; set; }
    }
}