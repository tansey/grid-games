using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace grid_games.Reversi
{
    public class ReversiGame : GridGame
    {
        public ReversiGame(IAgent hero, IAgent villain)
            : base(8, 8, hero, villain)
        {
            this.AgentMoved += new AgentMovedHandler(flipPiecesAndCheckGameOver);
        }

        void flipPiecesAndCheckGameOver(GridGame game, int player, Move m)
        {
            flipPieces(game, player, m);
            checkGameOver(game, player, m);
        }

        void flipPieces(GridGame game, int player, Move m)
        {
            // TODO
        }

        void checkGameOver(GridGame game, int player, Move m)
        {
            // TODO
        }
    }
}
