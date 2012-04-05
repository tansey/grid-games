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
                Console.WriteLine("t = Tic-Tac-Toe");
                Console.WriteLine("c = Connect4");
                Console.WriteLine("r = Reversi");
                string gameName = getOption("Game? ", "t", "c", "r");

                bool player1 = getOption("Do you want to be player 1 or 2? ", "1", "2") == "1";

                GridGame game = CreateGame(gameName, player1);

                game.PlayToEnd();

                AsciiAgent.DrawBoard(game.Board);

                AnnounceWinner(game);

            } while (getOption("Play again (y/n)? ", "y", "n") == "y");
        }

        private static void AnnounceWinner(GridGame game)
        {
            if (game.Winner == 1)
                Console.WriteLine("You won!");
            else if (game.Winner == -1)
                Console.WriteLine("You lost. :(");
            else
                Console.WriteLine("You tied.");
        }

        private static GridGame CreateGame(string gameName, bool player1)
        {
            IAgent human = new AsciiAgent(0);
            IAgent cpu = new RandomAgent(1);

            IAgent hero = player1 ? human : cpu;
            IAgent villain = player1 ? cpu : human;

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
