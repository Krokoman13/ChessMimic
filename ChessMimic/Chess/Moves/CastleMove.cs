using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    public class CastleMove : Move
    {
        public Coordinate kingCord
        {
            get { return this.originalPosition; }
        }

        public Coordinate rookCord
        {
            get { return this.targetPosition; }
        }

        public CastleMove(int kingX, int kingY, int rookX, int rookY) : base(kingX, kingY, rookX, rookY)
        {
        }

        public CastleMove(string notation, int color) : base(0, 0, 0, 0)
        {
            if (notation == "O-O")  //Casteling Kingside 
            {
                if (color == Piece.White)
                {
                    originalPosition = new Coordinate(4, 0);
                    targetPosition = new Coordinate(7, 0);
                }
                else
                {
                    originalPosition = new Coordinate(4, 7);
                    targetPosition = new Coordinate(7, 7);
                }
            }
            else if (notation == "O-O-O")  //Casteling QueenSide
            {
                if (color == Piece.White)
                {
                    originalPosition = new Coordinate(4, 0);
                    targetPosition = new Coordinate(0, 0);
                }
                else
                {
                    originalPosition = new Coordinate(4, 7);
                    targetPosition = new Coordinate(0, 7);
                }
            }
        }

        public CastleMove(string king, string rook) : base(king, rook)
        {
        }

        public CastleMove(Coordinate king, Coordinate rook) : base(king, rook)
        {
        }

        public override string RevAlgebraic()
        {
            if (rookCord.x < kingCord.x) return "O-O-O";
            else return "O-O";
        }
    }
}
