using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpNeat.Genomes.Neat;
using SharpNeat.Core;
using SharpNeat.Phenomes;

namespace grid_games
{
    public class MinimaxBenchmarkEvaluator<TGenome> : GridGameEvaluator<TGenome>
        where TGenome : NeatGenome, global::SharpNeat.Core.IGenome<TGenome>
    {
        IAgent _minimax;

        public MinimaxBenchmarkEvaluator(IGenomeDecoder<TGenome, IBlackBox> genomeDecoder,
                                GridGameParameters parameters) : base(genomeDecoder, parameters)
        {
            _minimax = parameters.CreateMinimaxAgent(-1);
        }

        protected override void evaluateAgents()
        {
            double[] scores = new double[_agents.Length];
            
            for (int i = 0; i < _agents.Length; i++)
            {
                int winner = evaluate(_agents[i], _minimax);

                if (winner == 1)
                    scores[i] += _params.WinReward;
                else if (winner == 0)
                    scores[i] += _params.TieReward;

                winner = evaluate(_minimax, _agents[i]);

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
