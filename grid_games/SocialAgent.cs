using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpNeat.Phenomes;

namespace grid_games
{
    public class SocialAgent : NeuralAgent
    {
        const int DEFAULT_MEMORY_SIZE = 1;
        const double DEFAULT_LEARNING_RATE = 0.1;
        const double DEFAULT_MOMENTUM_RATE = 0.9;

        /// <summary>
        /// The maximum number of timesteps to remember.
        /// </summary>
        public int MemorySize { get; set; }

        /// <summary>
        /// A sliding window of stateActionPair-action pairs for this teacher.
        /// </summary>
        public LinkedList<StateActionReward> Memory { get; set; }

        /// <summary>
        /// The learning rate for backpropping on this teacher.
        /// </summary>
        public double LearningRate { get; set; }

        /// <summary>
        /// The momentum rate for backpropping on this teacher.
        /// </summary>
        public double Momentum { get; set; }

        public SocialAgent(int id, IBlackBox brain, int memorySize = DEFAULT_MEMORY_SIZE)
            : base(id, brain)
        {
            MemorySize = memorySize;
            Memory = new LinkedList<StateActionReward>();
            LearningRate = DEFAULT_LEARNING_RATE;
            Momentum = DEFAULT_MOMENTUM_RATE;
        }

        public override Move GetMove(int[,] board, bool[,] validNextMoves)
        {
            var move = base.GetMove(board, validNextMoves);

            double[] inputs = new double[Brain.InputSignalArray.Length];
            Brain.InputSignalArray.CopyTo(inputs, 0);
            double[] outputs = new double[Brain.OutputSignalArray.Length];
            Brain.OutputSignalArray.CopyTo(outputs, 0);

            if (Memory.Count >= MemorySize)
                Memory.RemoveFirst();

            Memory.AddLast(new StateActionReward(inputs, outputs, 0));

            return move;
        }
    }
}
