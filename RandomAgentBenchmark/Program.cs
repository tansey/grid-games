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

namespace RandomAgentBenchmark
{
    class Program
    {
        const string EXPERIMENT_DIR = "../../../experiments/random/tictactoe/";
        static GridGameExperiment experiment;
        static NeatEvolutionAlgorithm<NeatGenome> ea;
        static bool finished = false;
        
        static void Main(string[] args)
        {
            GridGameParameters gg = new GridGameParameters()
            {
                Name = "Random Benchmark",
                Description = "A baseline experiment to validate that the agents can evolve to beat a random player.",
                Game = "Tic-Tac-Toe",
                Evaluator = "Random",
                Generations = 500,
                Inputs = 9,
                Outputs = 9,
                WinReward = 2,
                TieReward = 1,
                LossReward = 0,
                PopulationSize = 100,
                Species = 10
            };
            using (TextWriter writer = new StreamWriter(EXPERIMENT_DIR + "config.xml"))
            {
                XmlSerializer ser = new XmlSerializer(typeof(GridGameParameters));
                ser.Serialize(writer, gg);
            }

            experiment = new GridGameExperiment(EXPERIMENT_DIR + "config.xml");
            ea = experiment.CreateEvolutionAlgorithm();
            ea.UpdateScheme = new SharpNeat.Core.UpdateScheme(1);
            ea.UpdateEvent += new EventHandler(ea_UpdateEvent);
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
            using (TextWriter writer = new StreamWriter(EXPERIMENT_DIR + "results.csv", true))
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
