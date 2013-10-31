using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Battleships.Game
{
    /// <summary>
    /// This is the GameBoard class which is created for each player for the Game. This class sets the Ships on Random and the game is played.
    /// </summary>
    public class GameBoard
    {
        /// <summary>
        /// Fixed Size on Board as per specification
        /// </summary>
        private const int BOARD_SIZE = 10;
        /// <summary>
        /// Cell Lettering constants
        /// </summary>
        private string[] boardChars = new string[] {"A", "B", "C", "D", "E", "F", "G", "H", "I", "J"};
        
        /// <summary>
        /// No of Destroyer Ships as per spec
        /// </summary>
        private const int NO_OF_DESTROYERS = 2;

        /// <summary>
        /// No of Battleships as per spec
        /// </summary>
        private const int NO_OF_BATTLESHIPS = 1;

        private const string COMPUTER = "Computer";

        /// <summary>
        /// Constructor takes the player and initialises the GameBoard
        /// </summary>
        /// <param name="player"></param>
        public GameBoard(string player)
        {
            Player = player;
            AssignShipsToGame();
        }

        /// <summary>
        /// Property used to store the Board information 
        /// </summary>
        public Board Board { get; private set; }

        /// <summary>
        /// The player for this Game Board
        /// </summary>
        public string Player { get; private set; }

        /// <summary>
        /// Used temporarily to store the potential cell locations 
        /// </summary>
        private List<Cell> TempCells { get; set; }

        /// <summary>
        /// Last Move played by a player.
        /// </summary>
        private GameMove LastMove { get; set; }

        /// <summary>
        /// Ships for game as a dictionary of type of ship and no of ships of that type
        /// </summary>
        private ShipsForGame<Type,int> ShipsForGame { get; set; }

        /// <summary>
        /// Dictionary to alert ship sunk status
        /// </summary>
        private Dictionary<Guid, Tuple<Ship,List<Cell>>> ShipSunkStatus { get; set; }

        /// <summary>
        /// Method to layout the Ships inrandom on the board
        /// </summary>
        public void SetUpShips()
        {
            CreateBlankBoard();
            foreach (var ship in ShipsForGame)
            {
                var shipType = ship.Key;
                var noOfShips = ship.Value;
                for (int i = 0; i < noOfShips; i++)
                {
                    AddShipToBoard(shipType);
                    TempCells = null;
                }
            }
        }

        /// <summary>
        /// Determines if Game is Over
        /// </summary>
        /// <returns></returns>
        public bool IsGameOver()
        {
            if (IsWinner())
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Play Move by Computer
        /// </summary>
        /// <returns>GameMove</returns>
        public GameMove PlayMove()
        {
            LastMove = new GameMove();
            if (Player.ToLowerInvariant() == COMPUTER.ToLowerInvariant())
            {
                var randomCell = RandomiseCellForComputerToPlay();
                LastMove = PlayMove(randomCell.DisplayName);
                LastMove.TimeMovePlayed = DateTime.UtcNow;
                System.Threading.Thread.Sleep(500);
            }
            return LastMove;
        }

        /// <summary>
        /// Player Move
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public GameMove PlayMove(string input)
        {
            LastMove = new GameMove();
            LastMove.Player = Player;
            LastMove.Move = input;
            var cellToPlay = (from c in Board.Cells
                              where c.Key.ToLowerInvariant() == input.ToLowerInvariant()
                              select c).FirstOrDefault();

            if (cellToPlay.Value == null) //cannot find cell on board
            {
                LastMove.Message = "Invalid Move";
                LastMove.Status = MoveStatus.Invalid;
            }
            else
            {
                if (cellToPlay.Value.IsSea || cellToPlay.Value.IsHit)
                {
                    LastMove.Status = MoveStatus.HasAlreadyBeenPlayed;
                    LastMove.Message = "This Move has been alreay played....";

                }
                else if (cellToPlay.Value.HasShip)
                {
                    cellToPlay.Value.IsHit = true;
                    LastMove.Status = MoveStatus.Success;
                    LastMove.Message = "You hit a ship!";

                    bool hasSunkAShip = CheckIfPlayerHasSunkAShip(input);
                    if (hasSunkAShip)
                    {
                        LastMove.Message += ", Also You have sunk it!!!!!!";
                    }
                }
                else
                {
                    cellToPlay.Value.IsSea = true;
                    LastMove.Status = MoveStatus.Failure;
                    LastMove.Message = "You hit Sea! Better luck next time";
                }
            }
            LastMove.TimeMovePlayed = DateTime.UtcNow;
            return LastMove;
        }

        /// <summary>
        /// Check if a Ship has been sunk and notify UI accordingly
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private bool CheckIfPlayerHasSunkAShip(string input)
        {
            var result = false;
            foreach (var item in ShipSunkStatus)
            {
                var tuple = item.Value;
                var ship = tuple.Item1;
                var cells = tuple.Item2;

                var cellAttacked = cells.FirstOrDefault(x => x.DisplayName == input);

                if (cellAttacked != null)
                {
                    var cellsHitCount = cells.Count(x => x.HasShip && x.IsHit);
                    if (cellsHitCount ==  ship.Size)
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Paints the console with uptodate moves and board information 
        /// </summary>
        public void DisplayBoard()
        {
            if (Player.ToLowerInvariant() != COMPUTER.ToLowerInvariant())
            {
                Console.WriteLine("|**| - Bombed Ship, |XX| - Sea , |A1|- Cell ready to strike");
            }
            Console.WriteLine("");
            Console.WriteLine("-----------------Game Board: " + Player + "--------------------------");
            for (int i = 0; i < boardChars.Length; i++)
            {
                Console.WriteLine("");
                for (int j = 0; j < BOARD_SIZE; j++)
                {
                    var key = string.Format("{0}{1}", boardChars[i], j + 1);
                    var cell = GetCellByKey(key);
                    if (cell.HasShip)
                    {
                        if (cell.IsHit)
                        {
                            SendTextToConsole("|**|", ConsoleColor.Red);
                        }
                        else
                        {
                            if (cell.ShipType == typeof(Destroyer))
                            {
                                SendTextToConsole("|" + key + "|", ConsoleColor.Gray);
                                
                                //SendTextToConsole("|DD|", ConsoleColor.Gray); //remove comment to see hidden ship
                            }
                            else if (cell.ShipType == typeof(BattleShip))
                            {
                                SendTextToConsole("|" + key + "|", ConsoleColor.Gray);
                                //SendTextToConsole("|BB|", ConsoleColor.Gray);  //remove comment to see hidden ship
                            }
                        }
                    }
                    else
                    {
                        if (cell.IsSea)
                        {
                            SendTextToConsole("|XX|", ConsoleColor.Cyan);
                        }
                        else
                        {
                            SendTextToConsole("|" + key + "|", ConsoleColor.Gray);
                        }
                    }
                }
            }
            Console.WriteLine("");
        }

        /// <summary>
        /// Assigns ships to the game
        /// </summary>
        private void AssignShipsToGame()
        {
            var ships = new ShipsForGame<Type, int>();
            ships.Add(typeof(BattleShip), NO_OF_BATTLESHIPS);
            ships.Add(typeof(Destroyer), NO_OF_DESTROYERS);
            ShipsForGame = ships;
        }

        /// <summary>
        /// Creates a blank new board without the ships (Clean initial state)
        /// </summary>
        private void CreateBlankBoard()
        {
            var board = new Board { Cells = new Dictionary<string, Cell>() };

            for (int i = 0; i < boardChars.Length; i++)
            {
                for (int j = 0; j < BOARD_SIZE; j++)
                {
                    var key = string.Format("{0}{1}", boardChars[i], j + 1);
                    board.Cells.Add(key, new Cell { DisplayName = key, HasShip = false, IsHit = false, IsSea = false, PosX = i, PosY = j });
                }
            }
            Board = board;
        }

        /// <summary>
        /// Method used to place ship on board
        /// </summary>
        /// <param name="type"></param>
        private void AddShipToBoard(Type type)
        {
            bool result = false;
            while (result == false)
            {
                var randomStartPoint = RandomiseStartValue();
                var ship = CreateInstanceOfShipfromType(type);
                if (randomStartPoint != null)
                {
                    randomStartPoint.ShipType = type;
                    result = TryAddingShipToBoard(randomStartPoint, ship);
                    if (result)
                    {
                        AddShipToCellsOnBoard(ship);
                    }
                }
            }
        }

        /// <summary>
        /// Creates a new instance of a ship from Type provided
        /// </summary>
        /// <param name="shipType"></param>
        /// <returns></returns>
        private Ship CreateInstanceOfShipfromType(Type shipType)
        {
            Type type = shipType;
            Object obj = Activator.CreateInstance(type);
            return (Ship)obj;
        }

        /// <summary>
        /// Method called once the TempCells list is validated that the ship can fit on the cells in the list
        /// </summary>
        private void AddShipToCellsOnBoard(Ship ship)
        {
            var list = TempCells;
            var fistCell = list.FirstOrDefault();

            foreach (var cell in list)
            {
                var displayName = cell.DisplayName;

                var cellFromBoard = (from c in Board.Cells
                                     where c.Key == displayName
                                     select c.Value).FirstOrDefault();

                if (cellFromBoard != null)
                {
                    if (fistCell != null)
                    {
                        cellFromBoard.ShipType = fistCell.ShipType;
                    }
                    cellFromBoard.HasShip = true;
                }
            }
            
            if (ShipSunkStatus == null)
            {
                ShipSunkStatus = new Dictionary<Guid, Tuple<Ship, List<Cell>>>();
            }

            ShipSunkStatus.Add(ship.ShipName, Tuple.Create(ship, TempCells));

            TempCells = null;
        }

        /// <summary>
        /// Using a random staring point to try determing if a ship can be added on certain cells on the board
        /// </summary>
        /// <param name="randomStartPoint"></param>
        /// <param name="ship"></param>
        /// <returns></returns>
        private bool TryAddingShipToBoard(Cell randomStartPoint, Ship ship)
        {
            TempCells = null;
            List<Cell> tempCells = new List<Cell>();
            tempCells.Add(randomStartPoint);

            //try increasing
            List<Cell> cellsAfterDirection1 = null;
            var size = ship.Size;
            for (int i = 1; i < size; i++)
            {
                cellsAfterDirection1 = TryDirection1(tempCells);
            }

            var result = CheckIfPotentialDirectionFeasible(cellsAfterDirection1, size);

            if (result)
            {
                TempCells = cellsAfterDirection1;
            }

            List<Cell> cellsAfterDirection2 = null;
            if (result == false)
            {
                TempCells = null;
                TempCells = tempCells = new List<Cell>();
                tempCells.Add(randomStartPoint);

                for (int i = 1; i < size; i++)
                {
                    cellsAfterDirection2 = TryDirection2(tempCells);
                }

                result = CheckIfPotentialDirectionFeasible(cellsAfterDirection2, size);

                if (result)
                {
                    TempCells = cellsAfterDirection2;
                }

            }
            return result;
        }

        /// <summary>
        /// Takes astring A1 and returns a co-ordinate class with Letter: A, Number: 1
        /// </summary>
        /// <param name="displayName"></param>
        /// <returns></returns>
        private Coordinate SplitDisplayName(string displayName)
        {
            var regExp = new Regex("[A-J]", RegexOptions.IgnoreCase);
            var match = regExp.Match(displayName);
            var regExp2 = new Regex("[0-9]");
            var match2 = regExp2.Match(displayName);
            var letter = match.Value;
            var num = Convert.ToInt32(match2.Value);
            return new Coordinate { Letter = letter, Number = num };
        }

        /// <summary>
        /// Checks to see if a potential direction is suitable for adding the ship
        /// </summary>
        /// <param name="cellsAfterDirection1"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private bool CheckIfPotentialDirectionFeasible(List<Cell> cellsAfterDirection1, int size)
        {
            bool result = false;
            if (cellsAfterDirection1 == null)
            {
                result = false;
            }
            else if (cellsAfterDirection1.Count != size)
            {
                result = false;
            }
            else
            {
                result = (from c in cellsAfterDirection1
                          where c.HasShip == false
                          select c).Any();
            }

            return result;
        }

        /// <summary>
        /// Tries Direction on right
        /// </summary>
        /// <param name="tempCells"></param>
        /// <returns></returns>
        private List<Cell> TryDirection1(List<Cell> tempCells)
        {
            var lastCell = tempCells.LastOrDefault();
            var lastCoordinate = SplitDisplayName(lastCell.DisplayName);

            if (lastCoordinate.Number >= BOARD_SIZE - 1)
            {
                return tempCells;
            }

            var cell = GetCellByKey(lastCoordinate.Letter + (lastCoordinate.Number + 1).ToString());
            if (cell != null)
            {
                tempCells.Add(cell);
            }

            return tempCells;
        }

        /// <summary>
        /// Tries the Left Direction
        /// </summary>
        /// <param name="tempCells"></param>
        /// <returns></returns>
        private List<Cell> TryDirection2(List<Cell> tempCells)
        {
            var lastCell = tempCells.LastOrDefault();
            var lastCoordinate = SplitDisplayName(lastCell.DisplayName);


            if (lastCoordinate.Number <= 0)
            {
                return tempCells;
            }

            var cell = GetCellByKey(lastCoordinate.Letter + (lastCoordinate.Number - 1).ToString());
            if (cell != null)
            {
                tempCells.Add(cell);
            }

            return tempCells;
        }

        /// <summary>
        /// Helper method to get Cell by Key e.g A6, Gets the Cell info
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private Cell GetCellByKey(string key)
        {
            var cell = (from c in Board.Cells
                        where c.Key == key
                        select c.Value).FirstOrDefault();
            return cell;
        }

        /// <summary>
        /// Using random (Simple .NET Random class) to get a random cell on the board to place the ships
        /// </summary>
        /// <returns></returns>
        private Cell RandomiseStartValue()
        {
            var result = true;
            Cell potentialCell = null;
            
            while (result)
            {
                Random r = new Random();
                int index = r.Next(boardChars.Count());
                var letter = boardChars[index];

                var rNumber = r.Next(1, BOARD_SIZE);
                var randomCell = letter + rNumber;

                potentialCell = (from c in Board.Cells
                                 where c.Key == randomCell
                                 select c.Value).FirstOrDefault();

                if (!potentialCell.HasShip)
                {
                    result = false;
                }
            }

            return potentialCell;
        }

        /// <summary>
        /// Randomises cell for Computer game play
        /// </summary>
        /// <returns></returns>
        private Cell RandomiseCellForComputerToPlay()
        {
            var result = true;
            Cell potentialCell = null;

            while (result)
            {
                Random r = new Random();
                int index = r.Next(boardChars.Count());
                var letter = boardChars[index];

                var rNumber = r.Next(1, BOARD_SIZE);
                var randomCell = letter + rNumber;

                potentialCell = (from c in Board.Cells
                                 where c.Key == randomCell
                                 select c.Value).FirstOrDefault();
                
                if (potentialCell.IsSea)
                {
                    result = true;
                }
                else if (potentialCell.IsHit)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            return potentialCell;
        }
        
        /// <summary>
        /// Sends Text to Console
        /// </summary>
        /// <param name="content"></param>
        /// <param name="colorToSet"></param>
        public void SendTextToConsole(string content, ConsoleColor colorToSet)
        {
            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = colorToSet;
            Console.Write(content);
            Console.ForegroundColor = originalColor;
        }

        /// <summary>
        /// Determines if the Player has hit all cells with ships
        /// </summary>
        /// <returns></returns>
        private bool IsWinner()
        {
            var count = 0;
            foreach (var ship in ShipsForGame)
            {
                var noOfShips = ship.Value;
                var shipType = ship.Key;
                var shipInstance = CreateInstanceOfShipfromType(shipType);
                count += noOfShips * shipInstance.Size;
            }
            return (from c in Board.Cells
                    where c.Value.HasShip && c.Value.IsHit
                    select c).Count() == count;
        }
    }
}