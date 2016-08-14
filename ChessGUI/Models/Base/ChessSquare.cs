﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Chess.Models.Base
{
    /// <summary>
    /// This class represents a ChessSquare on a chess board. A ChessSquare has a location
    /// with a [File, Rank] format, a TileColor, and at most one ChessPiece.
    /// </summary>
    public class ChessSquare : INotifyPropertyChanged
    {
        // Horizontal row { A - H }
        public char File { get; set; }

        // Vertical row { 1 - 8 }
        public int Rank { get; set; }

        // String representation of this tiles File,Rank location
        public string Name { get { return File + "" + Rank; } }

        public ChessColor Color { get; set; }

        // The chess piece that is currently on this ChessSquare, if any
        private ChessPiece _piece;
        public ChessPiece Piece
        {
            get
            {
                return _piece;
            }
            set
            {
                _piece = value;
                FieldChanged();
            }
        }

        //public ChessPiece Piece { get; set; }

        /// <summary>
        /// Constructs a new ChessSquare.
        /// </summary>
        /// <param name="file">Horizontal Row (File)</param>
        /// <param name="rank">Vertical Row (Rank)</param>
        /// <param name="color">Color (White / Black)</param>
        public ChessSquare(char file, int rank, ChessColor color)
        {
            File = file;
            Rank = rank;
            Color = color;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Checks to see if this ChessSquare is occupied with a chess piece.
        /// </summary>
        /// <returns>true if occupied</returns>
        public bool IsOccupied()
        {
            // If OccupiedPiece isn't null, we know that this ChessSquare is not available
            return Piece != null;
        }

        public void ClearPiece()
        {
            Piece = null;
        }

        public override string ToString()
        {
            return Name; /*+ " occupied with: [ " + Piece + " ]"; */
        }

        /// <summary>
        /// Updates the property on the calling type.
        /// </summary>
        /// <param name="field"></param>
        public void FieldChanged([CallerMemberName] string field = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(field));
        }
    }
}