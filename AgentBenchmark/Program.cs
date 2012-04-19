using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using grid_games;
using System.Xml.Serialization;
using System.IO;
using System.Diagnostics;
using System.Xml;

namespace AgentBenchmark
{
    class Program
    {
        static GridGameParameters _params;
        static GridGameExperiment _experiment;

        static void Main(string[] args)
        {
            _params = GridGameParameters.GetParameters(args);
            if (_params == null)
                return;

            Debug.Assert(_params.AgentPath != null);
            Debug.Assert(File.Exists(_params.AgentPath));

            using (TextWriter writer = new StreamWriter(_params.ConfigPath))
            {
                XmlSerializer ser = new XmlSerializer(typeof(GridGameParameters));
                ser.Serialize(writer, _params);
            }

            _experiment = new GridGameExperiment(_params.ConfigPath);

            IAgent agent = null;
            IAgent benchmark = null;

            // Load the agent to benchmark
            var modelGenome = _experiment.LoadPopulation(XmlReader.Create(_params.AgentPath))[0];
            var brain = _experiment.CreateGenomeDecoder().Decode(modelGenome);
            if (_params.MctsNeat)
                agent = _params.CreateMctsNeatAgent(1, brain);
            else
                agent = _params.CreateMctsAgent(1, false);

            // Create the benchmark MCTS agent
            if (_params.Evaluator == "mcts")
                benchmark = _params.CreateMctsAgent(-1, true);
            else
                benchmark = new RandomAgent(-1);

            Outcome[] outcomes = new Outcome[_params.MatchesPerOpponent * 2];

            Console.WriteLine("Starting games as player 1..");
            for (int i = 0; i < _params.MatchesPerOpponent; i++)
            {
                Console.Write(i + "...");
                if (i > 0 && i % 10 == 0)
                    Console.WriteLine();
                outcomes[i] = RunTrial(agent, benchmark, 1);
            }
            Console.WriteLine();

            Console.WriteLine("Starting games as player 2..");
            for (int i = 0; i < _params.MatchesPerOpponent; i++)
            {
                Console.Write(i + "...");
                if (i > 0 && i % 10 == 0)
                    Console.WriteLine();
                outcomes[i + _params.MatchesPerOpponent] = RunTrial(benchmark, agent, -1);
            }
            Console.WriteLine();
            Console.WriteLine("Saving log file...");
            using (TextWriter writer = new StreamWriter(_params.BenchmarkResultsPath))
            {
                // games
                // wins, ties, losses
                // win %, tie %, loss %
                // p1 wins, p1 ties, p1 losses, 
                // p1 win %, p1 tie %, p1 loss %, 
                // p2 wins, p2 ties, p2 losses, 
                // p2 win %, p2 tie %, p2 loss %,
                // time per move, turns per game
                writer.WriteLine(
                    outcomes.Length + "," + // games
                    
                    outcomes.Count(o => o.Winner == o.AgentId) + "," + // wins
                    Pct(outcomes.Count(o => o.Winner == o.AgentId), outcomes.Length) + "," + // win %
                    outcomes.Count(o => o.Winner == 0) + "," + // ties
                    Pct(outcomes.Count(o => o.Winner == 0), outcomes.Length) + "," + // tie %
                    outcomes.Count(o => o.Winner == o.BenchmarkId) + "," + // losses
                    Pct(outcomes.Count(o => o.Winner == o.BenchmarkId), outcomes.Length) + "," + // loss %
                    
                    outcomes.Count(o => o.Winner == o.AgentId && o.AgentId == 1) + "," + // p1 wins
                    Pct(outcomes.Count(o => o.Winner == o.AgentId && o.AgentId == 1), outcomes.Count(o => o.AgentId == 1)) + "," + // p1 win %
                    outcomes.Count(o => o.Winner == 0 && o.AgentId == 1) + "," + // p1 ties
                    Pct(outcomes.Count(o => o.Winner == 0 && o.AgentId == 1), outcomes.Count(o => o.AgentId == 1)) + "," + // p1 tie %
                    outcomes.Count(o => o.Winner == o.BenchmarkId && o.AgentId == 1) + "," + // p1 losses
                    Pct(outcomes.Count(o => o.Winner == o.BenchmarkId && o.AgentId == 1), outcomes.Count(o => o.AgentId == 1)) + "," + // p1 loss %

                    outcomes.Count(o => o.Winner == o.AgentId && o.AgentId == -1) + "," + // p2 wins
                    Pct(outcomes.Count(o => o.Winner == o.AgentId && o.AgentId == -1), outcomes.Count(o => o.AgentId == -1)) + "," + // p2 win %
                    outcomes.Count(o => o.Winner == 0 && o.AgentId == -1) + "," + // p2 ties
                    Pct(outcomes.Count(o => o.Winner == 0 && o.AgentId == -1), outcomes.Count(o => o.AgentId == -1)) + "," + // p2 tie %
                    outcomes.Count(o => o.Winner == o.BenchmarkId && o.AgentId == -1) + "," + // p2 losses
                    Pct(outcomes.Count(o => o.Winner == o.BenchmarkId && o.AgentId == -1), outcomes.Count(o => o.AgentId == -1)) + "," + // p2 loss %

                    string.Format("{0:N2},", outcomes.Average(o => o.AverageTurnTime)) +
                    string.Format("{0:N2}", outcomes.Average(o => o.TotalMoves))
                    );
            }
            Console.WriteLine("Done!");
        }

        static Outcome RunTrial(IAgent hero, IAgent villain, int agentId)
        {
            var game = _params.GameFunction(hero, villain);

            game.PlayToEnd();

            return new Outcome(game.Winner, agentId, (float)game.Turns.Where(t => t.Player == agentId).Average(t =>t.Time), game.Turns.Count);
        }

        static string Pct(double count, double total, int decimals = 2)
        {
            return string.Format("{0:N" + decimals + "}%", count / total * 100);
        }
    }
}
