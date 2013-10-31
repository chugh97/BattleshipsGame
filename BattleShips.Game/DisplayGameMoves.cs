using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Battleships.Game
{
    public class DisplayGameMoves
    {
        /// <summary>
        /// Constant defining the Computer
        /// </summary>
        private const string COMPUTER = "COMPUTER";

        /// <summary>
        /// No of Last Game Moves to Display
        /// </summary>
        private const int NO_OF_MESSAGES_TO_DISPLAY = 2;


        /// <summary>
        /// Display Last 2 Moves
        /// </summary>
        /// <param name="gameMoves"></param>
        public static void DisplayLastTwoGameMoves(IList<GameMove> gameMoves)
        {
            if (gameMoves != null && gameMoves.Any())
            {
                var lastMoves = gameMoves.ToList().OrderByDescending(c => c.TimeMovePlayed).Take(NO_OF_MESSAGES_TO_DISPLAY).ToList().OrderBy(c => c.TimeMovePlayed);

                ConsoleColor originalColor = Console.ForegroundColor;
                foreach (var gameMove in lastMoves)
                {
                    Console.ForegroundColor = originalColor;

                    if (gameMove.Player.ToLowerInvariant() == COMPUTER.ToLowerInvariant())
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    }
                    else
                    {
                        Console.ForegroundColor = originalColor;
                    }

                    if (gameMove.Status == MoveStatus.Success)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    else if (gameMove.Status == MoveStatus.Invalid)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                    }
                    else if (gameMove.Status == MoveStatus.HasAlreadyBeenPlayed)
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                    }

                    Console.WriteLine(string.Format("{0} {1}: {2}", " Move for ", gameMove.Player, gameMove.Move));
                    Console.WriteLine(string.Format("{0} {1}: {2}", " RESULT for ", gameMove.Player, gameMove.Message));
                }

                Console.ForegroundColor = originalColor;
            }
        }
    }
}
