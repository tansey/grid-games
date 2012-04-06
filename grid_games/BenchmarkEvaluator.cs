using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpNeat.Genomes.Neat;
using SharpNeat.Core;
using SharpNeat.Phenomes;

namespace grid_games
{
    public class BenchmarkEvaluator<TGenome> : RoundRobinEvaluator<TGenome>
        where TGenome : NeatGenome, global::SharpNeat.Core.IGenome<TGenome>
    {
        IAgent _opponent;

        public BenchmarkEvaluator(IGenomeDecoder<TGenome, IBlackBox> genomeDecoder,
                                IAgent opponent,
                                GridGameParameters parameters) : base(genomeDecoder, parameters)
        {
            _opponent = opponent;
        }

        protected override void evaluateAgents()
        {
            double[] scores = new double[_agents.Length];

            // Play N matches against the benchmark opponent
            for(int round = 0 ; round < _params.MatchesPerOpponent; round++)
                for (int i = 0; i < _agents.Length; i++)
                {
                    int winner = evaluate(_agents[i], _opponent);

                    if (winner == 1)
                        scores[i] += _params.WinReward;
                    else if (winner == 0)
                        scores[i] += _params.TieReward;

                    winner = evaluate(_opponent, _agents[i]);

                    if (winner == -1)
                        scores[i] += _params.WinReward;
                    else if (winner == 0)
                        scores[i] += _params.TieReward;
                }

            // copy the scores to the genome fitnesses
            for (int i = 0; i < scores.Length; i++)
            {
                _genomeList[i].EvaluationInfo.SetFitness(scores[i]);
                _genomeList[i].EvaluationInfo.AlternativeFitness = scores[i];
            }
        }
    }
}
