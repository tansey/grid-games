﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using grid_games;
using System.IO;
using System.Xml.Serialization;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;
using System.Threading;

namespace CondorEvolution
{
    class Program
    {
        static GridGameExperiment experiment;
        static NeatEvolutionAlgorithm<NeatGenome> ea;
        static bool finished = false;

        static void Main(string[] args)
        {
            GridGameParameters gg = GridGameParameters.GetParameters(args);
            if (gg == null)
                return;

            experiment = new GridGameExperiment(gg.ConfigPath);
            experiment.Evaluator = new CondorEvaluator(gg, CondorGroup.Grad);
            ea = experiment.CreateEvolutionAlgorithm();
            ea.UpdateScheme = new SharpNeat.Core.UpdateScheme(1);
            ea.UpdateEvent += new EventHandler(ea_UpdateEvent);

            using (TextWriter writer = new StreamWriter(gg.ResultsPath))
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
            using (TextWriter writer = new StreamWriter(experiment.Parameters.ResultsPath, true))
                writer.WriteLine(generation + "," + topFitness + "," + averageFitness);

            // Save the best genome to file
            // TODO: Should we save funcIds here?
            //       I think we need to if it's HyperNEAT, but not if it's regular NEAT.
            var doc = NeatGenomeXmlIO.SaveComplete(new List<NeatGenome>() { ea.CurrentChampGenome }, true);
            doc.Save(string.Format(experiment.Parameters.ChampionPath, generation));

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
