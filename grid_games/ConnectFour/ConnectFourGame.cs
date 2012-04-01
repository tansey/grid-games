using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace grid_games.ConnectFour
{
    public class ConnectFourGame : GridGame
    {
        public ConnectFourGame(IAgent hero, IAgent villain)
            : base(6, 7, hero, villain)
        {
            this.AgentMoved += new AgentMovedHandler(checkGameOver);
        }

        void checkGameOver(GridGame game, int player, Move m)
        {
            // TODO
        }
    }
}
