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
        static string EXPERIMENT_DIR = "../../../experiments/";
        static string RESULTS_FILE;
        static string CONFIG_FILE;
        static GridGameExperiment experiment;
        static NeatEvolutionAlgorithm<NeatGenome> ea;
        static bool finished = false;

        static Dictionary<string, string> optionalParameters = new Dictionary<string, string>();
        
        static void Main(string[] args)
        {
            GridGameParameters gg = GetParameters(args);
            if (gg == null)
                return;

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

        static GridGameParameters GetParameters(string[] args)
        {
            if (args.Length == 0)
            {
                PrintHelp();
                return null;
            }

            string offset = args[0];

            GridGameParameters gg = new GridGameParameters()
            {
                Name = "Random Benchmark",
                Description = "A baseline experiment to validate that the agents can evolve to beat a random player.",
                Game = "tictactoe",
                Inputs = 9,
                Outputs = 9,
                Evaluator = "random",
                WinReward = 2,
                TieReward = 1,
                LossReward = 0,
                Generations = 500,
                PopulationSize = 100,
                Species = 10,
                SocialAgents = false,
                LamarckianEvolution = false,
                Subcultures = 10,
                GenerationsPerMemoryIncrement = 20,
                MaxMemorySize = 0
            };
            
            EXPERIMENT_DIR += string.Format("{0}/{1}/", gg.Game, gg.Evaluator);
            RESULTS_FILE = EXPERIMENT_DIR + offset + "_results.csv";
            CONFIG_FILE = EXPERIMENT_DIR + offset + "_config.xml";

            
            for (int i = 1; i < args.Length; i++)
            {
                if (args[i][0] != '-')
                {
                    Console.WriteLine("Invalid option: '{0}'. Options must be prefixed with '-'", args[i]);
                    return null;
                }

                switch (args[i].Substring(1).Trim().ToLower())
                {
                    case "help": PrintHelp(); return null;

                    case "game":
                    case "g": 
                        string game = args[++i];
                        gg.Game = game.Trim().ToLower();
                        if (gg.GameFunction == null)
                        {
                            Console.WriteLine("Invalid game: '{0}'. Options are tictactoe, connect4, reversi.", game);
                            return null;
                        }
                        break;

                    case "evaluator":
                    case "eval":
                    case "e":
                        string eval = args[++i];
                        gg.Evaluator = eval;
                        if (gg.Evaluator != "roundrobin" && gg.Evaluator != "random" && gg.Evaluator != "minimax")
                        {
                            Console.WriteLine("Invalid evaluator: '{0}'. Options are random, roundrobin, minimax.", eval);
                            return null;
                        }
                        break;
                    case "inputs":
                    case "i":
                        int inputs;
                        if (!int.TryParse(args[++i], out inputs))
                        {
                            Console.WriteLine("Invalid input neuron count: '{0}'.", args[i]);
                            return null;
                        }
                        gg.Inputs = inputs;
                        break;

                    case "outputs":
                    case "o":
                        int outputs;
                        if (!int.TryParse(args[++i], out outputs))
                        {
                            Console.WriteLine("Invalid output neuron count: '{0}'.", args[i]);
                            return null;
                        }
                        gg.Outputs = outputs;
                        break;

                    case "name":
                    case "n": gg.Name = args[++i];
                        break;

                    case "winreward":
                    case "win": 
                        double win;
                        if (!double.TryParse(args[++i], out win))
                        {
                            Console.WriteLine("Invalid win reward: '{0}'.", args[i]);
                            return null;
                        }
                        gg.WinReward = win;
                        break;

                    case "tiereward":
                    case "tie":
                        double tie;
                        if (!double.TryParse(args[++i], out tie))
                        {
                            Console.WriteLine("Invalid tie reward: '{0}'.", args[i]);
                            return null;
                        }
                        gg.TieReward = tie;
                        break;

                    case "losereward":
                    case "lossreward":
                    case "lose":
                    case "loss":
                        double lose;
                        if (!double.TryParse(args[++i], out lose))
                        {
                            Console.WriteLine("Invalid lose reward: '{0}'.", args[i]);
                            return null;
                        }
                        gg.LossReward = lose;
                        break;

                    case "generations":
                    case "gens":
                        int gens;
                        if (!int.TryParse(args[++i], out gens))
                        {
                            Console.WriteLine("Invalid generations: '{0}'.", args[i]);
                            return null;
                        }
                        gg.Generations = gens;
                        break;

                    case "popsize":
                    case "pop":
                        int pop;
                        if (!int.TryParse(args[++i], out pop))
                        {
                            Console.WriteLine("Invalid population size: '{0}'.", args[i]);
                            return null;
                        }
                        gg.PopulationSize = pop;
                        break;

                    case "species":
                        int species;
                        if (!int.TryParse(args[++i], out species))
                        {
                            Console.WriteLine("Invalid species count: '{0}'.", args[i]);
                            return null;
                        }
                        gg.Species = species;
                        break;

                    case "subcultures":
                    case "subs":
                    case "sub":
                        int subs;
                        if (!int.TryParse(args[++i], out subs))
                        {
                            Console.WriteLine("Invalid subculture count: '{0}'.", args[i]);
                            return null;
                        }
                        gg.Subcultures = subs;
                        break;

                    case "memoryincrement":
                    case "meminc":
                        int mem;
                        if (!int.TryParse(args[++i], out mem))
                        {
                            Console.WriteLine("Invalid generations per memory increment value: '{0}'.", args[i]);
                            return null;
                        }
                        gg.GenerationsPerMemoryIncrement = mem;
                        break;

                    case "maxmemory":
                    case "maxmem":
                        int maxmem;
                        if (!int.TryParse(args[++i], out maxmem))
                        {
                            Console.WriteLine("Invalid maximum memory size: '{0}'.", args[i]);
                            return null;
                        }
                        gg.MaxMemorySize = maxmem;
                        break;

                    case "social":
                        gg.SocialAgents = true;
                        break;
                    case "lamarckian":
                    case "lamarck":
                        gg.LamarckianEvolution = true;
                        break;

                    case "dir":
                        string dir = args[++i];
                        if (!dir.EndsWith("/"))
                            dir += "/";
                        EXPERIMENT_DIR = dir; 
                        RESULTS_FILE = EXPERIMENT_DIR + offset + "_results.csv";
                        CONFIG_FILE = EXPERIMENT_DIR + offset + "_config.xml";
                        break;

                    case "depth":
                        int depth;
                        if (!int.TryParse(args[++i], out depth))
                        {
                            Console.WriteLine("Invalid depth size: '{0}'.", args[i]);
                            return null;
                        }
                        gg.MinimaxDepth = depth;
                        break;

                    default:
                        Console.WriteLine("Invalid option: '{0}'. Option unknown. Use -help to see options.", args[i].Substring(1));
                        return null;
                }
            }

            return gg;
            
        }

        static void PrintHelp()
        {
            Console.WriteLine("Usage: AgentBenchmark.exe <offset> [-options...]");
            Console.WriteLine("<offset>".PadRight(25) + "File to use when saving the config and results files.");
            Console.WriteLine("-help".PadRight(25) + "Prints the usage summary.");
            Console.WriteLine("-game -g".PadRight(25) + "Name of the game. Valid options: tictactoe, connect4, reversi. Default: tictactoe");
            Console.WriteLine("-inputs -i".PadRight(25) + "Number of inputs for the neural network (usually # of board spaces). Default: 9");
            Console.WriteLine("-outputs -o".PadRight(25) + "Number of outputs for the neural network (usually # of board spaces). Default: 9");
            Console.WriteLine("-evaluator -eval -e".PadRight(25) + "Evaluation function to use. Valid options: random, roundrobin, minimax. Default: random");
            Console.WriteLine("-name -n".PadRight(25) + "Name of the experiment.");
            Console.WriteLine("-winreward -win".PadRight(25) + "Reward an agent receives for winning a game. Default: 2");
            Console.WriteLine("-tiereward -tie".PadRight(25) + "Reward an agent receives for tying a game. Default: 1");
            Console.WriteLine("-losereward -lose".PadRight(25) + "Reward an agent receives for losing a game. Default: 0");
            Console.WriteLine("-generations -gens".PadRight(25) + "Number of generations to evolve. Default: 500");
            Console.WriteLine("-popsize -pop".PadRight(25) + "Size of the population. Default: 100");
            Console.WriteLine("-species".PadRight(25) + "Number of species to divide the genomes into. This is the NEAT parameter. Default: 10");
            Console.WriteLine("-social".PadRight(25) + "Use social learning (ESL) agents. Default: false (plain neuroevolution)");
            Console.WriteLine("-lamarckian -lamarck".PadRight(25) + "Use Lamarckian evolution. Default: false (Darwinian evolution)");
            Console.WriteLine("-subcultures -subs -sub".PadRight(25) + "Number of subcultures to divide the agents into. This is the ESL parameter. Default: 10");
            Console.WriteLine("-memoryincrement -meminc".PadRight(25) + "Number of generations between increments of the memory size. Default: 20");
            Console.WriteLine("-maxmemory -maxmem".PadRight(25) + "Maximum size of the memory window for social agents. Default: 0 (unlimited)");
            Console.WriteLine("-dir".PadRight(25) + "Directory to save the experiment output. Default: ../../../experiments/[game]/[eval]");
            Console.WriteLine("-depth".PadRight(25) + "Maximum search depth for minimax agents. Default: 9");
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
