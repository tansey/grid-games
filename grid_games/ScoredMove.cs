using System;

namespace grid_games
{
	public class ScoredMove
	{
		
		public Move Move {get; set;}
		public int Score {get; set;}
		public ScoredMove (int x, int y, int score)
		{
			Score = score;
			Move = new Move(x,y);
		}
	}
}

