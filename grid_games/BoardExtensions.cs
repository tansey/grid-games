using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace grid_games
{
    public static class BoardExtensions
    {
        public static bool HasEmptyCell(this int[,] board)
        {
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (board[i, j] == 0)
                        return true;
            return false;
        }

        public static void PrintBoard(this int[,] board, string padding = "")
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(padding + "  ");
            for (int row = 0; row < board.GetLength(0); row++)
                sb.Append("  " + row + " ");
            sb.AppendLine();
            for (int col = board.GetLength(1) - 1; col >= 0; col--)
            {
                sb.Append(padding + "  ");
                for (int row = 0; row < board.GetLength(0); row++)
                    sb.Append("----");
                sb.AppendLine("-");
                sb.Append(col + " ");
                sb.Append(padding);
                for (int row = 0; row < board.GetLength(0); row++)
                    sb.Append(string.Format("| {0} ", board[row, col] == 1 ? "X" : board[row, col] == -1 ? "O" : " "));
                sb.AppendLine("|");
            }
            sb.Append(padding + "  ");
            for (int col = 0; col < board.GetLength(0); col++)
                sb.Append("----");
            sb.AppendLine("-");
            Console.WriteLine(sb.ToString());
        }

        public static void SaveBoard(this int[,] board, TextWriter writer, string padding = "")
        {
            StringBuilder sb = new StringBuilder();
            for (int col = board.GetLength(1) - 1; col >= 0; col--)
            {
                sb.Append(padding);
                for (int row = 0; row < board.GetLength(0); row++)
                    sb.Append("----");
                sb.AppendLine("-");
                sb.Append(padding);
                for (int row = 0; row < board.GetLength(0); row++)
                    sb.Append(string.Format("| {0} ", board[row, col] == 1 ? "X" : board[row, col] == -1 ? "O" : " "));
                sb.AppendLine("|");
            }
            sb.Append(padding);
            for (int col = 0; col < board.GetLength(0); col++)
                sb.Append("----");
            sb.AppendLine("-");
            writer.WriteLine(sb.ToString());
        }
    }
}
