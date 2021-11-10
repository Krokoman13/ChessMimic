using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    class GameHandeler
    {
        Board _board;

        ChessPlayer _whitePlayer;
        ChessPlayer _blackPlayer;

        int moves = 0;

        public GameHandeler(ChessPlayer white, ChessPlayer black)
        {
            _board = new Board();

            white.SetBoard(_board);
            black.SetBoard(_board);

            _whitePlayer = white;
            _blackPlayer = black;
        }

        public void Run()
        {
            Console.WriteLine();
            _board.Display();
            Console.WriteLine(_board.GenerateRealFenString());
            Console.WriteLine();

            while (!_board.gameOver)
            {
                step();
            }

            Console.WriteLine("Game over");

            if (_board.winner == Piece.Black) Console.WriteLine("CheckMate, Black Wins");
            else if (_board.winner == Piece.White) Console.WriteLine("CheckMate, White Wins");
            else Console.WriteLine("Draw");

            Console.WriteLine(_board.GenerateRealFenString());
        }

        private void step()
        {
            Move move;

            if (_board.whiteToMove)
            {
                moves++;
                move = _whitePlayer.MakeMove();
            }
            else
            { 
                move = _blackPlayer.MakeMove();
            }

            _board.ExecuteMove(move);
            _board.Display();

            Console.WriteLine();
            Console.WriteLine(moves + (_board.whiteToMove ? " Black" : " White") + ": " + move.RevAlgebraic());
            Console.WriteLine("Fen: " + _board.GenerateRealFenString());
            Console.WriteLine();
        }
    }
}
