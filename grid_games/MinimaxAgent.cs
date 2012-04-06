using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using grid_games;
namespace grid_games
{
    /// <summary>
    /// Performs a generalized Minimax tree search.
    /// </summary>
    public class MinimaxAgent : Agent
    {
        public delegate bool CheckGameOver(int[,] Board, out int winner);
        public delegate void GetValidNextMoves(int[,] board, bool[,] validNextMoves);
        public delegate double BoardEval(int[,] board, int player);

        CheckGameOver _checkGameOver;
        GetValidNextMoves _validNextMoves;
        BoardEval _boardEval;

        GridGameParameters _params;

        public MinimaxAgent(int id, 
            CheckGameOver check, 
            GetValidNextMoves valid,
            BoardEval eval,
            GridGameParameters parameters) : base(id)
        {
            _checkGameOver = check;
            _validNextMoves = valid;
            _boardEval = eval;

            _params = parameters;
        }

        public override Move GetMove(int[,] board, bool[,] validNextMoves)
        {
			ScoredMove move = MiniMax (board, validNextMoves, _params.MinimaxDepth, PlayerId);
			return move.Move;
        }
		
		
		public ScoredMove MiniMax(int[,] board, bool[,] validNextMoves, int depth, int player){
            int win;
            bool over = _checkGameOver(board, out win);
            if (over)
            {
                if (win == PlayerId)
                    return new ScoredMove(0, 0, _params.WinReward);
                else if (win == 0)
                    return new ScoredMove(0, 0, _params.TieReward);
                else
                    return new ScoredMove(0, 0, _params.LossReward);
            }

            if (depth == 0)
                return new ScoredMove(0, 0, _boardEval(board, player));


			double alpha = double.MinValue;
			ScoredMove nextMove = null;
			for (int i = 0; i < board.GetLength(0); i++){
				for (int j = 0; j < board.GetLength(1); j++) {
					if (!validNextMoves[i,j])
                        continue;
					
                    //int [,] newBoard = new int[board.GetLength(0),board.GetLength(1)];
                    //Array.Copy(board, newBoard, board.Length);
                    //newBoard[i,j] = player;

                    // Temporarily move here
                    int prev = board[i, j];
                    board[i, j] = player;

                    // Update the valid moves for the next player
                    _validNextMoves(board, validNextMoves);

                    // Recurse
					ScoredMove move = MiniMax(board, validNextMoves, depth-1, player * -1);

                    // Undo the temporary move
                    board[i, j] = prev;

                    // If we found a win, return this move
                    if (move.Score == 2)
                        //return move;
                        return new ScoredMove(i, j, 2);
					if (move.Score > alpha)
					{
						alpha = move.Score;
						nextMove = new ScoredMove(i, j, alpha);
					}
					
				}
			}
			return nextMove;
		}
	}
}