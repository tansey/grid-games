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

namespace grid_games
{
    public class GridGameExperiment 
    {
        public GridGameParameters Parameters { get; set; }
        public NeatGenomeParameters NeatParameters { get; set; }
        public NeatEvolutionAlgorithmParameters EvoParameters { get; set; }
        public GridGameEvaluator<NeatGenome> Evaluator { get; set; }

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
            Evaluator = CreateEvaluator();

            // Initialize the evolution algorithm.
            ea.Initialize(Evaluator, genomeFactory, genomeList);

            // Finished. Return the evolution algorithm
            return ea;
        }

        public GridGameEvaluator<NeatGenome> CreateEvaluator()
        {
            // Create genome decoder.
            IGenomeDecoder<NeatGenome, IBlackBox> genomeDecoder = CreateGenomeDecoder();

            switch (Parameters.Evaluator.ToString().ToLower().Replace(" ", "").Replace("-", ""))
            {
                case "minimax": return new MinimaxBenchmarkEvaluator<NeatGenome>(genomeDecoder, Parameters);
                case "roundrobin": return new GridGameEvaluator<NeatGenome>(genomeDecoder, Parameters);
                case "randombenchmark":
                case "random": return new RandomBenchmarkEvaluator<NeatGenome>(genomeDecoder, Parameters);
                default:
                    break;
            }

            return null;
        }

        /// <summary>
        /// Creates a new genome decoder that can be used to convert a genome into a phenome.
        /// </summary>
        public IGenomeDecoder<NeatGenome, IBlackBox> CreateGenomeDecoder()
        {
            NetworkActivationScheme activationScheme = NetworkActivationScheme.CreateCyclicFixedTimestepsScheme(4);

            return new NeatGenomeDecoder(activationScheme);
        }
    }
}
