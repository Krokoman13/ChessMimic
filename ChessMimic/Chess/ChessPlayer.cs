using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    public abstract class ChessPlayer
    {
        protected Board _board;
        protected int _color;

        public ChessPlayer(int color)
        {
            _color = color;
        }

        public void SetBoard(Board board)
        {
            _board = board;
        }

        public abstract Move MakeMove();

        public abstract string PlayerType();
    }
}
