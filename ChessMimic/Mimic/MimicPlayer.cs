using Chess;
using System;

namespace Mimic
{
    public class MimicPlayer : ChessPlayer
    {
        const string white = "White";
        const string black = "Black";
        const string instructionsPath = "Instructions/";

        string[] winningInstructions;
        string[] otherInstructions;

        public MimicPlayer(int color = Piece.Black) : base (color)
        {
            winningInstructions = System.IO.File.ReadAllText(instructionsPath + (color == Piece.Black ? black : white) + "Win" + ".txt").Split('\n');
            otherInstructions = System.IO.File.ReadAllText(instructionsPath + (color == Piece.Black ? black : white) + ".txt").Split('\n');
        }

        public override Move MakeMove()
        {
            Move move = findMove();
            if (move.valid) return move;

            Console.WriteLine(_board.GenerateRealFenString());
            throw new ArgumentException("Cannot find a valid Move");
        }

        public Move findMove()
        {
            if (_board == null) return null;

            string fen1 = _board.GenerateRealFenString();

            for (int i = 0; i < winningInstructions.Length; i += 3)
            {
                string fen2 = winningInstructions[i];

                if (fen1 == fen2)
                {
                    string move = winningInstructions[i + 1];
                    if (move[0] == 'O') return new CastleMove(move, _color);
                    if (move[move.Length -1] == '#') _board.CheckMate(_color);
                    return new Move(move);
                }
            }

            for (int i = 0; i < otherInstructions.Length; i += 3)
            {
                string fen2 = otherInstructions[i];

                if (fen1 == fen2)
                {
                    string move = otherInstructions[i + 1];
                    if (move[0] == 'O') return new CastleMove(move, _color);
                    if (move[move.Length - 1] == '#') _board.CheckMate(_color);
                    return new Move(move);
                }
            }

            return Move.invalid;   //Return an invalid move
        }
    }
}
