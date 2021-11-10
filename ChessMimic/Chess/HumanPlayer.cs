
using System;

namespace Chess
{
    class HumanPlayer : ChessPlayer
    {
        public HumanPlayer(int color) : base(color)
        {
        }

        public override Move MakeMove()
        {
            Move move = Move.invalid;

            while (!move.valid)
            {
                Console.WriteLine("Please enter a valid move (Chess notation): ");
                move = _board.GenerateMove(Console.ReadLine());
            }

            return move;
        }
    }
}
