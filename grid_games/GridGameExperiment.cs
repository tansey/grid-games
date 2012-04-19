using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpNeat.Domains;
using System.Xml.Serialization;
using System.IO;
using SharpNeat.Genomes.Neat;
using System.Xml;
using SharpNeat.Core;
using SharpNeat.Network;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.DistanceMetrics;
using SharpNeat.SpeciationStrategies;
using System.Threading.Tasks;
using SharpNeat.EvolutionAlgorithms.ComplexityRegulation;
using SharpNeat.Phenomes;
using SharpNeat.Decoders.Neat;
using SharpNeat.Decoders;
using SharpNeat.Decoders.HyperNeat;

namespace grid_games
{
    public class GridGameExperiment 
    {
        public GridGameParameters Parameters { get; set; }
        public NeatGenomeParameters NeatParameters { get; set; }
        public NeatEvolutionAlgorithmParameters EvoParameters { get; set; }
        public IGenomeListEvaluator<NeatGenome> Evaluator { get; set; }
        public bool Condor { get; set; }

        public GridGameExperiment(string paramsFilename)
        {
            using (TextReader reader = new StreamReader(paramsFilename))
            {
                XmlSerializer ser = new XmlSerializer(typeof(GridGameParameters));
                Parameters = (GridGameParameters)ser.Deserialize(reader);
            }
            initialize();
        }

        public GridGameExperiment(GridGameParameters parameters)
        {
            Parameters = parameters;
            initialize();
        }

        private void initialize()
        {
            EvoParameters = new NeatEvolutionAlgorithmParameters()
            {
                SpecieCount = Parameters.Species
            };
            NeatParameters = new NeatGenomeParameters()
            {
                ActivationFn = PlainSigmoid.__DefaultInstance
            };
        }

        /// <summary>
        /// Load a population of genomes from an XmlReader and returns the genomes in a new list.
        /// The genome factory for the genomes can be obtained from any one of the genomes.
        /// </summary>
        public List<NeatGenome> LoadPopulation(XmlReader xr)
        {
            NeatGenomeFactory genomeFactory = (NeatGenomeFactory)CreateGenomeFactory();
            return NeatGenomeXmlIO.ReadCompleteGenomeList(xr, false, genomeFactory);
        }

        /// <summary>
        /// Save a population of genomes to an XmlWriter.
        /// </summary>
        public void SavePopulation(XmlWriter xw, IList<NeatGenome> genomeList)
        {
            // Writing node IDs is not necessary for NEAT.
            NeatGenomeXmlIO.WriteComplete(xw, genomeList, false);
        }

        /// <summary>
        /// Create a genome factory for the experiment.
        /// Create a genome factory with our neat genome parameters object and the appropriate number of input and output neuron genes.
        /// </summary>
        public IGenomeFactory<NeatGenome> CreateGenomeFactory()
        {
            return new NeatGenomeFactory(Parameters.Inputs, Parameters.Outputs, NeatParameters);
        }

        /// <summary>
        /// Create and return a NeatEvolutionAlgorithm object ready for running the NEAT algorithm/search. Various sub-parts
        /// of the algorithm are also constructed and connected up.
        /// This overload requires no parameters and uses the default population size.
        /// </summary>
        public NeatEvolutionAlgorithm<NeatGenome> CreateEvolutionAlgorithm()
        {
            return CreateEvolutionAlgorithm(Parameters.PopulationSize);
        }

        /// <summary>
        /// Create and return a NeatEvolutionAlgorithm object ready for running the NEAT algorithm/search. Various sub-parts
        /// of the algorithm are also constructed and connected up.
        /// This overload accepts a population size parameter that specifies how many genomes to create in an initial randomly
        /// generated population.
        /// </summary>
        public NeatEvolutionAlgorithm<NeatGenome> CreateEvolutionAlgorithm(int populationSize)
        {
            // Create a genome2 factory with our neat genome parameters object and the appropriate number of input and output neuron genes.
            IGenomeFactory<NeatGenome> genomeFactory = CreateGenomeFactory();

            // Create an initial population of randomly generated genomes.
            List<NeatGenome> genomeList = genomeFactory.CreateGenomeList(populationSize, 0);

            // Create evolution algorithm.
            return CreateEvolutionAlgorithm(genomeFactory, genomeList);
        }

        /// <summary>
        /// Create and return a NeatEvolutionAlgorithm object ready for running the NEAT algorithm/search. Various sub-parts
        /// of the algorithm are also constructed and connected up.
        /// This overload accepts a pre-built genome2 population and their associated/parent genome2 factory.
        /// </summary>
        public NeatEvolutionAlgorithm<NeatGenome> CreateEvolutionAlgorithm(IGenomeFactory<NeatGenome> genomeFactory, List<NeatGenome> genomeList)
        {
            // Create distance metric. Mismatched genes have a fixed distance of 10; for matched genes the distance is their weigth difference.
            IDistanceMetric distanceMetric = new ManhattanDistanceMetric(1.0, 0.0, 10.0);
            ISpeciationStrategy<NeatGenome> speciationStrategy = new ParallelKMeansClusteringStrategy<NeatGenome>(distanceMetric, new ParallelOptions());

            // Create complexity regulation strategy.
            IComplexityRegulationStrategy complexityRegulationStrategy = new NullComplexityRegulationStrategy();// ExperimentUtils.CreateComplexityRegulationStrategy(_complexityRegulationStr, _complexityThreshold);

            // Create the evolution algorithm.
            NeatEvolutionAlgorithm<NeatGenome> ea = new NeatEvolutionAlgorithm<NeatGenome>(EvoParameters, speciationStrategy, complexityRegulationStrategy);

            // Create a genome list evaluator. This packages up the genome decoder with the phenome evaluator.
            if(Evaluator == null)
                Evaluator = CreateEvaluator();

            // Initialize the evolution algorithm.
            ea.Initialize(Evaluator, genomeFactory, genomeList);

            // Finished. Return the evolution algorithm
            return ea;
        }

        public RoundRobinEvaluator<NeatGenome> CreateEvaluator()
        {
            // Create genome decoder.
            IGenomeDecoder<NeatGenome, IBlackBox> genomeDecoder = CreateGenomeDecoder();

            switch (Parameters.Evaluator.ToString().ToLower().Replace(" ", "").Replace("-", ""))
            {
                case "blondie":
                    {
                        IBlackBox brain;
                        using (XmlReader reader = XmlReader.Create(Parameters.OpponentPath))
                            brain = genomeDecoder.Decode(LoadPopulation(reader)[0]);
                        return new BenchmarkEvaluator<NeatGenome>(genomeDecoder, Parameters.CreateBlondieAgent(-1, brain), Parameters);
                    }
                case "minimax": return new BenchmarkEvaluator<NeatGenome>(genomeDecoder, Parameters.CreateMinimaxAgent(-1), Parameters);
                case "coevolve": return new RoundRobinEvaluator<NeatGenome>(genomeDecoder, Parameters);
                case "mcts": return new BenchmarkEvaluator<NeatGenome>(genomeDecoder, Parameters.CreateMctsAgent(-1, true), Parameters);
                case "randombenchmark":
                case "random": return new BenchmarkEvaluator<NeatGenome>(genomeDecoder, new RandomAgent(-1), Parameters);
                default:
                    break;
            }

            return null;
        }

        /// <summary>
        /// Creates the target agent for evaluating.
        /// </summary>
        /// <returns></returns>
        public IAgent CreateTargetAgent()
        {
            // Load the agent to benchmark
            var modelGenome = LoadPopulation(XmlReader.Create(Parameters.AgentPath))[0];
            var brain = CreateGenomeDecoder().Decode(modelGenome);
            if (Parameters.MctsNeat)
            {
                Console.WriteLine("Creating MCTS-NEAT agent");
                return Parameters.CreateMctsNeatAgent(1, brain);
            }

            Console.WriteLine("Found no target agent specifications. Returning random agent...");
            return new RandomAgent(1);
        }

        public IAgent CreateEvalAgent()
        {
            // Create the benchmark MCTS agent
            if (Parameters.Evaluator == "mcts")
            {
                Console.WriteLine("Creating MCTS benchmark agent");
                return Parameters.CreateMctsAgent(-1, true);
            }

            Console.WriteLine("Creating Random benchmark agent");
            return new RandomAgent(-1);
        }

        /// <summary>
        /// Creates a new genome decoder that can be used to convert a genome into a phenome.
        /// </summary>
        public IGenomeDecoder<NeatGenome, IBlackBox> CreateGenomeDecoder()
        {
            if (Parameters.HyperNeat)
                return CreateHyperNeatGenomeDecoder();

            NetworkActivationScheme activationScheme = NetworkActivationScheme.CreateCyclicFixedTimestepsScheme(4);

            return new NeatGenomeDecoder(activationScheme);
        }

        /// <summary>
        /// Creates a new HyperNEAT genome decoder that can be used to convert
        /// a genome into a phenome.
        /// </summary>
        public IGenomeDecoder<NeatGenome, IBlackBox> CreateHyperNeatGenomeDecoder()
        {

            var game = Parameters.GameFunction(null, null);
            int xTotal = game.Board.GetLength(0);
            int yTotal = game.Board.GetLength(1);

            // Create an input and an output layer for the HyperNEAT
            // substrate. Each layer corresponds to the game board
            // with 9 squares.
            SubstrateNodeSet inputLayer = new SubstrateNodeSet(Parameters.Inputs);
            SubstrateNodeSet outputLayer = new SubstrateNodeSet(Parameters.Outputs);

            // Each node in each layer needs a unique ID.
            uint uid = 1;

            // The game board is represented as a 2-dimensional plane.
            // Each square is at [0,...,1] in the x and y axis.
            // Thus, the game board for TicTacToe looks like this:
            //
            //  (0,1)  |  (0.5,1)  | (1,1)
            // (0,0.5) | (0.5,0.5) | (1,0.5)
            //  (0,0)  |  (0.5,0)  | (1,0)
            //
            for (int x = 0; x < xTotal; x++)
                for (int y = 0; y < yTotal; y++)
                    inputLayer.NodeList.Add(new SubstrateNode(uid++, new double[] { x, y }));

            for (int x = 0; x < xTotal; x++)
                for (int y = 0; y < yTotal; y++)
                    outputLayer.NodeList.Add(new SubstrateNode(uid++, new double[] { x, y }));

            List<SubstrateNodeSet> nodeSetList = new List<SubstrateNodeSet>(2);
            nodeSetList.Add(inputLayer);
            nodeSetList.Add(outputLayer);

            // Define a connection mapping from the input layer to the output layer.
            List<NodeSetMapping> nodeSetMappingList = new List<NodeSetMapping>(1);
            nodeSetMappingList.Add(NodeSetMapping.Create(0, 1, (double?)null));

            // Construct the substrate using a plain sigmoid as the phenome's
            // activation function. All weights under 0.2 will not generate
            // connections in the final phenome.
            List<ActivationFunctionInfo> fnList = new List<ActivationFunctionInfo>(5);
            fnList.Add(new ActivationFunctionInfo(0, 0.2, Linear.__DefaultInstance));
            fnList.Add(new ActivationFunctionInfo(1, 0.2, BipolarSigmoid.__DefaultInstance));
            fnList.Add(new ActivationFunctionInfo(2, 0.2, Gaussian.__DefaultInstance));
            fnList.Add(new ActivationFunctionInfo(3, 0.2, Sine.__DefaultInstance));
            fnList.Add(new ActivationFunctionInfo(4, 0.2, PlainSigmoid.__DefaultInstance));
            var activationFnLib = new DefaultActivationFunctionLibrary(fnList);
            Substrate substrate = new Substrate(nodeSetList,
                activationFnLib,
                4, 0.2, 5, nodeSetMappingList);

            // Create genome decoder. Decodes to a neural network packaged with
            // an activation scheme that defines a fixed number of activations per evaluation.
            IGenomeDecoder<NeatGenome, IBlackBox> genomeDecoder =
                new HyperNeatDecoder(substrate, 
                                     NetworkActivationScheme.CreateCyclicFixedTimestepsScheme(4),
                                     NetworkActivationScheme.CreateCyclicFixedTimestepsScheme(4),
                                     false);

            return genomeDecoder;
        }
    }
}
