using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace grid_games
{
    /// <summary>
    /// Randomly chooses and returns a valid move.
    /// </summary>
    public class RandomAgent : Agent
    {
        Random _random;

        public RandomAgent(int id) : base(id)
        {
            _random = new Random();
        }

        public RandomAgent(int id, int seed)
            : base(id)
        {
            _random = new Random(seed);
        }

        public override Move GetMove(int[,] board, bool[,] validNextMoves)
        {
            List<Move> validMoves = new List<Move>();
            for (int i = 0; i < validNextMoves.GetLength(0); i++)
                for (int j = 0; j < validNextMoves.GetLength(1); j++)
                    if (validNextMoves[i, j])
                        validMoves.Add(new Move(i, j));

            return validMoves[_random.Next(validMoves.Count)];
        }
    }
}
