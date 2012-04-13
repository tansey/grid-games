using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using grid_games;

namespace AsciiGames
{
    /// <summary>
    /// An agent that lets a human play the game via the console.
    /// </summary>
    public class AsciiAgent : Agent
    {
        public AsciiAgent(int id) : base(id)
        {

        }

        public override Move GetMove(int[,] board, bool[,] validNextMoves)
        {
            board.PrintBoard();

            DisplayValidMoves(validNextMoves);

            int[] move = GetMoveFromConsole(validNextMoves);

            return new Move(move[0], move[1]);
        }

        private int[] GetMoveFromConsole(bool[,] validNextMoves)
        {
            Console.Write("Your move? ");
            string input = Console.ReadLine();
            int[] move = tryParseInput(input);
            if (move[0] >= validNextMoves.GetLength(0) || move[0] < 0 
                || move[1] >= validNextMoves.GetLength(1) || move[1] < 0 
                || !validNextMoves[move[0], move[1]])
                move = null;
            while (move == null)
            {
                Console.WriteLine("Invalid input. Try again.");
                Console.Write("Your move? ");
                input = Console.ReadLine();
                move = tryParseInput(input);
                if (!validNextMoves[move[0], move[1]])
                    move = null;
            }
            return move;
        }

        private int[] tryParseInput(string input)
        {
            string[] tokens = input.Replace("(", "").Replace(")", "").Trim().Split(',', ' ');
            if (tokens.Length < 2)
                return null;

            try
            {
                int x = int.Parse(tokens.First());
                int y = int.Parse(tokens.Last());
                return new int[] { x, y };
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static void DisplayValidMoves(bool[,] validNextMoves)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Valid next moves:");
            const int MOVES_PER_LINE = 4;
            int moves = 0;
            for (int x = 0; x < validNextMoves.GetLength(0); x++)
                for (int y = 0; y < validNextMoves.GetLength(1); y++)
                    if (validNextMoves[x, y])
                    {
                        string move = string.Format("({0}, {1})", x, y);
                        if (moves % MOVES_PER_LINE == 0)
                            sb.AppendLine();
                        sb.Append(move.PadRight(10));
                        moves++;
                    }
            Console.WriteLine(sb.ToString());
        }

        
    }
}
