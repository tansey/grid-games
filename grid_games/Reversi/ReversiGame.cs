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
		
		void setValidNextMoves()
        {
            for (int i = 0; i < 8; i++){
                for (int j = 0; j < 8; j++){
					if (Board[i][j] == 0) 
						//Look in all eight dirs for a move
					{
						for (int k = -1; k < 2; k++){
							for (int l = -1; l < 2; l++){
								// find opponents pieces, if that works
								bool opponent_found = false;
								int iidx = i;
								int jidx = j;
								do{
									iidx += k;
									jidx += l;
									if (iidx >= 0 && iidx < 8 && jidx >= 0 && jidx < 8)
									if (Board[iidx, jidx] == ActingPlayer * -1)
										opponent_found = true;
								} while (Board[iidx, jidx] == ActingPlayer * -1)
								
									
									// find a blank space
										//Really?  That happened?  Sweet, you found a valid move!
							}
						}
					
					}
				}
			}
                    
        }
    }
}
