using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace grid_games
{
    public interface IAgent
    {
        int PlayerId { get; set; }

        Move GetMove(int[,] board, bool[,] validNextMoves);
    }
}
