using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using grid_games;
namespace grid_games
{
    /// <summary>
    /// Randomly chooses and returns a valid move.
    /// </summary>
    public class MinimaxAgent : Agent
    {
        public MinimaxAgent(int id) : base(id)
        {
        }

        public override Move GetMove(int[,] board, bool[,] validNextMoves)
        {
			ScoredMove move = MiniMax (board, 9, PlayerId);
			return move.Move;
        }
		
		
		int winner(int[,] Board) {

            if (Board[0, 0] != 0)
            {
                var type = Board[0, 0];
                //top left to bottom left
                if (type == Board[0, 1] && type == Board[0, 2])
                    return type;

                //top left to top right
                if (type == Board[1, 0] && type == Board[2, 0])
                    return type;
				
                //top left to bottom right
                if (type == Board[1, 1] && type == Board[2, 2])
                    return type;
            }

            if (Board[0, 2] != 0)
            {
                var type = Board[0, 2];

                //bottom left to top right
                if (type == Board[1, 1] && type == Board[2, 0])
                    return type;

                //bottom left to bottom right
                if (type == Board[1, 2] && type == Board[2, 2])
                    return type;
            }

            if (Board[0, 1] != 0)
            {
                var type = Board[0, 1];

                //middle left to middle right
                if (type == Board[1, 1] && type == Board[2, 1])
                    return type;
            }

            if (Board[1, 0] != 0)
            {
                var type = Board[1, 0];

                //middle top to middle bottom
                if (type == Board[1, 1] && type == Board[1, 2])
                    return type;
            }

            if (Board[2, 0] != 0)
            {
                var type = Board[2, 0];

                //top right to bottom right
                if (type == Board[2, 1] && type == Board[2, 2])
                    return type;
			}
			
			return 0;
		}
		
		bool tie(int[,] board) {
			return !board.HasEmptyCell();
		}
		
		
		
		
		public ScoredMove MiniMax(int[,] board, int depth, int player){
			int win = winner(board);
			if (win != 0){
				if (win == PlayerId)
					return new ScoredMove(0,0,2);
				else
					return new ScoredMove(0,0,0);
			}				
			if (tie(board) || depth == 0)
				return new ScoredMove(0,0,1);
			
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