using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public static void DrawBoard(this int[,] board)
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
