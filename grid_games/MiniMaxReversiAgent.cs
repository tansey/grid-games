using System;

namespace grid_games
{
	public class MiniMaxReversiAgent : MinimaxAgent
	{
        public MiniMaxReversiAgent(int id) : base(id)
        {
        }

        public override Move GetMove(int[,] board, bool[,] validNextMoves)
        {
			ScoredMove move = MiniMax (board, 6, PlayerId);
			return move.Move;
        }
		
		
		int winner(int[,] Board) {
			if (!Board.HasEmptyCell()){
				var sum = 0;
				for (int i = 0; i < 8; i++)
					for (int j = 0; j < 8; j++)
						sum += Board[i,j];
				if (sum < 0)
					return -1;
				if (sum > 0)
					return 1;
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

