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

        void agentPassed(GridGame game, int movingPlayer, int curPlayer)
        {
            setValidNextMoves();
        }

        void flipPiecesAndCheckGameOver(GridGame game, int movingPlayer, int curPlayer, Move m)
        {
            flipPieces(game, movingPlayer, m, Board);
            checkGameOver(game, movingPlayer, m);
        }

        static void flipPieces(GridGame game, int player, Move m, int[,] board)
        {
			//Console.WriteLine("Flip {0}, {1}", m.Column, m.Row);
            int i = m.Row;
			int j = m.Column;
			//board[m.Column, m.Row] = ActingPlayer;
			for (int k = -1; k < 2; k++)
				for (int l = -1; l < 2; l++)
					if (ValidDirection(i,j,k,l, player, board))
				{
					//Console.WriteLine("{0}, {1}", m.Column, m.Row);
						FlipTokens(i,j,k,l, player, board);
				}
        }

        void checkGameOver(GridGame game, int player, Move m)
        {
            int winner;
            GameOver = CheckGameOver(game.Board, out winner);

            if (GameOver)
                Winner = winner;

            setValidNextMoves();
        }
		
		//prerequisite: this is a valid direction
		static void FlipTokens(int i, int j, int k, int l, int player, int[,] board){
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
                    opponent_found = board[iidx, jidx] == player * -1;
				else
					opponent_found = false;
				// did we find an opponent's piece? flip it!
				if (opponent_found)
                    board[iidx, jidx] = player;
					
				// we keep going as long as they have pieces
			} while (valid_space && opponent_found);										
			
		}
		
		static bool ValidDirection(int i, int j, int k, int l, int player, int[,] board){
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
                    opponent_found = board[iidx, jidx] == (player * -1);	
				else
					opponent_found = false;
				if (opponent_found)
					opponent_ever_found = true;
				// we keep going as long as they have pieces
			} while (valid_space && opponent_found);
								
			// ok, if there are no more of their guys, we look for one of ours.
			// if we find one, then we can put our piece on the original square
			if (opponent_ever_found){
                if (valid_space && board[iidx, jidx] == player)
                {
					//Console.WriteLine("{0},{1}",iidx, jidx
					return true;
				}
			}
			
			return false;
		}
			
        
		
		static bool oneRemainingPlayer(int[,] board){
			bool player1 = false;
			bool player2 = false;
			for (int i = 0; i < 8; i++){
				for (int j = 0; j < 8; j++){
					if (board[i,j] == 1)
						player1 = true;
					else if (board[i,j] == -1)
						player2 = true;
					if (player1 && player2)
						return false;
				}
			}
			
			return true;
		}
			
		void setValidNextMoves()
        {
			GetValidNextMoves(Board, ValidNextMoves, ActingPlayer);
		}
		
  		public static void GetValidNextMoves(int[,] board, bool[,] validNextMoves, int player)
		{

            for (int i = 0; i < 8; i++){
                for (int j = 0; j < 8; j++){
					validNextMoves[i, j] = false;
					if (board[i,j] == 0) {
						//Look in all eight dirs for a move
						// there is only a move if looking in one direction we
						// see opponent's pieces followed by one of our pieces.
						for (int k = -1; k < 2; k++){
							for (int l = -1; l < 2; l++){
								if (!validNextMoves[i,j])
									if (ValidDirection(i,j,k,l, player, board))
								{
										validNextMoves[i, j] = true;
								}
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
		
				
		public static bool CheckGameOver(int[,] Board, out int winner)
        {	
			winner = 0;
			bool[,] validMove = new bool[8,8];
			GetValidNextMoves(Board, validMove, 1);
			for (int i = 0; i < 8; i++)
				for (int j = 0; j <8; j++)
					if (validMove[i,j])
						return false;
			
			GetValidNextMoves(Board, validMove, -1);
			for (int i = 0; i < 8; i++)
				for (int j = 0; j <8; j++)
					if (validMove[i,j])
						return false;
			
			int sum = 0;			
			for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
					sum += Board[i,j];
			if (sum < 0)
				winner = -1;
			if (sum > 0)
				winner = 1;
			return true;

			//return false;
        }

        public static double EvaluateBoard(int[,] board, int player)
        {
            int sum = 0;
			for (int i = 0; i < 8; i++)
				for (int j = 0; j < 8; j++)
					sum += board[i,j];
            return sum * player; 
        }
	
					
	}
}	
