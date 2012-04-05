using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpNeat.Genomes.Neat;
using SharpNeat.Core;
using SharpNeat.Phenomes;

namespace grid_games
{
    public class RandomBenchmarkEvaluator<TGenome> : GridGameEvaluator<TGenome>
        where TGenome : NeatGenome, global::SharpNeat.Core.IGenome<TGenome>
    {
        public RandomBenchmarkEvaluator(IGenomeDecoder<TGenome, IBlackBox> genomeDecoder,
                                GridGameParameters parameters) : base(genomeDecoder, parameters)
        {
        }

        protected override void evaluateAgents()
        {
            IAgent randomAgent = new RandomAgent(-1);
            double[] scores = new double[_agents.Length];
            for (int i = 0; i < _agents.Length; i++)
            {
                int winner = evaluate(_agents[i], randomAgent);

                if (winner == 1)
                    scores[i] += _params.WinReward;
                else if (winner == 0)
                    scores[i] += _params.TieReward;

                winner = evaluate(randomAgent, _agents[i]);

                if (winner == -1)
                    scores[i] += _params.WinReward;
                else if (winner == 0)
                    scores[i] += _params.TieReward;
            }

            for (int i = 0; i < scores.Length; i++)
            {
                _genomeList[i].EvaluationInfo.SetFitness(scores[i]);
                _genomeList[i].EvaluationInfo.AlternativeFitness = scores[i];
            }
        }
    }
}
