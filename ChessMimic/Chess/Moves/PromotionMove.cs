using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    public class PromotionMove : Move
    {
        public int toPromoteTo;

        public PromotionMove(Coordinate startpos, Coordinate endPos, int piece) : base(startpos, endPos)
        {
            toPromoteTo = piece;
        }

        public override string RevAlgebraic()
        {
            return base.RevAlgebraic() + "Promote to: " + Piece.DisplayChar(toPromoteTo);
        }
    }
}
