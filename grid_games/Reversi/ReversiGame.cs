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
			//Console.WriteLine("No Valid Moves for player {0}", ActingPlayer);
            setValidNextMoves();
        }

        void flipPiecesAndCheckGameOver(GridGame game, int player, Move m)
        {
            flipPieces(game, player, m);
            checkGameOver(game, player, m);
			setValidNextMoves();
        }

        void flipPieces(GridGame game, int player, Move m)
        {
			//Console.WriteLine("Flip {0}, {1}", m.Column, m.Row);
            int i = m.Row;
			int j = m.Column;
			//Board[m.Column, m.Row] = ActingPlayer;
			for (int k = -1; k < 2; k++)
				for (int l = -1; l < 2; l++)
					if (validDirection(i,j,k,l, ActingPlayer))
				{
					//Console.WriteLine("{0}, {1}", m.Column, m.Row);
						flipTokens(i,j,k,l);
				}
        }
		
		
		//prerequisite: this is a valid direction
		void flipTokens(int i, int j, int k, int l){
			//Console.WriteLine("{0}, {1}", i, j);
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
				if (valid_space)
					opponent_found = Board[iidx, jidx] == ActingPlayer * -1;
				else
					opponent_found = false;
				// did we find an opponent's piece? flip it!
				if (opponent_found)
					Board[iidx, jidx] = ActingPlayer;
					
				// we keep going as long as they have pieces
			} while (valid_space && opponent_found);										
			
		}
		
		bool validDirection(int i, int j, int k, int l, int player){
			if (j == 0 && k == 0)
				return false;
													
			// first we look for opponents pieces
			bool opponent_found = false;
			bool opponent_ever_found = false;
			bool valid_space = true;
			int iidx = i;
			int jidx = j;
			
			do{
				// move in the right direction	
				iidx += k;
				jidx += l;
				
				// make sure we haven't gone off the board
				valid_space = (iidx >= 0 && iidx < 8 && jidx >= 0 && jidx < 8);
				if (valid_space)
					opponent_found = Board[iidx, jidx] == (player * -1);	
				else
					opponent_found = false;
				if (opponent_found)
					opponent_ever_found = true;
				// we keep going as long as they have pieces
			} while (valid_space && opponent_found);
								
			// ok, if there are no more of their guys, we look for one of ours.
			// if we find one, then we can put our piece on the original square
			if (opponent_ever_found){
				if (valid_space && Board[iidx, jidx] == player){
					//Console.WriteLine("{0},{1}",iidx, jidx
					return true;
				}
			}
			
			return false;
		}
		
		

        void checkGameOver(GridGame game, int player, Move m)
        {
			
			//Console.Out.WriteLine("Check {0},{1}", m.Row, m.Column);
				if (noValidMoves(1) && noValidMoves(-1)){
				calculateWinner();
				GameOver = true;
				return;
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
			bool [,] validNext = getValidNextMoves(ActingPlayer);
            //Console.Out.WriteLine();
            for (int i = 0; i < 8; i++){
				//Console.Out.WriteLine();
                for (int j = 0; j < 8; j++){
					ValidNextMoves[i, j] = validNext[i,j];
				}
			}
		}
		
		bool[,] getValidNextMoves(int player)
		{
			bool [,] validNext = new bool[9,9];
			//Console.Out.WriteLine();
            for (int i = 0; i < 8; i++){
				//Console.Out.WriteLine();
                for (int j = 0; j < 8; j++){
					//Console.Out.Write("{0}\t", Board[i,j]);
					validNext[i, j] = false;
					if (Board[i,j] == 0) {
						//Look in all eight dirs for a move
						// there is only a move if looking in one direction we
						// see opponent's pieces followed by one of our pieces.
						for (int k = -1; k < 2; k++){
							for (int l = -1; l < 2; l++){
								if (validDirection(i,j,k,l, player * -1))
								{
									//Console.WriteLine("{0}, {1}", i, j);
									validNext[i, j] = true;
								}
							}
						}
					}
				}
			}
			
			return validNext;
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
			
			
		bool noValidMoves(int player)		
		{   
			bool[,] validNext = getValidNextMoves(player);
			for (int i = 0; i < 8; i++)
				for (int j = 0; j < 8; j++)
					if (validNext[i, j] == true)
						return false;
			return true;
		}
	}
}	
