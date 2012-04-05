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

            if (Board[0, 0] != 0)
            {
                var type = Board[0, 0];
                //top left to bottom left
                if (type == Board[0, 1] && type == Board[0, 2])
                {
                    GameOver = true;
                    Winner = type;
                    return;
                }

                //top left to top right
                if (type == Board[1, 0] && type == Board[2, 0])
                {
                    GameOver = true;
                    Winner = type;
                    return;
                }

                //top left to bottom right
                if (type == Board[1, 1] && type == Board[2, 2])
                {
                    GameOver = true;
                    Winner = type;
                    return;
                }
            }

            if (Board[0, 2] != 0)
            {
                var type = Board[0, 2];

                //bottom left to top right
                if (type == Board[1, 1] && type == Board[2, 0])
                {
                    GameOver = true;
                    Winner = type;
                    return;
                }

                //bottom left to bottom right
                if (type == Board[1, 2] && type == Board[2, 2])
                {
                    GameOver = true;
                    Winner = type;
                    return;
                }
            }

            if (Board[0, 1] != 0)
            {
                var type = Board[0, 1];

                //middle left to middle right
                if (type == Board[1, 1] && type == Board[2, 1])
                {
                    GameOver = true;
                    Winner = type;
                    return;
                }
            }

            if (Board[1, 0] != 0)
            {
                var type = Board[1, 0];

                //middle top to middle bottom
                if (type == Board[1, 1] && type == Board[1, 2])
                {
                    GameOver = true;
                    Winner = type;
                    return;
                }
            }

            if (Board[2, 0] != 0)
            {
                var type = Board[2, 0];

                //top right to bottom right
                if (type == Board[2, 1] && type == Board[2, 2])
                {
                    GameOver = true;
                    Winner = type;
                    return;
                }
            }

            // if the game is not over, set the valid next moves
            setValidNextMoves();
			
			//if there are no valid new moves and no one won, it's a draw!
			bool empty = false;
            for (int i = 0; i < 6; i++)
                for (int j = 0; j < 7; j++)
                    if (Board[i,j] == 0)
						empty = true;
			if (empty) 
			{
				GameOver = true;
				Winner = 0;
				return;
			}
        }

        void setValidNextMoves()
        {
            // Any empty square is a valid move
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    ValidNextMoves[i, j] = Board[i, j] == 0;
        }
    }
}
