using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace grid_games
{
    public static class NumberExtensions
    {
        /// <summary>
        /// Converts the board piece to a value acceptable for an input neuron.
        /// </summary>
        /// <param name="playerId">The PlayerId of the acting player.
        /// Should be 1 or -1 for 2-player games.</param>
        /// <returns>1 for the acting player's piece, 0.5 for an empty cell, or 0 for an opponent's piece.</returns>
        public static double toBoardSensor(this int i, int playerId)
        {
            if (i == playerId)
                return 1;
            if (i == 0)
                return 0.5;
            return 0;
        }
    }
}
