namespace Chess
{
    using System;
    using System.Collections.Generic;

    public class Board
    {
        public const string startFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        public const int WhiteIndex = 0;
        public const int BlackIndex = 1;

        private bool _whiteToMove;
        private bool _gameOver = false;
        private int _winner = -1;

        public bool whiteToMove
        {
            get { return _whiteToMove; }
        }
        public bool gameOver
        {
            get { return _gameOver; }
        }
        public int winner
        {
            get 
            {
                if (_winner == WhiteIndex) return Piece.White;
                if (_winner == BlackIndex) return Piece.Black;
                return _winner;
            }
        }

        private const int boardSize = 8;

        public Square[] board;

        public Board()
        {
            Generate(startFen);
        }

        public void Generate(string fen)
        {
            board = new Square[boardSize * boardSize];

            for (int y = 0; y < boardSize; y++)
            {
                for (int x = 0; x < boardSize; x++)
                {
                    board[(y * boardSize) + x] = new Square(this, new Coordinate(x, y));
                }
            }

            string[] sections = fen.Split(' ');

            int file = 0;
            int rank = 7;

            foreach (char symbol in sections[0])
            {
                if (symbol == '/')
                {
                    file = 0;
                    rank--;
                }
                else
                {
                    if (char.IsDigit(symbol))
                    {
                        file += (int)char.GetNumericValue(symbol);
                    }
                    else
                    {
                        int piece = Piece.ToInt(symbol);
                        GetSquare(file, rank).piece = piece;
                        file++;
                    }
                }
            }

            _whiteToMove = (sections[1][0] == 'w');
        }

        public string GenerateRealFenString()
        {
            string fen = "";
            for (int rank = boardSize - 1; rank >= 0; rank--)
            {
                int numEmptyFiles = 0;
                for (int file = 0; file < 8; file++)
                {
                    int i = rank * 8 + file;
                    int piece = GetSquare(file, rank).piece;
                    if (piece != Piece.None)
                    {
                        if (numEmptyFiles != 0)
                        {
                            fen += numEmptyFiles;
                            numEmptyFiles = 0;
                        }
                        fen += Piece.DisplayChar(piece).ToString();
                    }
                    else
                    {
                        numEmptyFiles++;
                    }

                }
                if (numEmptyFiles != 0)
                {
                    fen += numEmptyFiles;
                }
                if (rank != 0)
                {
                    fen += '/';
                }
            }

            // Side to move
            fen += ' ';
            fen += (_whiteToMove) ? 'w' : 'b';
            return fen;
        }

        public void Display()
        {
            bool alternate = false;

            Console.Write("   ");
            for (int x = 0; x < (boardSize * 2); x++)
            {
                if (alternate) Console.Write("---");
                else Console.Write("+");
                alternate = !alternate;
            }

            Console.WriteLine("+");

            for (int y = boardSize - 1; y > -1; y--)
            {
                Console.Write(" " + Coordinate.YToChar(y).ToString() + " ");

                for (int x = 0; x < boardSize * 2; x++)
                {
                    if (alternate)
                    {
                        Console.Write(' ');
                        Console.Write(Piece.DisplayChar(GetSquare(x / 2, y).piece));
                        Console.Write(' ');
                    }
                    else Console.Write("|");
                    alternate = !alternate;
                };

                Console.WriteLine("|");
                Console.Write("   ");
                for (int x = 0; x < boardSize * 2; x++)
                {
                    if (alternate) Console.Write("---");
                    else Console.Write("+");
                    alternate = !alternate;
                }

                Console.WriteLine("+");
            }

            Console.Write("    ");
            for (int x = 0; x < boardSize; x++)
            {
                Console.Write(" " + Coordinate.XToChar(x).ToString() + "  ");
            }
            Console.WriteLine();
        }

        public Square GetSquare(int x, int y)
        {
            if (x < 0 || x > boardSize - 1 || y < 0 || y > boardSize - 1) return null;

            return board[x + y * boardSize];
        }

        public Square GetSquare(Coordinate coordinate)
        {
            return GetSquare(coordinate.x, coordinate.y);
        }

        public bool Contains(Square square, int piece)
        {
            if (square == null) return false;
            return square.piece == piece;
        }
        public bool Contains(int x, int y, int piece)
        {
            return Contains(GetSquare(x, y), piece);
        }
        public bool Contains(Coordinate coordinate, int piece)
        {
            return Contains(GetSquare(coordinate), piece);
        }

        public void ExecuteMove(Move move)
        {
            executeMove(move);
            _whiteToMove = !_whiteToMove;
        }

        private void executeMove(Move move)
        {
            if (!move.valid)
            {
                Display();
                throw new ArgumentException("Invalid move, Start and End positions are the same");
            }
            if (move is CastleMove)
            {
                CastleMove castle = (CastleMove)move;
                ExecuteCastle(castle.kingCord, castle.rookCord);
                return;
            }
            GetSquare(move.targetPosition).piece = GetSquare(move.originalPosition).piece;
            GetSquare(move.originalPosition).piece = Piece.None;
            if (move is PromotionMove)
            {
                PromotionMove promotion = (PromotionMove)move;
                GetSquare(move.originalPosition).piece = promotion.toPromoteTo;
            }
        }

        public void ExecuteCastle(Coordinate kingCord, Coordinate rookCord)
        {
            if (kingCord.x > rookCord.x)
            {
                executeMove(new Move(kingCord.x, kingCord.y, kingCord.x - 2, kingCord.y));
                executeMove(new Move(rookCord.x, rookCord.y, kingCord.x - 1, kingCord.y));
            }

            if (kingCord.x < rookCord.x)
            {
                executeMove(new Move(kingCord.x, kingCord.y, kingCord.x + 2, kingCord.y));
                executeMove(new Move(rookCord.x, rookCord.y, kingCord.x + 1, kingCord.y));
            }
        }

        public Move GenerateMove(string notation)
        {
            if (notation.Length < 2) return null;
            if (notation[notation.Length - 1] == '#')
            {
                CheckMate((_whiteToMove ? Piece.White : Piece.Black));
                return GenerateMove(notation.Substring(0, notation.Length - 1));
            }
            if (notation == "O-O" || notation == "O-O-O")  //Casteling Kingside 
            {
                return new CastleMove(notation, (_whiteToMove ? Piece.White : Piece.Black));
            }   
            if (notation.Length == 5 && notation[2] == '-') return new Move(notation);  //Reverse algabriac notation
            if (notation[notation.Length - 1] == '\r') return GenerateMove(notation.Substring(0, notation.Length - 1));  //Remove the + sign
            if (notation[notation.Length - 1] == '+') return GenerateMove(notation.Substring(0, notation.Length - 1));  //Remove the + sign
            if (char.IsLower(notation[0])) return GenerateMove("P" + notation);  //Define that it is a pawn move
            if (notation[notation.Length - 3] == 'x') //If the piece is capturing another piece
            {
                if (notation[0] == 'P') //If it is a pawn capturing, generate a move
                {
                    Coordinate endPos;
                    Coordinate startPos;

                    if (notation.Length == 5)
                    {
                        endPos = new Coordinate(notation.Substring(3, 2));
                        startPos = findCapturingPawnStartPos(Piece.Pawn, endPos, Coordinate.GetX(notation[1]));
                    }
                    else
                    {
                        endPos = new Coordinate(notation.Substring(2, 2));
                        startPos = findCapturingPawnStartPos(Piece.Pawn, endPos);
                    }

                    return new Move(startPos, endPos);
                }
                else if (notation.Length > 4) return GenerateMove(notation.Substring(0, notation.Length - 3) + notation.Substring(notation.Length - 2));
                else return GenerateMove(notation[0].ToString() + notation.Substring(notation.Length - 2));  //Remove the X
            }   
            if (notation.Length == 3)
            {
                Coordinate endPos = new Coordinate(notation.Substring(1, 2));
                Coordinate startPos = findStartPos(Piece.PieceType(Piece.ToInt(notation[0])), endPos);
                return new Move(startPos, endPos);
            }
            if (notation.Length == 4 && notation[2] == '=') //PawnPromotion
            {
                Coordinate endPos = new Coordinate(notation.Substring(0, 2));
                Coordinate startPos = findStartPos(Piece.Pawn, endPos);
                return new PromotionMove(startPos, endPos, Piece.ToInt(notation[3]));
            }
            if (notation.Length == 4)
            {
                Coordinate endPos = new Coordinate(notation.Substring(2, 2));

                Coordinate startPos;
                if (char.IsDigit(notation[1])) startPos = findStartPosY(Piece.PieceType(Piece.ToInt(notation[0])), endPos, Coordinate.GetY(notation[1]));
                else startPos = findStartPosX(Piece.PieceType(Piece.ToInt(notation[0])), endPos, Coordinate.GetX(notation[1]));
                return new Move(startPos, endPos);
            }
            if (notation.Length == 5) return GenerateMove(notation.Substring(1, 2) + "-" + notation.Substring(3, 2));
            return null;
        }

        private Coordinate findCapturingPawnStartPos(int piece, Coordinate endPos)
        {
            if (_whiteToMove)
            {
                if (Contains(endPos.x - 1, endPos.y - 1, piece | Piece.White)) return new Coordinate(endPos.x - 1, endPos.y - 1);
                if (Contains(endPos.x + 1, endPos.y - 1, piece | Piece.White)) return new Coordinate(endPos.x + 1, endPos.y - 1);
            }
            else
            {
                if (Contains(endPos.x - 1, endPos.y + 1, piece | Piece.Black)) return new Coordinate(endPos.x - 1, endPos.y + 1);
                if (Contains(endPos.x + 1, endPos.y + 1, piece | Piece.Black)) return new Coordinate(endPos.x + 1, endPos.y + 1);
            }

            return endPos;
        }

        public void CheckMate(int winner)
        {
            _gameOver = true;
            _winner = winner;
        }

        private Coordinate findCapturingPawnStartPos(int piece, Coordinate endPos, int xCord)
        {

            if (_whiteToMove)
            {
                if (Contains(xCord, endPos.y - 1, piece | Piece.White)) return new Coordinate(xCord, endPos.y - 1);
            }
            else
            {
                if (Contains(xCord, endPos.y + 1, piece | Piece.Black)) return new Coordinate(xCord, endPos.y + 1);
            }

            return endPos;
        }

        private Coordinate findStartPos(int pieceType, Coordinate endPos)
        {
            int coloredPiece = pieceType | (_whiteToMove ? Piece.White : Piece.Black);

            switch (pieceType)
            {
                case Piece.King:
                    return findKingStartPos(coloredPiece, endPos);
                case Piece.Pawn:
                    return findPawnStartPos(coloredPiece, endPos);
                case Piece.Knight:
                    return findKnightStartPos(coloredPiece, endPos);
                case Piece.Bishop:
                    return findBishopStartPos(coloredPiece, endPos);
                case Piece.Rook:
                    return findRookStartPos(coloredPiece, endPos);
                case Piece.Queen:
                    return findQueenStartPos(coloredPiece, endPos);

                default:
                    return endPos;
            }
        }

        private Coordinate findStartPosX(int pieceType, Coordinate endPos, int xCord)
        {
            int coloredPiece = pieceType | (_whiteToMove ? Piece.White : Piece.Black);

            switch (pieceType)
            {
                case Piece.Knight:
                    return findKnightStartPosX(coloredPiece, endPos, xCord);
                case Piece.Bishop:
                    return findBishopStartPosX(coloredPiece, endPos, xCord);
                case Piece.Rook:
                    return findRookStartPosX(coloredPiece, endPos, xCord);
                case Piece.Queen:
                    return findQueenStartPosX(coloredPiece, endPos, xCord);

                default:
                    return endPos;
            }
        }

        private Coordinate findStartPosY(int pieceType, Coordinate endPos, int yCord)
        {
            int coloredPiece = pieceType | (_whiteToMove ? Piece.White : Piece.Black);

            switch (pieceType)
            {
                case Piece.Knight:
                    return findKnightStartPosY(coloredPiece, endPos, yCord);
                case Piece.Bishop:
                    throw new NotImplementedException();
                    //return findBishopStartPosY(coloredPiece, endPos, xCord);
                case Piece.Rook:
                    return findRookStartPosY(coloredPiece, endPos, yCord);
                case Piece.Queen:
                    throw new NotImplementedException();
                    //return findQueenStartPosY(coloredPiece, endPos, xCord);

                default:
                    return endPos;
            }
        }

        private Coordinate findQueenStartPosX(int piece, Coordinate endPos, int xCord)
        {
            Coordinate startpos;
            startpos = findRookStartPosX(piece, endPos, xCord);

            if (startpos == endPos) startpos = findBishopStartPosX(piece, endPos, xCord);
            return startpos;
        }

        private Coordinate findQueenStartPos(int piece, Coordinate endPos)
        {
            Coordinate startpos;
            startpos = findRookStartPos(piece, endPos);

            if (startpos == endPos) startpos = findBishopStartPos(piece, endPos);
            return startpos;
        }

        private Coordinate findRookStartPosX(int piece, Coordinate endPos, int xCord)
        {
            if (Contains(xCord, endPos.y, piece)) return new Coordinate(xCord, endPos.y);
            for (int i = 0; i < boardSize; i ++) if (Contains(xCord, i, piece)) return new Coordinate(xCord, i);
            return endPos;
        }

        private Coordinate findRookStartPosY(int piece, Coordinate endPos, int yCord)
        {
            if (Contains(endPos.x, yCord, piece)) return new Coordinate(endPos.x, yCord);
            for (int i = 0; i < boardSize; i++) if (Contains(i, yCord, piece)) return new Coordinate(i, yCord);
            return endPos;
        }

        private Coordinate findRookStartPos(int piece, Coordinate endPos)
        {
            Coordinate startPos;

            for (int i = 1; i < boardSize; i++)
            {
                startPos = new Coordinate(endPos.x + i, endPos.y);
                if (startPos.x >= boardSize) break;
                Square currentSquare = GetSquare(startPos);
                if (Contains(currentSquare, piece)) return startPos;
                if (!Contains(currentSquare, Piece.None)) break;
            }

            for (int i = 1; i < boardSize; i++)
            {
                startPos = new Coordinate(endPos.x - i, endPos.y);
                if (startPos.x < 0) break;
                Square currentSquare = GetSquare(startPos);
                if (Contains(currentSquare, piece)) return startPos;
                if (!Contains(currentSquare, Piece.None)) break;
            }

            for (int i = 1; i < boardSize; i++)
            {
                startPos = new Coordinate(endPos.x, endPos.y + i);
                if (startPos.y >= boardSize) break;
                Square currentSquare = GetSquare(startPos);
                if (Contains(currentSquare, piece)) return startPos;
                if (!Contains(currentSquare, Piece.None)) break;
            }

            for (int i = 1; i < boardSize; i++)
            {
                startPos = new Coordinate(endPos.x, endPos.y - i);
                if (startPos.y >= boardSize) break;
                Square currentSquare = GetSquare(startPos);
                if (Contains(currentSquare, piece)) return startPos;
                if (!Contains(currentSquare, Piece.None)) break;
            }

            return endPos;
        }

        private Coordinate findBishopStartPosX(int piece, Coordinate endPos, int xCord)
        {
            int diffrence = endPos.x - xCord;

            if (Contains(piece, xCord, endPos.y - diffrence)) return new Coordinate(xCord, endPos.y - diffrence);
            if (Contains(piece, xCord, endPos.y + diffrence)) return new Coordinate(xCord, endPos.y + diffrence);

            return endPos;
        }

        private Coordinate findBishopStartPos(int piece, Coordinate endPos)
        {
            Coordinate startPos;

            for (int i = 1; i < boardSize; i++)
            {
                startPos = new Coordinate(endPos.x + i, endPos.y + i);
                if (startPos.x < 0 || startPos.x >= boardSize || startPos.y < 0 || startPos.y >= boardSize) break;
                Square currentSquare = GetSquare(startPos);
                if (Contains(currentSquare, piece)) return startPos;
                if (!Contains(currentSquare, Piece.None)) break;
            }

            for (int i = 1; i < boardSize; i++)
            {
                startPos = new Coordinate(endPos.x + i, endPos.y - i);
                if (startPos.x < 0 || startPos.x >= boardSize || startPos.y < 0 || startPos.y >= boardSize) break;
                Square currentSquare = GetSquare(startPos);
                if (Contains(currentSquare, piece)) return startPos;
                if (!Contains(currentSquare, Piece.None)) break;
            }

            for (int i = 1; i < boardSize; i++)
            {
                startPos = new Coordinate(endPos.x - i, endPos.y - i);
                if (startPos.x < 0 || startPos.x  >= boardSize || startPos.y < 0 || startPos.y >= boardSize) break;
                Square currentSquare = GetSquare(startPos);
                if (Contains(currentSquare, piece)) return startPos;
                if (!Contains(currentSquare, Piece.None)) break;
            }

            for (int i = 1; i < boardSize; i++)
            {
                startPos = new Coordinate(endPos.x - i, endPos.y + i);
                if (startPos.x < 0 || startPos.x >= boardSize || startPos.y < 0 || startPos.y >= boardSize) break;
                Square currentSquare = GetSquare(startPos);
                if (Contains(currentSquare, piece)) return startPos;
                if (!Contains(currentSquare, Piece.None)) break;
            }

            return endPos;
        }

        private Coordinate findKnightStartPosX(int piece, Coordinate endPos, int xCord)
        {
            Coordinate startPos;

            startPos = new Coordinate(xCord, endPos.y + 2);
            if (Contains(startPos, piece)) return startPos;

            startPos = new Coordinate(xCord, endPos.y - 2);
            if (Contains(startPos, piece)) return startPos;

            startPos = new Coordinate(xCord, endPos.y + 1);
            if (Contains(startPos, piece)) return startPos;
            
            startPos = new Coordinate(xCord, endPos.y - 1);
            if (Contains(startPos, piece)) return startPos;

            return endPos;
        }

        private Coordinate findKnightStartPosY(int piece, Coordinate endPos, int yCord)
        {
            Coordinate startPos;

            startPos = new Coordinate(endPos.x + 2, yCord);
            if (Contains(startPos, piece)) return startPos;

            startPos = new Coordinate(endPos.x - 2, yCord);
            if (Contains(startPos, piece)) return startPos;

            startPos = new Coordinate(endPos.x + 1, yCord);
            if (Contains(startPos, piece)) return startPos;

            startPos = new Coordinate(endPos.x - 1, yCord);
            if (Contains(startPos, piece)) return startPos;

            return endPos;
        }

        private Coordinate findKnightStartPos(int piece, Coordinate endPos)
        {
            Coordinate startPos;

            startPos = new Coordinate(endPos.x + 2, endPos.y + 1);
            if (Contains(startPos, piece)) return startPos;

            startPos = new Coordinate(endPos.x + 2, endPos.y - 1);
            if (Contains(startPos, piece)) return startPos;

            startPos = new Coordinate(endPos.x - 2, endPos.y + 1);
            if (Contains(startPos, piece)) return startPos;

            startPos = new Coordinate(endPos.x - 2, endPos.y - 1);
            if (Contains(startPos, piece)) return startPos;

            startPos = new Coordinate(endPos.x + 1, endPos.y + 2);
            if (Contains(startPos, piece)) return startPos;

            startPos = new Coordinate(endPos.x + 1, endPos.y - 2);
            if (Contains(startPos, piece)) return startPos;

            startPos = new Coordinate(endPos.x - 1, endPos.y + 2);
            if (Contains(startPos, piece)) return startPos;

            startPos = new Coordinate(endPos.x - 1, endPos.y - 2);
            if (Contains(startPos, piece)) return startPos;

            return endPos;
        }

        private Coordinate findPawnStartPos(int piece, Coordinate endPos)
        {

            for (int i = 1; i < 3; i++)
            {
                int yOriginCord = endPos.y + (_whiteToMove ? -i : i);

                if (Contains(endPos.x, yOriginCord, piece))
                {
                    return new Coordinate(endPos.x, yOriginCord);
                }

                //if (!Contains(endPos.x, i, Piece.None)) break;
            }

            return endPos;
        }

        private Coordinate findKingStartPos(int piece, Coordinate endPos)
        {
            int xOrigin = endPos.x;
            int yOrigin = endPos.y;

            for (int y = -1; y < 2; y++)
            {
                for (int x = -1; x < 2; x++)
                {
                    if (x == 0 && y == 0) continue;

                    if (Contains(x + endPos.x, y + endPos.y, piece))
                    {
                        xOrigin = x + endPos.x;
                        yOrigin = y + endPos.y;
                    }
                }
            }

            return new Coordinate(xOrigin, yOrigin);
        }
    }
}