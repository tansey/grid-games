using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace grid_games.TicTacToe
{
    /// <summary>
    /// The hero is X, the villain is O. Hero moves first.
    /// </summary>
    public class TicTacToeGame : GridGame
    {
        public TicTacToeGame(IAgent hero, IAgent villain)
            : base(3, 3, hero, villain)
        {
            this.AgentMoved += new AgentMovedHandler(checkGameOver);
        }

        void checkGameOver(GridGame game, int player, Move m)
        {
            var Board = game.Board;

            int winner;
            GameOver = CheckGameOver(Board, out winner);

            if (GameOver)
                Winner = winner;

            // if the game is not over, set the valid next moves
            setValidNextMoves();
        }

        public static bool CheckGameOver(int[,] Board, out int winner)
        {
            if (Board[0, 0] != 0)
            {
                var type = Board[0, 0];
                //top left to bottom left
                if (type == Board[0, 1] && type == Board[0, 2])
                {
                    winner = type;
                    return true;
                }

                //top left to top right
                if (type == Board[1, 0] && type == Board[2, 0])
                {
                    winner = type;
                    return true;
                }

                //top left to bottom right
                if (type == Board[1, 1] && type == Board[2, 2])
                {
                    winner = type;
                    return true;
                }
            }

            if (Board[0, 2] != 0)
            {
                var type = Board[0, 2];

                //bottom left to top right
                if (type == Board[1, 1] && type == Board[2, 0])
                {
                    winner = type;
                    return true;
                }

                //bottom left to bottom right
                if (type == Board[1, 2] && type == Board[2, 2])
                {
                    winner = type;
                    return true;
                }
            }

            if (Board[0, 1] != 0)
            {
                var type = Board[0, 1];

                //middle left to middle right
                if (type == Board[1, 1] && type == Board[2, 1])
                {
                    winner = type;
                    return true;
                }
            }

            if (Board[1, 0] != 0)
            {
                var type = Board[1, 0];

                //middle top to middle bottom
                if (type == Board[1, 1] && type == Board[1, 2])
                {
                    winner = type;
                    return true;
                }
            }

            if (Board[2, 0] != 0)
            {
                var type = Board[2, 0];

                //top right to bottom right
                if (type == Board[2, 1] && type == Board[2, 2])
                {
                    winner = type;
                    return true;
                }
            }

            //if there are no valid new moves and no one won, it's a draw!
            if (!Board.HasEmptyCell())
            {
                winner = 0;
                return true;
            }

            winner = 0;
            return false;
        }

        public static void GetValidNextMoves(int[,] board, bool[,] validNextMoves, int player)
        {
            // Any empty square is a valid move
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    validNextMoves[i, j] = board[i, j] == 0;
        }

        public static double EvaluateBoard(int[,] board, int player)
        {
            // It's tic-tac-toe. Go to the end of the game tree and stop being lazy.
            return 0; 
        }

        void setValidNextMoves()
        {
            GetValidNextMoves(Board, ValidNextMoves, ActingPlayer);
        }

        public override void Reset()
        {
            base.Reset();

            // Any empty square is a valid move
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    ValidNextMoves[i, j] = true;
        }


    }
}
