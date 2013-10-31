using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Battleships.Game;

namespace Battleships
{
    class Program
    {
        private const string COMPUTER = "COMPUTER";
        private const string EXIT = "EXIT";
        private static IList<GameMove> _gameMoves = null;
        static void Main(string[] args)
        {
            Console.Title = "BattleShips Test for Bede Gaming - by Shaleen Chugh";
           
            PlayBattleShips();

            Console.ReadKey();
        }

        private static void PlayBattleShips()
        {
            _gameMoves = new List<GameMove>();

            try
            {
                Console.WriteLine("Please enter your name:");
                var player = Console.ReadLine();

                //Guard against User enetering himself as Computer as his name
                if (!string.IsNullOrEmpty(player) && player.ToLowerInvariant() == COMPUTER.ToLowerInvariant())
                {
                    player = "Player 1";
                }

                player = player ?? "Player 1";

                GameBoard gb = new GameBoard(player);
                gb.SetUpShips();
                gb.DisplayBoard();

                var result = true;

                GameBoard gb2 = new GameBoard(COMPUTER);
                gb2.SetUpShips();
                gb2.DisplayBoard();

                while (result)
                {
                    Console.WriteLine("Please make your move for eg. A1 to J10: ");

                    var input = Console.ReadLine();

                    if (input == string.Empty)
                    {
                        Console.WriteLine("Please enter a value in the form of A1 to A10 to J1 to J10");
                    }

                    if (input == EXIT)
                    {
                        result = false;
                    }

                    if (!string.IsNullOrEmpty(input) && result)
                    {
                        Console.Clear();

                        var playerMove = gb.PlayMove(input);
                        _gameMoves.Add(playerMove);

                        var computerMove = gb2.PlayMove();
                        _gameMoves.Add(computerMove);

                        DisplayGameBoards(gb, gb2);

                        bool isGameOver = false;

                        isGameOver = IsGameOver(gb, gb2);

                        DisplayGameMoves.DisplayLastTwoGameMoves(_gameMoves);

                        if (isGameOver)
                        {
                            Console.Clear();
                            DisplayGameBoards(gb, gb2);
                            DisplayGameMoves.DisplayLastTwoGameMoves(_gameMoves);
                            Console.WriteLine("Do you wish another challenge of Battleships? Y/N");
                            input = Console.ReadLine();
                            input = PlayAgainOrExitGame(input);
                        }
                    }

                    result = ExitGameAndDisplayThankYouMessage(input);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An erro ocuured");
                Debug.WriteLine("An error occured");
            }
        }

        private static bool ExitGameAndDisplayThankYouMessage(string input)
        {
            bool result = true;
            if (input == EXIT)
            {
                Console.Clear();
                Console.WriteLine("Thanks for Playing Shaleen Chugh's version of Battleship");
                result = false;
            }
            return result;
        }

        private static string PlayAgainOrExitGame(string input)
        {
            if (input != "Y" && input != "N")
            {
                do
                {
                    Console.WriteLine("Do you wish another challenge? Y/N");
                    input = Console.ReadLine();
                } while (input != "Y" && input != "N");
            }

            if (input == "Y")
            {
                PlayBattleShips();
            }
            else if (input == "N")
            {
                input = EXIT;
            }

            return input;
        }

        private static bool IsGameOver(GameBoard gb, GameBoard gb2)
        {
            bool isGameOver = false;
            if (gb.IsGameOver())
            {
                _gameMoves.Add(new GameMove() {Player = gb.Player, Message = "You WIN!", TimeMovePlayed = DateTime.UtcNow});
                isGameOver = true;
            }
            else if (gb2.IsGameOver())
            {
                _gameMoves.Add(new GameMove()
                                  {Player = gb2.Player, Message = "Computer WINS !", TimeMovePlayed = DateTime.UtcNow});
                isGameOver = true;
            }
            return isGameOver;
        }

        private static void DisplayGameBoards(GameBoard gb, GameBoard gb2)
        {
            gb.DisplayBoard();
            gb2.DisplayBoard();
        }
    }
}