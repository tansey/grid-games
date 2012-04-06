using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using grid_games;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;
using System.IO;
using System.Threading;
using System.Xml.Serialization;

namespace AgentBenchmark
{
    class Program
    {
        static string EXPERIMENT_DIR = "../../../experiments/random/";
        static string RESULTS_FILE;
        static string CONFIG_FILE;
        static GridGameExperiment experiment;
        static NeatEvolutionAlgorithm<NeatGenome> ea;
        static bool finished = false;
        
        static void Main(string[] args)
        {
            if (args.Length != 5)
            {
                Console.WriteLine("Usage: AgentBenchmark.exe <game> <inputs> <outputs> <evaluator>");
                Console.WriteLine("<game>".PadRight(15) + "Name of the game. Valid options: tictactoe, connect4, reversi.");
                Console.WriteLine("<inputs>".PadRight(15) + "Number of inputs for the neural network (usually # of board spaces).");
                Console.WriteLine("<outputs>".PadRight(15) + "Number of outputs for the neural network (usually # of board spaces).");
                Console.WriteLine("<evaluator>".PadRight(15) + "Evaluation function to use. Valid options: random, roundrobin, minimax.");
                Console.WriteLine("<offset>".PadRight(15) + "File to use when saving the config and results files.");
                return;
            }

            EXPERIMENT_DIR += args[0] + "/";
            RESULTS_FILE = EXPERIMENT_DIR + args[4] + "_results.csv";
            CONFIG_FILE = EXPERIMENT_DIR + args[4] + "_config.xml";

            GridGameParameters gg = new GridGameParameters()
            {
                Name = "Random Benchmark",
                Description = "A baseline experiment to validate that the agents can evolve to beat a random player.",
                Game = args[0],
                Inputs = int.Parse(args[1]),
                Outputs = int.Parse(args[2]),
                Evaluator = args[3],
                WinReward = 2,
                TieReward = 1,
                LossReward = 0,
                Generations = 500,
                PopulationSize = 100,
                Species = 10,
                SocialAgents = true,
                LamarckianEvolution = true,
                Subcultures = 10,
                GenerationsPerMemoryIncrement = 1,
                MaxMemorySize = 0
            };
            using (TextWriter writer = new StreamWriter(CONFIG_FILE))
            {
                XmlSerializer ser = new XmlSerializer(typeof(GridGameParameters));
                ser.Serialize(writer, gg);
            }

            experiment = new GridGameExperiment(CONFIG_FILE);
            ea = experiment.CreateEvolutionAlgorithm();
            ea.UpdateScheme = new SharpNeat.Core.UpdateScheme(1);
            ea.UpdateEvent += new EventHandler(ea_UpdateEvent);

            using (TextWriter writer = new StreamWriter(RESULTS_FILE))
                writer.WriteLine("Generation,Best,Average");

            ea.StartContinue();

            while (!finished) Thread.Sleep(1000);
        }

        static void ea_UpdateEvent(object sender, EventArgs e)
        {
            // If this run has already finished, don't log anything.
            // This is needed because SharpNEAT calls this an extra
            // time when the algorithm is stopped.
            if (finished)
                return;

            // The average fitness of each genome.
            double averageFitness = ea.GenomeList.Average(x => x.EvaluationInfo.Fitness);

            // The fitness of the best individual in the population.
            double topFitness = ea.CurrentChampGenome.EvaluationInfo.Fitness;

            // The generation that just completed.
            int generation = (int)ea.CurrentGeneration;

            // Write the progress to the console.
            Console.WriteLine("Generation: {0} Best: {1} Avg: {2}",
                               generation, topFitness, averageFitness);

            // Append the progress to the results file in CSV format.
            using (TextWriter writer = new StreamWriter(RESULTS_FILE, true))
                writer.WriteLine(generation + "," + averageFitness + "," + topFitness);

            // Stop if we've evolved for enough generations
            if (ea.CurrentGeneration >= experiment.Parameters.Generations)
            {
                ea.Stop();
                finished = true;
                Console.WriteLine("Finished!");
            }
        }
    }
}
