﻿using ChessGUI.Controllers;
using System;

namespace ChessGUI.Models.SpecialMoves
{
    /// <summary>
    /// Base class for handling special moves. Classes that inherit this
    /// class will be able to communicate with the Game for updating
    /// when special moves have occurred.
    /// </summary>
    public abstract class SpecialMove
    {
        private static bool _isInitialized;
        public static ChessGame Game { get; protected set; }

        public SpecialMove()
        {
            if (!_isInitialized)
            {
                throw new Exception("Call Init() on SpecialMove before constructing them!");
            }
        }

        /// <summary>
        /// Initializes all special moves so that they can communicate with
        /// the ChessGame.
        /// </summary>
        /// <param name="game"></param>
        public static void Init(ChessGame game)
        {
            if (game != null)
            {
                Game = game;
                _isInitialized = true;
            }
        }

        /// <summary>
        /// Method to be implemented to check for a special move occurrence.
        /// </summary>
        public abstract void Check();
    }
}
