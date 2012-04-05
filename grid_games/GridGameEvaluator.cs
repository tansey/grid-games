using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpNeat.Core;
using SharpNeat.Genomes.Neat;
using SharpNeat.Phenomes;

namespace grid_games
{
    public class GridGameEvaluator<TGenome> : IGenomeListEvaluator<TGenome>
        where TGenome : NeatGenome, global::SharpNeat.Core.IGenome<TGenome>
    {
        readonly IGenomeDecoder<TGenome, IBlackBox> _genomeDecoder;
        ulong _evaluationCount;
        IAgent[] _agents;
        IList<NeatGenome> _genomeList;
        GridGameParameters _params;

        public bool StopConditionSatisfied { get { return false; } }

        public GridGameEvaluator(IGenomeDecoder<TGenome, IBlackBox> genomeDecoder,
                                GridGameParameters parameters)
        {
            _genomeDecoder = genomeDecoder;
            _params = parameters;
            _evaluationCount = 0;
        }
        
        public void Evaluate(IList<TGenome> genomeList)
        {
            _genomeList = (IList<NeatGenome>)genomeList;
            _agents = new IAgent[genomeList.Count];

            // Convert the genomes to phenomes (agents)
            createAgents(genomeList);


            // TODO: 
            //  - Do we play multiple round-robins?
            //  - Do we step everyone forward 1 move at a time?
            //  - Do we randomize the step order?
            // Play a round-robin game
            double[] scores = new double[_agents.Length];
            for(int heroIdx = 0; heroIdx < _agents.Length; heroIdx++)
                for (int villainIdx = 0; villainIdx < _agents.Length; villainIdx++)
                {
                    if (heroIdx == villainIdx)
                        continue;

                    int winner = evaluate(_agents[heroIdx], _agents[villainIdx]);

                    updateScores(scores, heroIdx, villainIdx, winner);
                }

            // TODO:
            //  - Lamarckian evolution would go here
        }

        private void updateScores(double[] scores, int heroIdx, int villainIdx, int winner)
        {
            if (winner == 1)
            {
                observeWinner(heroIdx);
                scores[heroIdx] += _params.WinReward;
                scores[villainIdx] += _params.LossReward;
            }
            else if (winner == -1)
            {
                observeWinner(villainIdx);
                scores[heroIdx] += _params.LossReward;
                scores[villainIdx] += _params.WinReward;
            }
            else
            {
                scores[heroIdx] += _params.TieReward;
                scores[villainIdx] += _params.TieReward;
            }
        }

        private void observeWinner(int winnerIdx)
        {
            // TODO: Social learning goes here
        }

        private int evaluate(IAgent hero, IAgent villain)
        {
            GridGame game = _params.GameFunction(hero, villain);

            game.PlayToEnd();

            _evaluationCount++;

            return game.Winner;
        }

        private void createAgents(IList<TGenome> genomeList)
        {
            for (int i = 0; i < _agents.Length; i++)
            {
                // Decode the genome.
                IBlackBox phenome = _genomeDecoder.Decode(genomeList[i]);

                // Check that the genome is valid.
                if (phenome == null)
                {
                    Console.WriteLine("Couldn't decode genome {0}!", i);
                    _agents[i] = new RandomAgent(i);
                }
                else
                    _agents[i] = new NeuralAgent(i, phenome);
            }
        }

        public ulong EvaluationCount
        {
            get { return _evaluationCount; }
        }

        public void Reset()
        {
            
        }

        
    }
}
