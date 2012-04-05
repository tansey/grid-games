using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpNeat.Decoders;
using grid_games.TicTacToe;
using grid_games.ConnectFour;
using grid_games.Reversi;

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
        public int Generations { get; set; }
        public string Evaluator { get; set; }

        /// <summary>
        /// Returns a function that creates a new grid game.
        /// </summary>
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
    }
}
