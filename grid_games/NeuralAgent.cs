using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpNeat.Phenomes;

namespace grid_games
{
    public class NeuralAgent : Agent
    {
        public IBlackBox Brain { get; set; }

        public NeuralAgent(int id, IBlackBox brain)
            : base(id)
        {
            Brain = brain;
        }

        public override Move GetMove(int[,] board, bool[,] validNextMoves)
        {
            // Clear the network
            Brain.ResetState();

            // Set the board state as the inputs
			for (int i = 0; i < board.GetLength(0); i++)
                for (int j = 0; j < board.GetLength(1); j++)
                    Brain.InputSignalArray[i * board.GetLength(1) + j] = board[i, j].toBoardSensor(PlayerId);
            
            // Activate the network
            Brain.Activate();

            // Get the highest activated output neuron that is a valid move
            // TODO:
            //  - Should this be soft-max?
            //  - Should this be a value function with enumerated valid moves?
            double maxVal = 0;
            int maxRow = -1, maxColumn = -1;
            for (int i = 0; i < board.GetLength(0); i++)
                for (int j = 0; j < board.GetLength(1); j++)
                {
            
                    // Only look at output neurons for valid moves
                    if (!validNextMoves[i, j])
                        continue;

                    // Get the activation of the neuron for this spot on the board
                    double val = Brain.OutputSignalArray[i * board.GetLength(1) + j];

                    // If this activation is the highest so far, it's the current move
                    if (maxRow == -1 || val > maxVal)
                    {
                        maxVal = val;
                        maxRow = i;
                        maxColumn = j;
                    }
                }

            return new Move(maxRow, maxColumn);
        }
    }
}
