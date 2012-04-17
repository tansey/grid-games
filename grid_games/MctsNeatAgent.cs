using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpNeat.Phenomes;

namespace grid_games
{
    public class MctsNeatAgent : MctsAgent
    {
        public IBlackBox Brain { get; set; }

        public MctsNeatAgent(int id,
            CheckGameOver check, 
            GetValidNextMoves valid,
            IBlackBox brain,
            ApplyMove applyMove,
            GridGameParameters parameters) : base(id, check, valid, applyMove, parameters)
        {
            Brain = brain;
        }

        protected override int playRandomGame(int[,] board, int player)
        {
            bool[,] validNextMoves = new bool[board.GetLength(0), board.GetLength(1)];
            int winner;
            while (!_checkGameOver(board, out winner))
            {
                _validNextMoves(board, validNextMoves, player);
                if(hasValidMove(validNextMoves))
                {
                    Move m = getSoftMaxMove(board, validNextMoves, player);
                    _applyMove(player, m, board);
                }
                player *= -1;
            }

            return winner;
        }

        private bool hasValidMove(bool[,] validNextMoves)
        {
            foreach (bool b in validNextMoves)
                if (b)
                    return true;
            return false;
        }

        private Move getSoftMaxMove(int[,] board, bool[,] validNextMoves, int player)
        {
            // Clear the network
            Brain.ResetState();

            // Set the board state as the inputs
            for (int i = 0; i < board.GetLength(0); i++)
                for (int j = 0; j < board.GetLength(1); j++)
                    Brain.InputSignalArray[i * board.GetLength(1) + j] = board[i, j].toBoardSensor(player);

            // Activate the network
            Brain.Activate();

            // Get the highest activated output neuron that is a valid move
            // TODO:
            //  - Should this be soft-max?
            //  - Should this be a value function with enumerated valid moves?
            double sum = 0;
            List<CandidateMove> candidates = new List<CandidateMove>();
            for (int i = 0; i < board.GetLength(0); i++)
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    // Only look at output neurons for valid moves
                    if (!validNextMoves[i, j])
                        continue;

                    // Get the activation of the neuron for this spot on the board
                    double val = Brain.OutputSignalArray[i * board.GetLength(1) + j];

                    candidates.Add(new CandidateMove(new Move(i, j), val));
                    sum += val;
                }

            // Perform softmax selection
            double guess = _random.NextDouble() * sum;
            int guessIdx = -1;
            do
            {
                guessIdx++;
                guess -= candidates[guessIdx].Score;
            }
            while (guess > 0);

            return candidates[guessIdx].Move;
        }

        struct CandidateMove
        {
            public readonly Move Move;
            public readonly double Score;

            public CandidateMove(Move m, double score)
            {
                Move = m;
                Score = score;
            }
        }
    }
}
