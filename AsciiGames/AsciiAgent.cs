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
            DrawBoard(board);

            DisplayValidMoves(validNextMoves);

            int[] move = GetMoveFromConsole(validNextMoves);

            return new Move(move[0], move[1]);
        }

        private int[] GetMoveFromConsole(bool[,] validNextMoves)
        {
            Console.Write("Your move? ");
            string input = Console.ReadLine();
            int[] move = tryParseInput(input);
            if (!validNextMoves[move[0], move[1]])
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

        public static void DrawBoard(int[,] board)
        {
            StringBuilder sb = new StringBuilder();
            for (int col = board.GetLength(1) - 1; col >= 0; col--)
            {
                for (int row = 0; row < board.GetLength(0); row++)
                    sb.Append("----");
                sb.AppendLine("-");
                for (int row = 0; row < board.GetLength(0); row++)
                    sb.Append(string.Format("| {0} ", board[row, col] == 1 ? "X" : board[row, col] == -1 ? "O" : " "));
                sb.AppendLine("|");
            }
            for (int col = 0; col < board.GetLength(0); col++)
                sb.Append("----");
            sb.AppendLine("-");
            Console.WriteLine(sb.ToString());
        }
    }
}
