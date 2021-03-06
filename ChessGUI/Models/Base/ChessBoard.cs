﻿using ChessGUI.Models.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chess.Models.Base
{
    /// <summary>
    /// This class represents a chessboard, consisting of an 8X8 Grid of ChessSquares.
    /// </summary>
    public class ChessBoard
    {
        // Horizontal Boundaries { a - h }
        public const char MIN_FILE = 'a';
        public const char MAX_FILE = 'h';

        // Vertical Boundaries { 1 - 8 }
        public const int MIN_RANK = 1;
        public const int MAX_RANK = 8;

        public const int ROW_COUNT = 8;
        public const int COL_COUNT = 8;

        // Contains all 64 chess tiles and allows us access squares by their name
        private Dictionary<string, ChessSquare> _squares;
        public List<ChessSquare> Squares { get { return _squares.Values.ToList(); } }

        public List<ChessPiece> LightPieces { get; private set; }
        public List<ChessPiece> DarkPieces { get; private set; }

        /// <summary>
        /// Constructs a new ChessBoard.
        /// </summary>
        public ChessBoard()
        {
            Init();
        }

        /// <summary>
        /// Initializes this Chessboard by ensuring that all the tiles are created.
        /// </summary>
        public void Init()
        {
            CreateSquares();
            BoardScanner.Board = this;
            AddPieces();
        }

        /// <summary>
        /// Creates the 8x8 ChessBoard Square grid with alternating white and black colors.
        /// </summary>
        private void CreateSquares()
        {
            _squares = new Dictionary<string, ChessSquare>();
            ChessColor squareColor = ChessColor.WHITE;

            // loop top to bottom { 8 - 1 }
            for (int r = MAX_RANK; r >= MIN_RANK; r--)
            {
                // loop left to right { a - h }
                for (char f = MIN_FILE; f <= MAX_FILE; f++)
                {
                    ChessSquare square = new ChessSquare(f, r, squareColor);
                    AlternateTileColor(ref squareColor);
                    _squares.Add(square.Name, square);
                }
                AlternateTileColor(ref squareColor);
            }
        }

        /// <summary>
        /// Adds all the LIGHT and DARK pieces to this ChessBoard.
        /// </summary>
        private void AddPieces()
        {
            // Light Pieces Placement boundaries
            const int LIGHT_START_RANK = 2;
            const int LIGHT_END_RANK = MIN_RANK;

            // Dark Pieces Placement boundaries
            const int DARK_START_RANK = MAX_RANK;
            const int DARK_END_RANK = 7;

            DarkPieces  = AddPieces(DARK_START_RANK, DARK_END_RANK, ChessColor.DARK);
            LightPieces = AddPieces(LIGHT_START_RANK, LIGHT_END_RANK, ChessColor.LIGHT);
        }

        /// <summary>
        /// Gets the proper initial starting piece positions for the specified
        /// ChessColor.
        /// </summary>
        /// <param name="color">ChessColor pieces</param>
        /// <returns>starting layout based on the specified ChessColor</returns>
        private List<char> GetInitialPieceLayout(ChessColor color)
        {
            List<char> pieceLayout = null;

            if (color == ChessColor.DARK)
            {
                pieceLayout = new List<char>()
                {
                    'R', 'N', 'B', 'Q', 'K', 'B', 'N', 'R',
                    'P', 'P', 'P', 'P', 'P', 'P', 'P', 'P',
                };
            } else if (color == ChessColor.LIGHT)
            {
                pieceLayout = new List<char>()
                {
                    'P', 'P', 'P', 'P', 'P', 'P', 'P', 'P',
                    'R', 'N', 'B', 'Q', 'K', 'B', 'N', 'R',
                };
            }
            return pieceLayout;
        }

        /// <summary>
        /// Adds pieces of specific ChessColor to this GameBoard.
        /// </summary>
        /// <param name="startRank">Starting Rank</param>
        /// <param name="endRank">Ending Rank</param>
        /// <param name="color">The color of the piece</param>
        private List<ChessPiece> AddPieces(int startRank, int endRank, ChessColor color)
        {
            List<char> pieceLayout = GetInitialPieceLayout(color);
            List<ChessPiece> pieceList = new List<ChessPiece>();
            int pieceIndex = 0;

            for (int r = startRank; r >= endRank; r--)
            {
                for (char f = MIN_FILE; f <= MAX_FILE; f++)
                {
                    ChessSquare square = SquareAt(f, r);
                    char symbol = pieceLayout.ElementAt(pieceIndex);

                    ChessPiece piece = ChessUtils.PieceFromSymbol(square, color, symbol);
                    pieceList.Add(piece);
                    pieceIndex++;
                }
            }
            return pieceList;
        }

        /// <summary>
        /// Alternates between WHITE and BLACK. This is useful when generating the
        /// board tiles.
        /// </summary>
        /// <param name="lastColor">ChessColor to check and modify to its inverse color</param>
        private void AlternateTileColor(ref ChessColor lastColor)
        {
            lastColor = (lastColor == ChessColor.WHITE) ? ChessColor.BLACK : ChessColor.WHITE;
        }

        /// <summary>
        /// Returns the ChessSquare based off the ChessSquare location name
        /// </summary>
        /// <param name="name">ChessSquare name</param>
        /// <returns>ChessSquare at the location specified by the name</returns>
        public ChessSquare SquareAt(string name)
        {
            ChessSquare square = null;
            _squares.TryGetValue(name.ToLower(), out square);
            return square;
        }

        /// <summary>
        /// Returns ChessSquare based off the file and rank position.
        /// </summary>
        /// <param name="file">File position</param>
        /// <param name="rank">Rank position</param>
        /// <returns>ChessSquare at the specific File,Rank position</returns>
        public ChessSquare SquareAt(char file, int rank)
        {
            return SquareAt(file + "" + rank);
        }

        #region old

        /////////////////////////////////////////////////////
        /////////////////////////////////////////////////////
        // TODO:: These were for the console. Maybe make into its own class. But they're not used anymore!



        /// <summary>
        /// Prints this chessboard out to the console, for debugging purposes.
        /// </summary>
        public void PrintDebug()
        {
            Console.WriteLine("\t  Black");

            // print rank and board pieces
            for (int r = MAX_RANK; r >= MIN_RANK; r--)
            {
                Console.Write(r + " ");

                for (char f = MIN_FILE; f <= MAX_FILE; f++)
                {
                    ChessSquare square = SquareAt(f, r);
                    char symbol = square.IsOccupied() ? square.Piece.Symbol : ' ';
                    ChessConsoleColors(square);

                    Console.Write(" " + symbol + " ");
                }
                NormalConsoleColors();
                Console.WriteLine();
            }
            PrintFileLabels();
        }

        /// <summary>
        /// Colorizes the Console Buffer with the proper colors based on the ChessSquare.
        /// </summary>
        /// <param name="square">ChessSquare to get color values from</param>
        private void ChessConsoleColors(ChessSquare square)
        {
            // Background Square Color
            Console.BackgroundColor =
                (square.Color == ChessColor.LIGHT) ? ConsoleColor.Gray : ConsoleColor.DarkGray;

            // Foreground Text Color
            if (square.Piece != null)
            {
                Console.ForegroundColor =
                    (square.Piece.Color == ChessColor.LIGHT) ? ConsoleColor.White : ConsoleColor.Black;
            }
        }

        /// <summary>
        /// Colorizes the Console Buffer with normal colors.
        /// </summary>
        private void NormalConsoleColors()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Green;
        }

        /// <summary>
        /// Prints labels for each File { A - H }
        /// </summary>
        private void PrintFileLabels()
        {
            Console.Write("   ");
            for (char f = MIN_FILE; f <= MAX_FILE; f++)
            {
                Console.Write(f + "  ");
            }
            Console.WriteLine();
            Console.WriteLine("\t  White");
        }

        #endregion
    }
}
