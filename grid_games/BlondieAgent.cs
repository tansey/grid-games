using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpNeat.Phenomes;
using System.Diagnostics;

namespace grid_games
{
    /// <summary>
    /// A Blondie24-style agent that performs a minimax search using a neural network
    /// as its board evaluation function.
    /// </summary>
    public class BlondieAgent : Agent
    {
        MinimaxAgent _minimax;
        GridGameParameters _params;

        public IBlackBox Brain { get; set; }

        public BlondieAgent(int id,
            MinimaxAgent.CheckGameOver check,
            MinimaxAgent.GetValidNextMoves valid,
            IBlackBox brain,
            GridGameParameters parameters)
            : base(id)
        {
            Brain = brain;
            _params = parameters;
            _minimax = new MinimaxAgent(id, check, valid, Evaluate, parameters);
        }

        public override Move GetMove(int[,] board, bool[,] validNextMoves)
        {
            _minimax.PlayerId = PlayerId;
            return _minimax.GetMove(board, validNextMoves);
        }

        public double Evaluate(int[,] board, int player)
        {
            // ANN should map from board squares to a single value
            Debug.Assert(Brain.InputCount == board.Length);
            Debug.Assert(Brain.OutputCount == 1);

            // Clear the network
            Brain.ResetState();

            // Console.WriteLine("player: {0}", player);
            // Set the board state as the inputs
            for (int i = 0; i < board.GetLength(0); i++)
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    //Console.WriteLine("[{0},{1}]={2}", i, j, board[i, j].toBoardSensor(player));
                    Brain.InputSignalArray[i * board.GetLength(0) + j] = board[i, j].toBoardSensor(player);
                }

            // Activate the network
            Brain.Activate();

            //Console.WriteLine("{0}: {1}", AgentId, Brain.OutputSignalArray[0]);

            // Return the value of the board
            return Brain.OutputSignalArray[0] * _params.WinReward;
        }
    }
}
