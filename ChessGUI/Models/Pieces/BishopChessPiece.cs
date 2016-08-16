﻿using System;
using Chess.Models.Base;
using ChessGUI.Models.Utils;

namespace Chess.Models.Pieces
{
    public class BishopChessPiece : ChessPiece
    {
        public BishopChessPiece(ChessSquare location, ChessColor color) : base(location, color)
        {
        }

        public override Move[] MoveDirections
        {
            get
            {
                return Moves.DIAGONAL;
            }
        }

        //public override MoveDirection[] MoveDirections
        //{
        //    get
        //    {
        //        return Moves.DIAGONAL;
        //    }
        //}

        public override char Symbol
        {
            get
            {
                return 'B';
            }
        }

        public override int Value
        {
            get
            {
                return 350;
            }
        }

        public override string ToString()
        {
            return Color + "_Bishop";
        }
    }
}
