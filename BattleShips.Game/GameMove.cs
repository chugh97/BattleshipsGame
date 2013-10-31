using System;

namespace Battleships.Game
{
    /// <summary>
    /// Encapsulates a datastructure for the Game Move
    /// </summary>
    public class GameMove
    {
        /// <summary>
        /// The Player
        /// </summary>
        public string Player { get; set; }
        
        /// <summary>
        /// Move A1..I10
        /// </summary>
        public string Move { get; set; }

        /// <summary>
        /// Any Message Associated with Move
        /// </summary>
        public string Message { get; set; }
        
        /// <summary>
        /// Status like Success or Invalid etc
        /// </summary>
        public MoveStatus Status { get; set; }

        /// <summary>
        /// Datetime the Move was played
        /// </summary>
        public DateTime? TimeMovePlayed { get; set; }
    }
}
