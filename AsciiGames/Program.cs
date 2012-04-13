using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using grid_games;
using grid_games.TicTacToe;
using grid_games.ConnectFour;
using grid_games.Reversi;

namespace AsciiGames
{
    class Program
    {
        static void Main(string[] args)
        {
            do
            {
                GridGameParameters ggp = GridGameParameters.DefaultParameters("ascii");

                Console.WriteLine("t = Tic-Tac-Toe");
                Console.WriteLine("c = Connect4");
                Console.WriteLine("r = Reversi");
                string gameName = getOption("Game? ", "t", "c", "r");

                switch (gameName)
                {
                    case "t": ggp.Game = "tictactoe"; break;
                    case "c": ggp.Game = "connect4"; break;
                    case "r": ggp.Game = "reversi"; break;
                    default:
                        break;
                }

                bool player1 = getOption("Do you want to be player 1 or 2? ", "1", "2") == "1";

                Console.WriteLine("h = Human");
                Console.WriteLine("m = Minimax");
                Console.WriteLine("r = Random");
                string agentType = getOption("Opponent type? ", "h", "m", "r");

                IAgent opp;
                switch (agentType)
                {
                    case "h": opp = new AsciiAgent(1); break;
                    case "m":
                        int depth;
                        string depthStr = getOption("Max tree search depth? ");
                        while (!int.TryParse(depthStr, out depth))
                            depthStr = getOption("Invalid depth value. Try again: ");
                        ggp.MinimaxDepth = depth;
                        opp = ggp.CreateMinimaxAgent(1); break;
                    case "r": opp = new RandomAgent(1); break;
                    default:
                        throw new Exception("Unknown agent type");
                }

                GridGame game = CreateGame(gameName, player1, opp);

                game.PlayToEnd();

                game.Board.PrintBoard();

                AnnounceWinner(game, player1);

            } while (getOption("Play again (y/n)? ", "y", "n") == "y");
        }

        private static void AnnounceWinner(GridGame game, bool player1)
        {
            if (game.Winner == 0)
                Console.WriteLine("You tied.");
            else if(game.Winner == 1 || !player1)
                Console.WriteLine("You won!");
            else
                Console.WriteLine("You lost. :(");
        }

        private static GridGame CreateGame(string gameName, bool player1, IAgent opponent)
        {
            IAgent human = new AsciiAgent(0);

            IAgent hero = player1 ? human : opponent;
            IAgent villain = player1 ? opponent : human;

            GridGame game;
            switch (gameName)
            {
                case "t": game = new TicTacToeGame(hero, villain);
                    break;
                case "c": game = new ConnectFourGame(hero, villain);
                    break;
                case "r": game = new ReversiGame(hero, villain);
                    break;
                default: throw new Exception("invalid option");
            }
            return game;
        }

        private static string getOption(string msg, params string[] validOptions)
        {
            Console.Write(msg);
            string selection = Console.ReadLine();

            if (validOptions == null || validOptions.Length == 0)
                return selection;

            while (!validOptions.Contains(selection))
            {
                Console.WriteLine("Invalid option.");
                Console.Write(msg);
                selection = Console.ReadLine();
            }
            return selection;
        }
    }
}
