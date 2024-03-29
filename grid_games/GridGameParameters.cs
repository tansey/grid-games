﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpNeat.Decoders;
using grid_games.TicTacToe;
using grid_games.ConnectFour;
using grid_games.Reversi;
using System.Xml.Serialization;
using SharpNeat.Phenomes;
using System.Diagnostics;

namespace grid_games
{
    public class GridGameParameters
    {
        public string Game { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double WinReward { get; set; }
        public double LossReward { get; set; }
        public double TieReward { get; set; }
        public int PopulationSize { get; set; }
        public int Inputs { get; set; }
        public int Outputs { get; set; }
        public int Species { get; set; }
        public int Subcultures { get; set; }
        public int Generations { get; set; }
        public string Evaluator { get; set; }
        public int GenerationsPerMemoryIncrement { get; set; }
        public int MaxMemorySize { get; set; }
        public bool SocialAgents { get; set; }
        public bool LamarckianEvolution { get; set; }
        public int MinimaxDepth { get; set; }
        public string ExperimentPath { get; set; }
        public string ResultsPath { get; set; }
        public string ConfigPath { get; set; }
        public string ChampionPath { get; set; }
        public string OpponentPath { get; set; }
        public string AgentPath { get; set; }
        public string BenchmarkResultsPath { get; set; }
        public string BenchmarkGameLogPath { get; set; }
        public bool BlondieAgents { get; set; }
        public int MatchesPerOpponent { get; set; }
        public int MonteCarloTrials { get; set; }
        public int MinMcTrialsPerMove { get; set; }
        public double UctConst { get; set; }
        public bool HyperNeat { get; set; }
        public bool MctsNeat { get; set; }
        public int RoundRobinOpponents { get; set; }
        public int EvaluatorMonteCarloTrials { get; set; }
        public int EvaluatorMinMcTrialsPerMove { get; set; }
        public double EvaluatorUctConst { get; set; }
        

        /// <summary>
        /// Returns a function that creates a new grid game.
        /// </summary>
        [XmlIgnore]
        public Func<IAgent, IAgent, GridGame> GameFunction
        {
            get
            {
                switch (Game.ToLower())
                {
                    case "tic-tac-toe":
                    case "tictactoe":
                    case "tic tac toe": return (hero, villain) => new TicTacToeGame(hero, villain);

                    case "connect four":
                    case "connect4":
                    case "connect 4":
                    case "connect-four":
                    case "connect-4":
                    case "connectfour": return (hero, villain) => new ConnectFourGame(hero, villain);

                    case "reversi":
                    case "othello": return (hero, villain) => new ReversiGame(hero, villain);
                    default:
                        break;
                }

                return null;
            }
        }

        public MinimaxAgent CreateMinimaxAgent(int id)
        {
            switch (Game.ToLower())
            {
                case "tic-tac-toe":
                case "tictactoe":
                case "tic tac toe": return new MinimaxAgent(id, 
                    TicTacToeGame.CheckGameOver, 
                    TicTacToeGame.GetValidNextMoves, 
                    TicTacToeGame.EvaluateBoard,
                    TicTacToeGame.ApplyMove,
                    this);

                case "connect four":
                case "connect4":
                case "connect 4":
                case "connect-four":
                case "connect-4":
                case "connectfour": return new MinimaxAgent(id, 
                    ConnectFourGame.CheckGameOver, 
                    ConnectFourGame.GetValidNextMoves, 
                    ConnectFourGame.EvaluateBoard,
                    ConnectFourGame.ApplyMove,
                    this);

                case "reversi":
                case "othello": return new MinimaxAgent(id, 
                    ReversiGame.CheckGameOver, 
                    ReversiGame.GetValidNextMoves, 
                    ReversiGame.EvaluateBoard, 
                    ReversiGame.FlipPieces,
                    this);
                default:
                    throw new Exception("Unknown game: " + Game);
            }
        }

        public MctsAgent CreateMctsAgent(int id, bool benchmarkAgent)
        {
            switch (Game.ToLower())
            {
                case "tic-tac-toe":
                case "tictactoe":
                case "tic tac toe": return new MctsAgent(id,
                    TicTacToeGame.CheckGameOver,
                    TicTacToeGame.GetValidNextMoves,
                    TicTacToeGame.ApplyMove,
                    this, benchmarkAgent);

                case "connect four":
                case "connect4":
                case "connect 4":
                case "connect-four":
                case "connect-4":
                case "connectfour": return new MctsAgent(id,
                    ConnectFourGame.CheckGameOver,
                    ConnectFourGame.GetValidNextMoves,
                    ConnectFourGame.ApplyMove,
                    this, benchmarkAgent);

                case "reversi":
                case "othello": return new MctsAgent(id,
                    ReversiGame.CheckGameOver,
                    ReversiGame.GetValidNextMoves,
                    ReversiGame.FlipPieces,
                    this, benchmarkAgent);
                default:
                    throw new Exception("Unknown game: " + Game);
            }
        }

        public BlondieAgent CreateBlondieAgent(int id, IBlackBox boardEval)
        {
            switch (Game.ToLower())
            {
                case "tic-tac-toe":
                case "tictactoe":
                case "tic tac toe": return new BlondieAgent(id,
                    TicTacToeGame.CheckGameOver,
                    TicTacToeGame.GetValidNextMoves,
                    boardEval,
                    TicTacToeGame.ApplyMove,
                    this);

                case "connect four":
                case "connect4":
                case "connect 4":
                case "connect-four":
                case "connect-4":
                case "connectfour": return new BlondieAgent(id, 
                    ConnectFourGame.CheckGameOver, 
                    ConnectFourGame.GetValidNextMoves, 
                    boardEval, 
                    ConnectFourGame.ApplyMove,
                    this);

                case "reversi":
                case "othello": return new BlondieAgent(id, 
                    ReversiGame.CheckGameOver, 
                    ReversiGame.GetValidNextMoves, 
                    boardEval,
                    ReversiGame.FlipPieces,
                    this);
                default:
                    throw new Exception("Unknown game: " + Game);
            }
        }

        public MctsNeatAgent CreateMctsNeatAgent(int id, IBlackBox boardEval)
        {
            switch (Game.ToLower())
            {
                case "tic-tac-toe":
                case "tictactoe":
                case "tic tac toe": return new MctsNeatAgent(id,
                    TicTacToeGame.CheckGameOver,
                    TicTacToeGame.GetValidNextMoves,
                    boardEval,
                    TicTacToeGame.ApplyMove,
                    this);

                case "connect four":
                case "connect4":
                case "connect 4":
                case "connect-four":
                case "connect-4":
                case "connectfour": return new MctsNeatAgent(id,
                    ConnectFourGame.CheckGameOver,
                    ConnectFourGame.GetValidNextMoves,
                    boardEval,
                    ConnectFourGame.ApplyMove,
                    this);

                case "reversi":
                case "othello": return new MctsNeatAgent(id,
                    ReversiGame.CheckGameOver,
                    ReversiGame.GetValidNextMoves,
                    boardEval,
                    ReversiGame.FlipPieces,
                    this);
                default:
                    throw new Exception("Unknown game: " + Game);
            }
        }

        public static GridGameParameters GetParameters(string[] args)
        {
            if (args.Length == 0)
            {
                PrintHelp();
                return null;
            }

            GridGameParameters gg = DefaultParameters(args[0]);

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
                        if (gg.Evaluator != "coevolve" && gg.Evaluator != "random" && gg.Evaluator != "minimax"
                            && gg.Evaluator != "blondie" && gg.Evaluator != "mcts")
                        {
                            Console.WriteLine("Invalid evaluator: '{0}'. Options are random, coevolve, minimax, blondie.", eval);
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
                        gg.ExperimentPath = dir;
                        gg.ResultsPath = gg.ExperimentPath + gg.Name + "_results.csv";
                        gg.ConfigPath = gg.ExperimentPath + gg.Name + "_config.xml";
                        gg.ChampionPath = gg.ExperimentPath + gg.Name + "_gen{0}_champion.xml";
                        gg.BenchmarkResultsPath = gg.ExperimentPath + gg.Name + "_benchmark.csv";
                        gg.BenchmarkGameLogPath = gg.ExperimentPath + gg.Name + "_games.txt";
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

                    case "blondie":
                        gg.BlondieAgents = true;
                        break;

                    case "opponent":
                    case "opp":
                        gg.OpponentPath = args[++i];
                        break;

                    case "matches":
                    case "battles":
                    case "bouts":
                        int matches;
                        if (!int.TryParse(args[++i], out matches))
                        {
                            Console.WriteLine("Invalid matches per opponent: '{0}'.", args[i]);
                            return null;
                        }
                        gg.MatchesPerOpponent = matches;
                        break;

                    case "mctrials":
                        int trials;
                        if (!int.TryParse(args[++i], out trials))
                        {
                            Console.WriteLine("Invalid trials per move: '{0}'.", args[i]);
                            return null;
                        }
                        gg.MonteCarloTrials = trials;
                        break;

                    case "mintrials":
                        int mintrials;
                        if (!int.TryParse(args[++i], out mintrials))
                        {
                            Console.WriteLine("Invalid minimum trials per possible move: '{0}'.", args[i]);
                            return null;
                        }
                        gg.MinMcTrialsPerMove = mintrials;
                        break;

                    case "uctconst":
                        double uctConst;
                        if (!double.TryParse(args[++i], out uctConst))
                        {
                            Console.WriteLine("Invalid UCT const: '{0}'.", args[i]);
                            return null;
                        }
                        gg.UctConst = uctConst;
                        break;
                    case "eval_mctrials":
                        int eval_trials;
                        if (!int.TryParse(args[++i], out eval_trials))
                        {
                            Console.WriteLine("Invalid evaluator trials per move: '{0}'.", args[i]);
                            return null;
                        }
                        gg.EvaluatorMonteCarloTrials = eval_trials;
                        break;

                    case "eval_mintrials":
                        int eval_mintrials;
                        if (!int.TryParse(args[++i], out eval_mintrials))
                        {
                            Console.WriteLine("Invalid evaluator minimum trials per possible move: '{0}'.", args[i]);
                            return null;
                        }
                        gg.EvaluatorMinMcTrialsPerMove = eval_mintrials;
                        break;

                    case "eval_uctconst":
                        double eval_uctConst;
                        if (!double.TryParse(args[++i], out eval_uctConst))
                        {
                            Console.WriteLine("Invalid evaluator UCT const: '{0}'.", args[i]);
                            return null;
                        }
                        gg.EvaluatorUctConst = eval_uctConst;
                        break;
                    case "hyperneat":
                        gg.HyperNeat = true;
                        break;
                    case "mctsneat":
                        gg.MctsNeat = true;
                        break;
                    case "oppcount":
                        int oppcount;
                        if(!int.TryParse(args[++i], out oppcount))
                        {
                            Console.WriteLine("Invalid opponent count: '{0}'.", args[i]);
                            return null;
                        }
                        gg.RoundRobinOpponents = oppcount;
                        break;
                    case "agentpath":
                        gg.AgentPath = args[++i];
                        break;
                    default:
                        Console.WriteLine("Invalid option: '{0}'. Option unknown. Use -help to see options.", args[i].Substring(1));
                        return null;
                }
            }

            return gg;
        }

        public static GridGameParameters DefaultParameters(string name)
        {
            GridGameParameters gg = new GridGameParameters()
            {
                Name = name,
                Description = "",
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
                MaxMemorySize = 0,
                ExperimentPath = "../../../experiments/tictactoe/random/",
                BlondieAgents = false,
                MinimaxDepth = 9,
                OpponentPath = null,
                MatchesPerOpponent = 1,
                MonteCarloTrials = 1000,
                MinMcTrialsPerMove = 1,
                UctConst = 0.5,
                HyperNeat = false,
                MctsNeat = false,
                RoundRobinOpponents = 100
            };

            gg.ResultsPath = gg.ExperimentPath + gg.Name + "_results.csv";
            gg.ConfigPath = gg.ExperimentPath + gg.Name + "_config.xml";
            gg.ChampionPath = gg.ExperimentPath + gg.Name + "_gen{0}_champion.xml";
            gg.BenchmarkResultsPath = gg.ExperimentPath + gg.Name + "_benchmark.csv";
            gg.BenchmarkGameLogPath = gg.ExperimentPath + gg.Name + "_games.txt";
            return gg;
        }

        static void PrintHelp()
        {
            Console.WriteLine("Usage: AgentBenchmark.exe <name> [-options...]");
            Console.WriteLine("<name>".PadRight(25) + "File prefix to use when saving the config and results files.");
            Console.WriteLine("-help".PadRight(25) + "Prints the usage summary.");
            Console.WriteLine("-game -g".PadRight(25) + "Name of the game. Valid options: tictactoe, connect4, reversi. Default: tictactoe");
            Console.WriteLine("-inputs -i".PadRight(25) + "Number of inputs for the neural network (usually # of board spaces). Default: 9");
            Console.WriteLine("-outputs -o".PadRight(25) + "Number of outputs for the neural network (usually # of board spaces). Default: 9");
            Console.WriteLine("-evaluator -eval -e".PadRight(25) + "Evaluation function to use. Valid options: random, coevolve, minimax, blondie, mcts. Default: random");
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
            Console.WriteLine("-depth".PadRight(25) + "Maximum search depth for Minimax and Blondie agents. Default: 9");
            Console.WriteLine("-blondie".PadRight(25) + "Use Blondie24-style agents composed of a neural network board evaluator with minimax. Default: false");
            Console.WriteLine("-opponent -opp".PadRight(25) + "Location of the opponent to use when evaluating.");
            Console.WriteLine("-matches".PadRight(25) + "Number of matches to play against each opponent when evaluating agents. Default: 1");
            Console.WriteLine("-mctrials".PadRight(25) + "Number of monte carlo trials per move for a MCTS agent. Default: 1000");
            Console.WriteLine("-mintrials".PadRight(25) + "Minimum number of monte carlo trials per possible move for a MCTS agent. Default: 1");
            Console.WriteLine("-uctconst".PadRight(25) + "Constant multiplier for the UCT update equation. Default: 0.5");
            Console.WriteLine("-hyperneat".PadRight(25) + "Use HyperNEAT for the agents. Default: false");
            Console.WriteLine("-mctsneat".PadRight(25) + "Use MCTS with a NEAT-based default policy. Default: false");
            Console.WriteLine("-oppcount".PadRight(25) + "Number of opponents to play against in round robin mode, selected randomly without replacement from the population. Default: 100");
            Console.WriteLine("-eval_mctrials".PadRight(25) + "Number of monte carlo trials per move for a MCTS benchmark agent. Default: 1000");
            Console.WriteLine("-eval_mintrials".PadRight(25) + "Minimum number of monte carlo trials per possible move for a MCTS benchmark agent. Default: 1");
            Console.WriteLine("-eval_uctconst".PadRight(25) + "Constant multiplier for the UCT update equation for a benchmark agent. Default: 0.5");
            Console.WriteLine("-agentpath".PadRight(25) + "The file path of the desired agent to benchmark.");
            
        }
    }
}
