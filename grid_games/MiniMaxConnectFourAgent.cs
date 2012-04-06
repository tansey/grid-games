using System;

namespace grid_games
{
	public class MiniMaxConnectFourAgent : MiniMaxAgent
	{
        public MiniMaxConnectFourAgent(int id) : base(id)
        {
        }

        public override Move GetMove(int[,] board, bool[,] validNextMoves)
        {
			ScoredMove move = MiniMax (board, 4, PlayerId);
			return move.Move;
        }
		
		
		int winner(int[,] Board) {
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
							return t1;
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
							return t1;
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
							return t1;
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
							return t1;
					}
				}
			}
			
			return 0;
		}
		
		bool tie(int[,] Board) {
			if (!Board.HasEmptyCell()){
				var sum = 0;
				for (int i = 0; i < 8; i++)
					for (int j = 0; j < 8; j++)
						sum += Board[i,j];
				if (sum == 0)
					return true;
			}
			return false;
		}
		
		public int EvaluationFunction(int[,] board) {
			var sum = 0;
				for (int i = 0; i < 8; i++)
					for (int j = 0; j < 8; j++)
						sum += board[i,j];
			return sum;
			
		}
		
		
		public ScoredMove MiniMax(int[,] board, int depth, int player){
			int win = winner(board);
			if (win != 0){
				if (win == PlayerId)
					return new ScoredMove(0,0,65);
				else
					return new ScoredMove(0,0,-65);
			}				
			if (tie(board))
				return new ScoredMove(0,0,0);
			if (depth == 0)
				return new ScoredMove(0, 0, EvaluationFunction(board));
			
			int alpha = -100;
			ScoredMove nextMove = null;
			for (int i = 0; i < 3; i++){
				for (int j = 0; j < 3; j++) {
					if (board[i,j] == 0)
					{
						int [,] newBoard = new int[3,3];
                        Array.Copy(board, newBoard, board.Length);
						newBoard[i,j] = player;
						ScoredMove move = MiniMax(newBoard, depth-1, player * -1);
						if (move.Score == 2)
							return move;
						if (move.Score > alpha)
						{
							alpha = move.Score;
							nextMove = move;
						}
					}
				}
			}
			return nextMove;
		}
	}
}

