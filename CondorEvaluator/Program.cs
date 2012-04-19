using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using grid_games;
using System.Xml;
using System.IO;
using SharpNeat.Core;

namespace CondorEvaluator
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 5)
            {
                Console.WriteLine("Usage: CondorEvaluator.exe <model_id> <model_file> <results_file> <finished_flag> <config_file>");
                return;
            }

            int curArg = 0;
            int modelId = int.Parse(args[++curArg]);
            string modelFile = args[++curArg];
            string resultsFile = args[++curArg];
            string finishedFlag = args[++curArg];
            string configFile = args[++curArg];

            GridGameExperiment experiment = new GridGameExperiment(configFile);

            IAgent model = experiment.CreateTargetAgent();
            IAgent benchmark = experiment.CreateEvalAgent();

            using (TextWriter tw = new StreamWriter(resultsFile))
            {
                // Evaluate
                if (model == null)
                {   // Non-viable genome.
                    tw.WriteLine("0.0 0.0");
                }
                else
                {
                    double score = Evaluate(model, benchmark, experiment.Parameters);
                    tw.WriteLine(score + " " + score);
                }
            }

            File.Create(finishedFlag);
        }

        static double Evaluate(IAgent model, IAgent benchmark, GridGameParameters _params)
        {
            double score = 0;
            Console.WriteLine("Starting games as player 1..");
            for (int i = 0; i < _params.MatchesPerOpponent; i++)
            {
                var game = _params.GameFunction(model, benchmark);
                game.PlayToEnd();
                if (game.Winner == 1)
                    score += _params.WinReward;
                else if (game.Winner == 0)
                    score += _params.TieReward;
                else
                    score += _params.LossReward;
            }
            Console.WriteLine("Starting games as player 2..");
            for (int i = 0; i < _params.MatchesPerOpponent; i++)
            {
                var game = _params.GameFunction(benchmark, model);
                game.PlayToEnd();
                if (game.Winner == -1)
                    score += _params.WinReward;
                else if (game.Winner == 0)
                    score += _params.TieReward;
                else
                    score += _params.LossReward;
            }
            Console.WriteLine("Done!");
            return score;
        }

        
    }
}
