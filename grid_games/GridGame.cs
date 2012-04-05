using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace grid_games
{
    public abstract class GridGame
    {
        const int HERO_ID = 1;
        const int VILLAIN_ID = -1;

        private int _rows, _columns, _startingPlayer;

        public int[,] Board { get; private set; }
        public int ActingPlayer { get; protected set; }
        public bool[,] ValidNextMoves { get; private set; }
        public bool GameOver { get; protected set; }
        public int Winner { get; protected set; }
        public IAgent Hero { get; set; }
        public IAgent Villain { get; set; }


        public delegate void AgentMovedHandler(GridGame game, int player, Move m);
        public event AgentMovedHandler AgentMoved;

        public GridGame(int rows, int columns, IAgent hero, IAgent villain, int startingPlayer = 1)
        {
            Debug.Assert(startingPlayer == 1 || startingPlayer == -1, "Player IDs are 1 (Hero) and -1 (Villain)");

            _rows = rows;
            _columns = columns;
            _startingPlayer = startingPlayer;

            Hero = hero;
            Villain = villain;

            Reset();
        }

        public virtual void PlayToEnd()
        {
            while (!GameOver)
            {
                IAgent player = ActingPlayer == HERO_ID ? Hero : Villain;

                Move m = player.GetMove(Board, ValidNextMoves);

                Move(m.Row, m.Column);

                if (AgentMoved != null)
                    AgentMoved(this, ActingPlayer, m);

                ActingPlayer *= -1;
            }
        }
        
        private void Move(int row, int column)
        {
            Debug.Assert(ValidNextMoves[row, column]);

            Board[row, column] = ActingPlayer;
        }

        public virtual void Reset()
        {
            Board = new int[_rows, _columns];
            ValidNextMoves = new bool[_rows, _columns];
            ActingPlayer = _startingPlayer;

            Hero.PlayerId = 1;
            Villain.PlayerId = -1;
        }
    }
}
