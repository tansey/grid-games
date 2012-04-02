using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace grid_games
{
    public abstract class Agent : IAgent
    {
        /// <summary>
        /// Either 1 or -1 for 2-player games
        /// </summary>
        public int PlayerId { get; set; }

        /// <summary>
        /// The unique ID of the agent.
        /// </summary>
        public int AgentId { get; set; }

        public Agent(int id)
        {
            AgentId = id;
        }

        public abstract Move GetMove(int[,] board, bool[,] validNextMoves);
    }
}
