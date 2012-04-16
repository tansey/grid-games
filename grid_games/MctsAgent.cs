using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace grid_games
{
    public class MctsAgent : Agent
    {
        public delegate bool CheckGameOver(int[,] Board, out int winner);
        public delegate void GetValidNextMoves(int[,] board, bool[,] validNextMoves, int player);
        public delegate void ApplyMove(int player, Move m, int[,] board);

        CheckGameOver _checkGameOver;
        GetValidNextMoves _validNextMoves;
        ApplyMove _applyMove;
        Random _random;
        GridGameParameters _params;

        public MctsAgent(int id, 
            CheckGameOver check, 
            GetValidNextMoves valid,
            ApplyMove applyMove,
            GridGameParameters parameters) : base(id)
        {
            Debug.Assert(parameters != null);

            _checkGameOver = check;
            _validNextMoves = valid;
            _applyMove = applyMove;

            _params = parameters;
            _random = new Random();
        }

        public override Move GetMove(int[,] board, bool[,] validNextMoves)
        {
            // Create candidate moves
            List<MctsMove> candidates = new List<MctsMove>();
            for (int i = 0; i < board.GetLength(0); i++)
                for (int j = 0; j < board.GetLength(1); j++)
                    if (validNextMoves[i, j])
                        candidates.Add(new MctsMove(new Move(i, j)));

            int[,] tempBoard = new int[board.GetLength(0), board.GetLength(1)];
            for (int trial = 0; trial < _params.MonteCarloTrials; trial++)
            {
                // select a node
                MctsMove candidate = null;
                
                // try each move at least once
                if (trial < candidates.Count * _params.MinMcTrialsPerMove)
                    candidate = candidates[trial % candidates.Count];
                else
                    candidate = selectBestCandidate(candidates);

                // create a temporary copy of the board that can be changed
                Array.Copy(board, tempBoard, board.Length);
                _applyMove(PlayerId, candidate.Move, tempBoard);

                // play a random game
                int winner = playRandomGame(tempBoard, PlayerId * -1);

                candidate.Trials++;
                candidate.Points += winner == PlayerId ? 1 : winner == 0 ? 0 : -1;
            }

            foreach (var c in candidates)
                Console.WriteLine("Move: ({0},{1}) Score: {2} Trials: {3}",
                    c.Move.Row, c.Move.Column, c.Score(0, _params.MonteCarloTrials),
                    c.Trials);

            return candidates.ArgMax(m => m.Score(0, _params.MonteCarloTrials)).Move;
        }

        private MctsMove selectBestCandidate(List<MctsMove> candidates)
        {
            int totalTrials = candidates.Sum(m => m.Trials);
            return candidates.ArgMax(m => m.Score(0.5, totalTrials));
        }
        
        private int playRandomGame(int[,] board, int player)
        {
            bool[,] validNextMoves = new bool[board.GetLength(0), board.GetLength(1)];
            int winner;
            while (!_checkGameOver(board, out winner))
            {
                _validNextMoves(board, validNextMoves, player);
                List<Move> moves = new List<Move>();
                for(int i = 0; i < board.GetLength(0); i++)
                    for (int j = 0; j < board.GetLength(1); j++)
                        if(validNextMoves[i,j])
                            moves.Add(new Move(i, j));
                
                if(moves.Count > 0)
                {
                    //Console.WriteLine("Number of Valid Moves (du-duh-dummmmmm): {0}", moves.Count);
                    Move m = moves[_random.Next(moves.Count)];
                    _applyMove(player, m, board);
                }
                player *= -1;
            }

            //board.PrintBoard();
            return winner;
        }



        class MctsMove
        {
            public Move Move;
            public int Trials;
            public double Points;

            public MctsMove(Move m)
            {
                Move = m;
            }

            public void Backup(int reward)
            {
                Trials++;
                Points += reward;
            }

            public double Score(double c, int totalTrials)
            {
                return Points / (double)Trials + c * Math.Sqrt(Math.Log(2 * totalTrials) / (double)Trials);
            }
        }


    }
}
