using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace grid_games
{
    public class StateActionReward
    {
        public readonly double[] State;
        public readonly double[] Action;
        public double Reward;

        public StateActionReward(double[] inputs, double[] outputs, double reward)
        {
            State = inputs;
            Action = outputs;
            Reward = reward;
        }
    }
}
