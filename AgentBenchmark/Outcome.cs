using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AgentBenchmark
{
    struct Outcome
    {
        public readonly int Winner;
        public readonly int AgentId;
        public readonly float AverageTurnTime;
        public readonly int TotalMoves;

        public Outcome(int winner, int agentId, float avgTurnTime, int totalMoves)
        {
            Winner = winner;
            AgentId = agentId;
            AverageTurnTime = avgTurnTime;
            TotalMoves = totalMoves;
        }

        public int BenchmarkId { get { return AgentId * -1; } }
    }
}
