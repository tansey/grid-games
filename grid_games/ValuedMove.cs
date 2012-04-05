using System;

namespace grid_games
{
	public class ValuedMove 
	{
		int Value;
		Move Move;
		public ValuedMove (int row, int column, int value)
		{
			Value = value;	
			Move = new Move(row, column);
			
		}
	}
}

