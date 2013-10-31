using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Battleships.Game.Tests
{
    [TestClass]
    public class BattleShipGameBoardTests
    {
        [TestMethod]
        public void Are_Ships_Set_Up_On_Board_Correctly_OK()
        {
            var gameBoard = new GameBoard("Tom");
            gameBoard.SetUpShips();
            var cells = gameBoard.Board.Cells;
            Assert.AreEqual(100, cells.Count);
            Assert.AreEqual(13, cells.Count(x => x.Value.HasShip));
        }

        [TestMethod]
        public void Play_Ten_Valid_Moves_And_Check_If_Moved_played_Ok()
        {
            var gameBoard = new GameBoard("Tom2");

            gameBoard.SetUpShips();
            for (int i = 1; i < 11; i++)
            {
                gameBoard.PlayMove("A" + i.ToString());
            }
            
            var cells = gameBoard.Board.Cells;
            Assert.AreEqual(100, cells.Count);
            Assert.AreEqual(13, cells.Count(x => x.Value.HasShip));
            Assert.AreEqual(10, cells.Count(x => x.Value.IsSea || x.Value.IsHit));
        }

        [TestMethod]
        public void Play_Three_InValid_Moves_And_Check()
        {
            var gameBoard = new GameBoard("Tom2");

            gameBoard.SetUpShips();
            gameBoard.PlayMove("K10");
            gameBoard.PlayMove("A");
            gameBoard.PlayMove("A1]");

            var cells = gameBoard.Board.Cells;
            Assert.AreEqual(100, cells.Count);
            Assert.AreEqual(13, cells.Count(x => x.Value.HasShip));
            Assert.AreEqual(0, cells.Count(x => x.Value.IsSea));
        }

        [TestMethod]
        public void Play_Move_On_Cell_With_Ship_And_Check()
        {
            var gameMoves = new List<GameMove>();
            var gameBoard = new GameBoard("Tom2");

            gameBoard.SetUpShips();

            var cellsWhichHaveShipsAndNotHit = (from c in gameBoard.Board.Cells
                                                 where c.Value.HasShip && !c.Value.IsHit
                                                 select c.Value).ToList();

            foreach (var cell in cellsWhichHaveShipsAndNotHit)
            {
                var gameMove = gameBoard.PlayMove(cell.DisplayName);
                gameMoves.Add(gameMove);
            }

            
            var cells = gameBoard.Board.Cells;
            Assert.AreEqual(100, cells.Count);
            Assert.AreEqual(13, cells.Count(x => x.Value.HasShip));
            Assert.AreEqual(0, cells.Count(x => x.Value.IsSea));
            Assert.AreEqual(13, cells.Count(x => x.Value.HasShip && x.Value.IsHit));
            Assert.AreEqual(13, gameMoves.Count(y=> y.Status == MoveStatus.Success));
        }

        [TestMethod]
        public void Play_Move_On_Cell_With_Ship_Which_Has_Been_Hit_And_Check()
        {
            var gameBoard = new GameBoard("Tom2");

            gameBoard.SetUpShips();

            var cellsWhichHaveShipsAndNotHit = (from c in gameBoard.Board.Cells
                                                where c.Value.HasShip && !c.Value.IsHit
                                                select c.Value).ToList();

            gameBoard.PlayMove(cellsWhichHaveShipsAndNotHit.First().DisplayName);

            //Play again on same cell
            var move = gameBoard.PlayMove(cellsWhichHaveShipsAndNotHit.First().DisplayName);

            var cells = gameBoard.Board.Cells;
            Assert.AreEqual(100, cells.Count);
            Assert.AreEqual(13, cells.Count(x => x.Value.HasShip));
            Assert.AreEqual(0, cells.Count(x => x.Value.IsSea));
            Assert.AreEqual(1, cells.Count(x => x.Value.HasShip && x.Value.IsHit));
            Assert.AreEqual(MoveStatus.HasAlreadyBeenPlayed, move.Status);
        }


        [TestMethod]
        public void Strike_All_Cells_With_Ships_and_Check_If_Game_Is_Over()
        {
            var gameMoves = new List<GameMove>();
            var gameBoard = new GameBoard("Tom2");

            gameBoard.SetUpShips();

            var cellsWhichHaveShipsAndNotHit = (from c in gameBoard.Board.Cells
                                                where c.Value.HasShip && !c.Value.IsHit
                                                select c.Value).ToList();

            foreach (var cell in cellsWhichHaveShipsAndNotHit)
            {
                var gameMove = gameBoard.PlayMove(cell.DisplayName);
                gameMoves.Add(gameMove);
            }

            var cells = gameBoard.Board.Cells;

            var gameOver = gameBoard.IsGameOver();

            Assert.AreEqual(true, gameOver);
            Assert.AreEqual(100, cells.Count);
            Assert.AreEqual(13, cells.Count(x => x.Value.HasShip));
            Assert.AreEqual(0, cells.Count(x => x.Value.IsSea));
            Assert.AreEqual(13, cells.Count(x => x.Value.HasShip && x.Value.IsHit));
            Assert.AreEqual(13, gameMoves.Count(y => y.Status == MoveStatus.Success));
        }

        [TestMethod]
        public void Computer_Plays_A_Move()
        {
            var gameMoves = new List<GameMove>();
            var gameBoard = new GameBoard("Computer");

            gameBoard.SetUpShips();

            var cells = gameBoard.Board.Cells;

            gameBoard.PlayMove();
           
            Assert.AreEqual(100, cells.Count);
            Assert.AreEqual(13, cells.Count(x => x.Value.HasShip));
            Assert.AreEqual(0, cells.Count(x => x.Value.IsSea));
            Assert.AreEqual(1, cells.Count(x => x.Value.IsSea == true || x.Value.IsHit == true));
        }

        [TestMethod]
        public void Strike_Some_Cells_With_Ships_and_Check_If_Game_Is_Over()
        {
            var gameMoves = new List<GameMove>();
            var gameBoard = new GameBoard("Tom2");

            gameBoard.SetUpShips();

            var cellsWhichHaveShipsAndNotHit = (from c in gameBoard.Board.Cells
                                                where c.Value.HasShip && !c.Value.IsHit
                                                select c.Value).Take(12).ToList();


            foreach (var cell in cellsWhichHaveShipsAndNotHit)
            {
                var gameMove = gameBoard.PlayMove(cell.DisplayName);
                gameMoves.Add(gameMove);
            }

            var cells = gameBoard.Board.Cells;

            var gameOver = gameBoard.IsGameOver();

            Assert.AreEqual(false, gameOver);
            Assert.AreEqual(100, cells.Count);
            Assert.AreEqual(13, cells.Count(x => x.Value.HasShip));
            Assert.AreEqual(0, cells.Count(x => x.Value.IsSea));
            Assert.AreEqual(12, cells.Count(x => x.Value.HasShip && x.Value.IsHit));
            Assert.AreEqual(12, gameMoves.Count(y => y.Status == MoveStatus.Success));
        }



    }
}
