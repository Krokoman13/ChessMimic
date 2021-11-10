
namespace Chess
{
    public class Square
    {
        public int piece;

        private Board board;
        private Coordinate cord;

        public Coordinate coordinate
        {
            get { return cord; }
        }

        public Square(Board board, Coordinate cord, int piece = Piece.None)
        {
            this.board = board;
            this.cord = cord;
            this.piece = piece;
        }

        public Square()
        {
            this.board = null;
            this.cord = new Coordinate();
            this.piece = Piece.None;
        }
    }
}
