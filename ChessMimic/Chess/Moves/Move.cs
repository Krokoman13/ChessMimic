
namespace Chess
{
    public class Move
    {
        public Coordinate originalPosition;
        public Coordinate targetPosition;

        public Move(int x1, int y1, int x2, int y2) : this(new Coordinate(x1, y1), new Coordinate(x2, y2))
        {
        }

        public Move(string move) : this(split(move, 0), split(move, 1))
        {
        }

        public Move(string original, string target) : this(new Coordinate(original), new Coordinate(target))
        {
        }

        public Move(Coordinate origin, Coordinate target)
        {
            this.originalPosition = origin;
            this.targetPosition = target;
        }

        public bool valid
        {
            get { return originalPosition != targetPosition; }
        }

        public virtual string RevAlgebraic()
        {
            string outp = originalPosition.Notation() + "-" + targetPosition.Notation();
            return outp;
        }

        static private string split(string inp, int index)
        {
            string[] sections = inp.Split('-');

            if (sections.Length != 2)
            {
                sections = new string[2] { inp.Substring(0, 2), inp.Substring(2, 2) };
            }

            return sections[index];
        }

        public override string ToString()
        {
            return RevAlgebraic();
        }

        public static Move invalid
        {
            get { return new Move(-1, -1, -1, -1); }
        }
    }
}
