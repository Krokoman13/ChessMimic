using Chess;
using System;
using System.IO;

namespace Mimic
{
    public class InstructionGenerator
    {
        Board board;

        const string relativePath = "";

        const string outputFilePath = "Instructions/";
        const string inputFilePath = "ToProces/";

        const string white = "White";
        const string black = "Black";

        const string win = "Win";

        public InstructionGenerator()
        {
            board = new Board();
        }

        public void SetFen(string fen)
        {
            board.Generate(fen);
        }

        public void ProcesAllFiles(DirectoryInfo directory)
        {
            foreach (FileInfo file in directory.GetFiles("*.txt"))
            {
                ProcesFile(file);
            }
        }

        public void ProcesAllFiles()
        {
            ProcesAllFiles(new DirectoryInfo(inputFilePath));
        }

        public void ProcesFile(string fileName)
        {
            Console.WriteLine();
            Console.WriteLine("File: " + fileName + " processing...");

            board = new Board();

            string[] rows = File.ReadAllText(inputFilePath + fileName).Split('\n');

            board.Generate(rows[0]);

            string newBlackInstr = "";
            string newWhiteInstr = "";

            string[] lastRowMoves = rows[rows.Length - 1].Split(' ');

            string winPath = "";
            bool whiteWins = lastRowMoves[lastRowMoves.Length - 1] == "1-0";
            bool blackWins = lastRowMoves[lastRowMoves.Length - 1] == "0-1";

            if (blackWins) winPath = relativePath + outputFilePath + black + win + ".txt";
            else if (whiteWins) winPath = relativePath + outputFilePath + white + win + ".txt";

            string winningInstr = "";

            for  (int y = 1; y < rows.Length; y++)
            {
                string[] moves = rows[y].Split(' ');

                for (int x = 1; x < moves.Length; x++)
                {
                    if (board.gameOver) break;
                    if (x > 2) break;

                    string fullInstruction;
                    string algebraicMove;
                    string fen;

                    Console.WriteLine(y +": "+ moves[x]);

                    if (y == 17)
                    {
                        board.Display();
                    }

                    Move move = board.GenerateMove(moves[x]);

                    fen = board.GenerateRealFenString();
                    board.ExecuteMove(move);

                    algebraicMove = move.RevAlgebraic();
                    if (board.gameOver) algebraicMove += "#";

                    fullInstruction = fen + "\n" + algebraicMove + "\n\n";

                    if (!isInFile(relativePath + outputFilePath + (board.whiteToMove ? black : white) + ".txt", fen))
                    {

                        if (!board.whiteToMove) newWhiteInstr += fullInstruction;
                        else newBlackInstr += fullInstruction;
                        //continue;
                    }

                    if (isInFile(winPath, fen)) continue;
                    if (!board.whiteToMove && whiteWins || board.whiteToMove && blackWins) winningInstr += fullInstruction;
                }

                if (board.gameOver)
                {
                    break;
                }
            }

            if (!blackWins) AddToFile(relativePath + outputFilePath + black + ".txt", newBlackInstr);
            if (!whiteWins) AddToFile(relativePath + outputFilePath + white + ".txt", newWhiteInstr);

            if (blackWins || whiteWins) AddToFile(winPath, winningInstr);

            Console.WriteLine(board.GenerateRealFenString());
            Console.WriteLine("File: " + fileName + " processed!");
            Console.Write("Winner: ");
            if (blackWins) Console.WriteLine(black);
            else if (whiteWins) Console.WriteLine(white);
            else Console.WriteLine();
            Console.WriteLine();
            board.Display();
        }

        public void AddToFile(string filePath, string toAdd)
        {
            File.WriteAllText(filePath, File.ReadAllText(filePath) + toAdd);
        }

        public void ProcesFile(FileInfo file)
        {
            ProcesFile(file.Name);
        }

        private string replaceInstructionInString(string inputString, string fen1, string newInstruction)
        {
            string[] fileContens = inputString.Split('\n');

            for (int i = 0; i < fileContens.Length; i += 3)
            {
                string fen2 = fileContens[i];

                if (fen1 == fen2)
                {
                    if (fileContens[i + 1] == newInstruction) return inputString;
                    fileContens[i + 1] = newInstruction;
                }
            }

            string outp = "";

            for (int i = 0; i < fileContens.Length; i++)
            {
                outp += fileContens[i] + "\n";
            }

            return outp;
        }

        private bool isInFile(string filePath, string fen1)
        {
            if (filePath == "") return false;

            string[] fileContens = File.ReadAllText(filePath).Split('\n');

            for (int i = 0; i < fileContens.Length; i += 3)
            {
                string fen2 = fileContens[i];

                if (fen1 == fen2)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
