using System;

namespace grid_games
{
	public class ScoredMove
	{

        public readonly Move Move;
        public readonly double Score;

		public ScoredMove (int x, int y, double score)
		{
			Score = score;
			Move = new Move(x,y);
		}
	}
}

