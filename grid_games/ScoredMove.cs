using System;

namespace grid_games
{
	public class ScoredMove
	{

        public readonly Move Move;
        public readonly int Score;

		public ScoredMove (int x, int y, int score)
		{
			Score = score;
			Move = new Move(x,y);
		}
	}
}

