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
            this.AgentPassed += new AgentPassedHandler(agentPassed);
        }

        void agentPassed(GridGame game, int player)
        {
            setValidNextMoves();
        }

        void flipPiecesAndCheckGameOver(GridGame game, int player, Move m)
        {
            flipPieces(game, player, m);
            checkGameOver(game, player, m);
        }

        void flipPieces(GridGame game, int player, Move m)
        {
            int i = m.Row;
			int j = m.Column;
			for (int k = -1; k < 2; k++)
				for (int l = -1; l < 2; l++)
					if (validDirection(i,j,k,l))
						flipTokens(i,j,k,l);
        }
		
		
		//prerequisite: this is a valid direction
		void flipTokens(int i, int j, int k, int l){
			int iidx = i;
			int jidx = j;
			bool opponent_found = false;
			bool valid_space = true;
			
			do{
				// move in the right direction	
				iidx += k;
				jidx += l;
				
				// make sure we haven't gone off the board
				valid_space = (iidx >= 0 && iidx < 8 && jidx >= 0 && jidx < 8);
				
				// did we find an opponent's piece? flip it!
				if (valid_space && Board[iidx, jidx] == ActingPlayer * -1){
					opponent_found = true;
					Board[iidx, jidx] = ActingPlayer;
				}
					
				// we keep going as long as they have pieces
			} while (valid_space && opponent_found);										
			
		}
		
		bool validDirection(int i, int j, int k, int l){
													
			// first we look for opponents pieces
			bool opponent_found = false;
			bool valid_space = true;
			int iidx = i;
			int jidx = j;
			
			do{
				// move in the right direction	
				iidx += k;
				jidx += l;
				
				// make sure we haven't gone off the board
				valid_space = (iidx >= 0 && iidx < 8 && jidx >= 0 && jidx < 8);
				
				// did we find an opponent's piece? hooray!
				if (valid_space && Board[iidx, jidx] == ActingPlayer * -1)
					opponent_found = true;
					
				// we keep going as long as they have pieces
			} while (valid_space && opponent_found);
								
			// ok, if there are no more of their guys, we look for one of ours.
			// if we find one, then we can put our piece on the original square
			if (opponent_found)
				if (valid_space && Board[iidx, jidx] == ActingPlayer)
					return true;
			
			return false;
		}
		
		

        void checkGameOver(GridGame game, int player, Move m)
        {
            setValidNextMoves();
			if (! noValidMoves())
				return;
			else {
				ActingPlayer = ActingPlayer *  -1;
				if (! noValidMoves()){
					ActingPlayer = ActingPlayer *  -1;
					return;
				}
				else {
					calculateWinner();
					GameOver = true;
					return;
				}
			}
        }
		
		void calculateWinner(){
			int sum = 0;
			// by adding everything, if the sum is negative,
			// player -1 wins.  If it's positive, player 1 wins.
			
			for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
					sum += Board[i,j];
			
			if (sum < 0)
				Winner = -1;
			if (sum > 0)
				Winner = 1;
			else
				Winner = 0;
		}
		
		void setValidNextMoves()
        {
            
            for (int i = 0; i < 8; i++){
                for (int j = 0; j < 8; j++){
					if (Board[i,j] == 0) {
						ValidNextMoves[i, j] = false;
						//Look in all eight dirs for a move
						// there is only a move if looking in one direction we
						// see opponent's pieces followed by one of our pieces.
						for (int k = -1; k < 2; k++){
							for (int l = -1; l < 2; l++){
								if (validDirection(i,j,k,l))
									ValidNextMoves[i, j] = true;
							}
						}
					}
				}
			}
		}

        public override void Reset()
        {
            base.Reset();

            Board[3, 3] = 1;
            Board[3, 4] = -1;
            Board[4, 3] = -1;
            Board[4, 4] = 1;
            setValidNextMoves();
        }
			
			
		bool noValidMoves()		
		{    
			for (int i = 0; i < 8; i++)
				for (int j = 0; j < 8; j++)
					if (ValidNextMoves[i, j] == true)
						return false;
			return true;
		}
	}
}	
