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
        readonly Func<IAgent, IAgent, GridGame> _createGame;
        ulong _evaluationCount;
        IAgent[] _agents;
        IList<NeatGenome> _genomeList;

        public bool StopConditionSatisfied { get { return false; } }

        public GridGameEvaluator(IGenomeDecoder<TGenome, IBlackBox> genomeDecoder,
                                Func<IAgent, IAgent, GridGame> createGame)
        {
            _genomeDecoder = genomeDecoder;
            _createGame = createGame;
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
            for(int heroIdx = 0; heroIdx < _agents.Length; heroIdx++)
                for (int villainIdx = 0; villainIdx < _agents.Length; villainIdx++)
                {
                    if (heroIdx == villainIdx)
                        continue;


                }
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
