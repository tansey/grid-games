using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using grid_games;
using System.IO;
using System.Diagnostics;
namespace grid_games
{
    /// <summary>
    /// Performs a generalized Minimax tree search.
    /// </summary>
    public class MinimaxAgent : Agent
    {
        public delegate bool CheckGameOver(int[,] Board, out int winner);
        public delegate void GetValidNextMoves(int[,] board, bool[,] validNextMoves, int player);
        public delegate double BoardEval(int[,] board, GridGameParameters ggp, int player);
        public delegate void ApplyMove(int player, Move m, int[,] board);

        CheckGameOver _checkGameOver;
        GetValidNextMoves _validNextMoves;
        BoardEval _boardEval;
        ApplyMove _applyMove;

        GridGameParameters _params;

        public MinimaxAgent(int id, 
            CheckGameOver check, 
            GetValidNextMoves valid,
            BoardEval eval,
            ApplyMove apply,
            GridGameParameters parameters) : base(id)
        {
            Debug.Assert(parameters != null);

            _checkGameOver = check;
            _validNextMoves = valid;
            _boardEval = eval;
            _applyMove = apply;

            _params = parameters;
        }

        public override Move GetMove(int[,] board, bool[,] validNextMoves)
        {
            ScoredMove move = MiniMax(board, validNextMoves, _params.MinimaxDepth, PlayerId, "");
            return move.Move;
        }

		public ScoredMove MiniMax(int[,] board, bool[,] validNextMoves, int depth, int player, string padding){
            //board.SaveBoard(writer, padding);
            //board.PrintBoard(padding);
            
            int win;
            bool over = _checkGameOver(board, out win);
            if (over)
            {
                if (win == PlayerId)
                {
                    //Console.WriteLine("{0}Reward: {1}", padding, _params.WinReward);
                    //Console.WriteLine();
                    //Console.WriteLine(); 
                    return new ScoredMove(0, 0, _params.WinReward);
                }
                else if (win == 0)
                {
                    //Console.WriteLine("{0}Reward: {1}", padding, _params.TieReward);
                    //Console.WriteLine();
                    //Console.WriteLine();
                    return new ScoredMove(0, 0, _params.TieReward);
                }
                else
                {
                    //Console.WriteLine("{0}Reward: {1}", padding, _params.LossReward);
                    //Console.WriteLine();
                    //Console.WriteLine();
                    return new ScoredMove(0, 0, _params.LossReward);
                }
            }

            if (depth == 0)
            {
                //Console.WriteLine("{0} Depth exceeded. Reward: {1}", padding, _boardEval(board, _params, player));
                //Console.WriteLine();
                //Console.WriteLine();
                return new ScoredMove(0, 0, _boardEval(board, _params, player));
            }
			
			double alpha = double.MinValue;
			if (player != PlayerId)
				alpha = double.MaxValue;

			ScoredMove nextMove = null;
            bool[,] oppValidMoves = new bool[validNextMoves.GetLength(0), validNextMoves.GetLength(1)];
            int[,] oppBoard = new int[board.GetLength(0), board.GetLength(1)];
			for (int i = 0; i < board.GetLength(0); i++){
				for (int j = 0; j < board.GetLength(1); j++) {
                    
                    // Only try valid next moves.
                    if (!validNextMoves[i, j])
                        continue;

                    // Try moving here
                    //int prev = board[i, j];
                    //board[i, j] = player;
                    Array.Copy(board, oppBoard, board.Length);
                    _applyMove(player, new Move(i, j), oppBoard);
                    
                    // Update the valid moves for the next player
                    _validNextMoves(oppBoard, oppValidMoves, player * -1);

                    // Recurse
                    ScoredMove move = MiniMax(oppBoard, oppValidMoves, depth - 1, player * -1, padding + " + ");

                    // Undo the move
                    //board[i, j] = prev;

                    // If we found a win, return this move.
					if (player == PlayerId){
                        if (move.Score == _params.WinReward)
                            return new ScoredMove(i, j, _params.WinReward);
                    	
                        // If we found a new highest-scoring branch,
                    	// set it as the current best.
						if (nextMove == null || move.Score > alpha)
						{
							alpha = move.Score;
							nextMove = new ScoredMove(i, j, alpha);
						}
					}
					else {
                        if (move.Score == _params.LossReward)
                            return new ScoredMove(i, j, _params.LossReward);
                    	
                        // If we found a new highest-scoring branch,
                    	// set it as the current best.
						if (nextMove == null || move.Score < alpha)
						{
							alpha = move.Score;
							nextMove = new ScoredMove(i, j, alpha);
						}
					}
					
					
				}
			}
			
			if (nextMove == null)
			{
				_validNextMoves(board, oppValidMoves, player * -1);
				return MiniMax(board, oppValidMoves, depth - 1, player * -1, padding + " + ");
			}

            // Return the best move we found.
			return nextMove;
		}
	}
}