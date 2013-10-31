namespace Battleships.Game
{
    public enum MoveStatus
    {
        /// <summary>
        /// If a Move is played successfully and hit a Ship
        /// </summary>
        Success = 0,
        /// <summary>
        /// If a move is played on Sea Cell
        /// </summary>
        Failure,
        /// <summary>
        /// If a move has wrong input like A100 which does not exist on board
        /// </summary>
        Invalid,
        /// <summary>
        /// If move already played
        /// </summary>
        HasAlreadyBeenPlayed
    }
}