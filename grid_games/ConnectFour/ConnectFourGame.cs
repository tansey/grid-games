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

        void checkGameOver(GridGame game, int movingPlayer, int curPlayer, Move m)
        {
			int winner;
			GameOver = CheckGameOver(Board, out winner);
			
            if(GameOver)
                Winner = winner;

            setValidNextMoves();
        }
		
        void setValidNextMoves()
        {
			GetValidNextMoves(Board, ValidNextMoves, ActingPlayer);
        }
		
		
		public static bool CheckGameOver(int[,] board, out int winner)
        {	
			// Check horizontal
			int num_in_a_row = 0;
			int oldtype = 0;
			int newtype = 0;
            for (int i = 0; i < 6; i++) {
				oldtype = 0;
				num_in_a_row = 0;
                for (int j = 0; j < 7; j++) {
					newtype = board[i,j];
					if (newtype == oldtype && newtype != 0){
						num_in_a_row += 1;
						if (num_in_a_row == 4) {
							winner = newtype;
							return true;
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
					newtype = board[i,j];
					if (newtype == oldtype && newtype != 0){
						num_in_a_row += 1;
						if (num_in_a_row == 4) {
							winner = newtype;
							return true;
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
					int t1 = board[i, j];
					int t2 = board[i + 1, j + 1];
					int t3 = board[i + 2, j + 2];
					int t4 = board[i + 3, j + 3];
					if (t1 != 0 && t1 == t2 && t2 == t3 && t3 == t4){
							winner = t1;
							return true;
					}
				}
			}
				
			// Check diagonal (down)
			for (int i = 0; i < 3; i++){
				for (int j = 3; j < 7; j++){
					int t1 = board[i, j];
					int t2 = board[i + 1, j - 1];
					int t3 = board[i + 2, j - 2];
					int t4 = board[i + 3, j - 3];
					if (t1 != 0 && t1 == t2 && t2 == t3 && t3 == t4){
							winner = t1;
							return true;
					}
				}
			}
			
			// Check no more moves
			bool empty = false;
            for (int i = 0; i < 6; i++)
                for (int j = 0; j < 7; j++)
                    if (board[i,j] == 0)
						empty = true;
			
			
			if (!empty)
			{
				winner = 0;
				return true;
			}
			winner = 0;
            return false;
        }

        public static void GetValidNextMoves(int[,] board, bool[,] validNextMoves, int player)
        {
            // Any empty square is a valid move
            for (int i = 0; i < board.GetLength(0); i++)
                for (int j = 0; j < board.GetLength(1); j++)
                    validNextMoves[i, j] = board[i, j] == 0 && (j == 0 || board[i, j-1] != 0);
        }

        public static double EvaluateBoard(int[,] board, int player)
        {
            // Um this will be coevolved with magic?
            return 0; 
        }
	
        public override void Reset()
        {
            base.Reset();
            setValidNextMoves();
        }
    }
}
