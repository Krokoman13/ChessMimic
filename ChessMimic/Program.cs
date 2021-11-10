using System;
using Chess;
using Mimic;

class Program
{
    static void Main(string[] args)
    {
        InstructionGenerator generator = new InstructionGenerator();
        generator.ProcesAllFiles();

        GameHandeler game = new GameHandeler(new HumanPlayer(Piece.White), new MimicPlayer(Piece.Black));
        game.Run();
    }
}

