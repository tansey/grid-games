using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace grid_games
{
    public class Turn
    {
        public readonly Move Move;
        public readonly int Player;
        public readonly double Time;

        public Turn(Move m, int player, double time)
        {
            Move = m;
            Player = player;
            Time = time;
        }
    }
}
