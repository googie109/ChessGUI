﻿using Chess.Models.Base;
using Chess.Models.Pieces;
using System.Collections.Generic;
using System.Windows;

namespace ChessGUI.Models.SpecialMoves
{
    /// <summary>
    /// This class handles checking if the KingChessPiece is in Check/CheckMate.
    /// This class also provides utilities for movement checking such that any
    /// pieces moved, including the KingChessPiece itself, doesn't not put the
    /// King at risk for capture.
    /// </summary>
    public class KingCheck : SpecialMove
    {
        public override void Check()
        {
            IsInCheck();
        }

        /// <summary>
        /// Checks to see if the king is InCheck.
        /// </summary>
        /// <returns>true if King is InCheck</returns>
        public static bool IsInCheck()
        {
            KingChessPiece king = Game.GetOpponent().KingPiece;
            List<ChessSquare> playerMoves = Game.GetPlayerMoves(Game.ActivePlayer);

            king.InCheck = (playerMoves.Contains(king.Location));
            Game.UpdateKingCheck(king);

            return king.InCheck;
        }
    }
}
