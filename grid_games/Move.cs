using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace grid_games
{
    public struct Move
    {
        public readonly int Row;
        public readonly int Column;

        public Move(int row, int column)
        {
            Row = row;
            Column = column;
        }
    }
}
