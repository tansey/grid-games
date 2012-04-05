using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace grid_games.ConnectFour
{
    public class ConnectFourGame : GridGame
    {
        public ConnectFourGame(IAgent hero, IAgent villain)
            : base(6, 7, hero, villain)
        {
            this.AgentMoved += new AgentMovedHandler(checkGameOver);
        }

        void checkGameOver(GridGame game, int player, Move m)
        {
			var Board = game.Board;
			
			// Check horizontal
			int num_in_a_row = 0;
			int oldtype = 0;
			int newtype = 0;
            for (int i = 0; i < 6; i++) {
				oldtype = 0;
				num_in_a_row = 0;
                for (int j = 0; j < 7; j++) {
					newtype = Board[i,j];
					if (newtype == oldtype && newtype != 0){
						num_in_a_row += 1;
						if (num_in_a_row == 4) {
							GameOver = true;
							Winner = newtype;
							return;
						}
					}
					else {
						oldtype = newtype;
						num_in_a_row = 1;
					}
				}
			}
			// Check vertical
            for (int j = 0; j < 7; j++) {
				oldtype = 0;
				num_in_a_row = 0;
                for (int i = 0; i < 6; i++) {
					newtype = Board[i,j];
					if (newtype == oldtype && newtype != 0){
						num_in_a_row += 1;
						if (num_in_a_row == 4) {
							GameOver = true;
							Winner = newtype;
							return;
						}
					}
					else {
						oldtype = newtype;
						num_in_a_row = 1;
					}
				}
			}
				
			// Check diagonal (up)
			for (int i = 0; i < 3; i++){
				for (int j = 0; j < 4; j++){
					int t1 = Board[i, j];
					int t2 = Board[i + 1, j + 1];
					int t3 = Board[i + 2, j + 2];
					int t4 = Board[i + 3, j + 3];
					if (t1 != 0 && t1 == t2 && t2 == t3 && t3 == t4){
							GameOver = true;
							Winner = t1;
							return;
					}
				}
			}
				
			// Check diagonal (down)
			for (int i = 0; i < 3; i++){
				for (int j = 3; j < 7; j++){
					int t1 = Board[i, j];
					int t2 = Board[i + 1, j - 1];
					int t3 = Board[i + 2, j - 2];
					int t4 = Board[i + 3, j - 3];
					if (t1 != 0 && t1 == t2 && t2 == t3 && t3 == t4){
							GameOver = true;
							Winner = t1;
							return;
					}
				}
			}
			
			setValidNextMoves();
			// Check no more moves
			bool empty = false;
            for (int i = 0; i < 6; i++)
                for (int j = 0; j < 7; j++)
                    if (Board[i,j] == 0)
						empty = true;
			
			
			if (!empty)
			{
				GameOver = true;
				Winner = 0;
				return;
			}
        }
		
		
        void setValidNextMoves()
        {
            // Any empty square is a valid move
            for (int i = 0; i < 6; i++)
                for (int j = 0; j < 7; j++)
                    ValidNextMoves[i, j] = Board[i, j] == 0 && (j == 0 || Board[i, j-1] != 0);
        }

        public override void Reset()
        {
            base.Reset();
            setValidNextMoves();
        }
    }
}
