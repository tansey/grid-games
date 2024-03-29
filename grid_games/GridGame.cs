﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace grid_games
{
    public abstract class GridGame
    {
        public const int HERO_ID = 1;
        public const int VILLAIN_ID = -1;

        private int _rows, _columns, _startingPlayer;

        public int[,] Board { get; private set; }
        public int ActingPlayer { get; protected set; }
        public bool[,] ValidNextMoves { get; private set; }
        public bool GameOver { get; protected set; }
        public int Winner { get; protected set; }
        public IAgent Hero { get; set; }
        public IAgent Villain { get; set; }
        public List<Turn> Turns { get; set; }


        public delegate void AgentMovedHandler(GridGame game, int movingPlayer, int currentPlayer, Move m);
        public event AgentMovedHandler AgentMoved;
        public delegate void AgentPassedHandler(GridGame game, int passingPlayer, int currentPlayer);
        public event AgentPassedHandler AgentPassed;


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
            int moveNum = 0;
            while (!GameOver)
            {
                moveNum++;
                IAgent player = ActingPlayer == HERO_ID ? Hero : Villain;

                if (!HasValidMove())
                {
                    ActingPlayer *= -1;
                    if(AgentPassed != null)
                        AgentPassed(this, ActingPlayer * -1, ActingPlayer);
                    continue;
                }

                // Create new stopwatch
                Stopwatch stopwatch = new Stopwatch();

                // Begin timing
                stopwatch.Start();

                // Get the player's move
                Move m = player.GetMove(Board, ValidNextMoves);

                // Stop timing
                stopwatch.Stop();

                // Write result
                Turns.Add(new Turn(m, ActingPlayer, stopwatch.ElapsedMilliseconds));

                Move(m.Row, m.Column);

                ActingPlayer *= -1;

                if (AgentMoved != null)
                    AgentMoved(this, ActingPlayer * -1, ActingPlayer, m);
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
            Turns = new List<Turn>();
            ActingPlayer = _startingPlayer;

            Hero.Reset();
            Villain.Reset();

            Hero.PlayerId = 1;
            Villain.PlayerId = -1;
        }

        public bool HasEmptyCell()
        {
            for (int i = 0; i < _rows; i++)
                for (int j = 0; j < _columns; j++)
                    if (Board[i, j] == 0)
                        return true;
            return false;
        }

        public bool HasValidMove()
        {
            for (int i = 0; i < _rows; i++)
                for (int j = 0; j < _columns; j++)
                    if (ValidNextMoves[i, j])
                        return true;
            return false;
        }
    }
}
