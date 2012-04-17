using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpNeat.Core;
using SharpNeat.Genomes.Neat;
using SharpNeat.Phenomes;
using SharpNeat.Phenomes.NeuralNets;
using System.Diagnostics;

namespace grid_games
{
    public class RoundRobinEvaluator<TGenome> : IGenomeListEvaluator<TGenome>
        where TGenome : NeatGenome, global::SharpNeat.Core.IGenome<TGenome>
    {
        protected readonly IGenomeDecoder<TGenome, IBlackBox> _genomeDecoder;
        protected ulong _evaluationCount;
        protected IAgent[] _agents;
        protected IList<NeatGenome> _genomeList;
        protected int[] _subcultures;
        protected GridGameParameters _params;
        protected int _generation;
        protected Random _random;

        public int CurrentMemorySize { get; set; }

        public bool StopConditionSatisfied { get { return false; } }

        public RoundRobinEvaluator(IGenomeDecoder<TGenome, IBlackBox> genomeDecoder,
                                GridGameParameters parameters)
        {
            _genomeDecoder = genomeDecoder;
            _params = parameters;
            _evaluationCount = 0;

            CurrentMemorySize = 1;

            _random = new Random();
        }
        
        public void Evaluate(IList<TGenome> genomeList)
        {
            _genomeList = (IList<NeatGenome>)genomeList;
            _agents = new IAgent[genomeList.Count];

            // Convert the genomes to phenomes (agents)
            createAgents(genomeList);

            // Play the networks against each other
            evaluateAgents();

            if(_params.SocialAgents && _params.LamarckianEvolution)
                PerformLamarckianEvolution(genomeList, a => (FastCyclicNetwork)((SocialAgent)a).Brain);

            _generation++;

            if (_params.SocialAgents 
                && _generation % _params.GenerationsPerMemoryIncrement == 0
                && (_params.MaxMemorySize == 0 || CurrentMemorySize < _params.MaxMemorySize))
                CurrentMemorySize++;
        }

        

        protected virtual void evaluateAgents()
        {
            // TODO: 
            //  - Do we play multiple round-robins?
            //  - Do we step everyone forward 1 move at a time?
            //  - Do we randomize the step order?

            IAgent[] villains = new IAgent[_params.RoundRobinOpponents];
            List<IAgent> tempAgents = _agents.ToList();
            for (int i = 0; i < villains.Length; i++)
            {
                int chosenIdx = _random.Next(tempAgents.Count);
                villains[i] = tempAgents[chosenIdx];
                tempAgents.RemoveAt(chosenIdx);
            }

            // Play a round-robin game
            double[] scores = new double[_agents.Length];
            for (int heroIdx = 0; heroIdx < _agents.Length; heroIdx++)
                for (int villainIdx = 0; villainIdx < villains.Length; villainIdx++)
                {
                    Console.WriteLine("{0} vs. {1}", heroIdx, villainIdx);
                    if (heroIdx == villainIdx)
                        continue;

                    int winner = evaluate(_agents[heroIdx], villains[villainIdx]);

                    updateScores(scores, heroIdx, villainIdx, winner);
                }

            for (int i = 0; i < scores.Length; i++)
            {
                _genomeList[i].EvaluationInfo.SetFitness(scores[i]);
                _genomeList[i].EvaluationInfo.AlternativeFitness = scores[i];
            }
        }

        protected int evaluate(IAgent hero, IAgent villain)
        {
            GridGame game = _params.GameFunction(hero, villain);

            game.PlayToEnd();

            _evaluationCount++;

            return game.Winner;
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
            if (!_params.SocialAgents)
                return;

            var observation = ((SocialAgent)_agents[winnerIdx]).Memory;
            for (int i = 0; i < _agents.Length; i++)
            {
                // Do not train yourself or anyone outside your subculture
                if (i == winnerIdx || _subcultures[i] != _subcultures[winnerIdx])
                    continue;

                var observer = (SocialAgent)_agents[i];
                observer.LearnFromObservation(observation);
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
                else if (_params.SocialAgents)
                    _agents[i] = new SocialAgent(i, phenome, CurrentMemorySize);
                else if (_params.BlondieAgents)
                    _agents[i] = _params.CreateBlondieAgent(i, phenome);
                else if (_params.MctsNeat)
                    _agents[i] = _params.CreateMctsNeatAgent(i, phenome);
                else
                    _agents[i] = new NeuralAgent(i, phenome);
            }

            if (_params.SocialAgents)
                CreateSubcultures();
        }

        /// <summary>
        /// Puts the agents into equal-sized subcultural groups.
        /// 
        /// <param name="numGroups">The number of groups to divide the population into</param>
        /// </summary>
        private void CreateSubcultures()
        {
            int minGroupSize = _agents.Length / _params.Subcultures;

            _subcultures = new int[_agents.Length];

            // Create a list of all the teacher IDs
            List<int> agentIds = new List<int>();
            for (int i = 0; i < _subcultures.Length; i++)
            {
                _subcultures[i] = -1;
                agentIds.Add(i);
            }

            // Select each ID for each subculture randomly
            for (int i = 0; i < _subcultures.Length; i++)
            {
                int temp = _random.Next(agentIds.Count);
                int idx = agentIds[temp];
                _subcultures[idx] = i % _params.Subcultures;
                agentIds.RemoveAt(temp);
            }

            Debug.Assert(_subcultures.GroupBy(g => g).Min(ag => ag.Count()) >= minGroupSize);
            Debug.Assert(_subcultures.GroupBy(g => g).Max(ag => ag.Count()) <= minGroupSize + 1);
            Debug.Assert(_subcultures.GroupBy(g => g).Count() == _params.Subcultures);
        }

        public ulong EvaluationCount
        {
            get { return _evaluationCount; }
        }

        public void Reset()
        {
            
        }

        /// <summary>
        /// Saves all phenotypic progress back to the genomes.
        /// </summary>
        private void PerformLamarckianEvolution(IList<TGenome> genomeList, Func<IAgent, FastCyclicNetwork> networkSelector)
        {
            for (int i = 0; i < _agents.Length; i++)
            {
                var agent = _agents[i];

                // Get the network for this teacher
                var network = networkSelector(agent);

                // Get the genome for this teacher
                var genome = (NeatGenome)genomeList[i];

                // Update the genome to match the phenome weights
                foreach (var conn in network.ConnectionArray)
                {
                    var genomeConn = (ConnectionGene)genome.ConnectionList.First(g => g.SourceNodeId == genome.NodeList[conn._srcNeuronIdx].Id && g.TargetNodeId == genome.NodeList[conn._tgtNeuronIdx].Id);
                    genomeConn.Weight = conn._weight;
                }
            }
        }
        
    }
}
