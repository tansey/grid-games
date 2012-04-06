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

    }
}
