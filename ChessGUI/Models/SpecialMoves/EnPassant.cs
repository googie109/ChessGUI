﻿using Chess.Models.Base;
using Chess.Models.Pieces;
using ChessGUI.Controllers;
using System.Linq;

namespace ChessGUI.Models.SpecialMoves
{
    /// <summary>
    /// This class handles the movement of PawnChessPieces where there
    /// initial move is 2 ranks. By moving 2 ranks, an enemy Pawn is
    /// allowed to capture the moved Pawn EnPassant.
    /// </summary>
    public class EnPassant : SpecialMove
    {
        private static readonly int[] ValidRanks = { 4, 5 };

        // Capture location that an enemy pawn will move to if they capture en passant
        public static ChessSquare CaptureSquare { get; private set; }

        // Pawn that recently moved 2 ranks and can be captured en passant
        public static PawnChessPiece MovedPawn { get; private set; }

        public static bool DidCapture { get; private set; }


        public override void Check()
        {
            CheckCapture();
        }

        /// <summary>
        /// Checks and updates pawns to allow for capturing en passant.
        /// </summary>
        private static void CheckCapture()
        {
            PawnChessPiece pawn = null;

            if (TryGetPawn(MoveController.MovePiece, out pawn))
            {
                if (DidMoveTwoRanks(pawn))
                {
                    Update(pawn);
                } else if ((pawn.Location == CaptureSquare))
                {
                    UpdateCapture(pawn);
                } else
                {
                    Clear();
                }
            } else
            {
                Clear();
            }
        }

        /// <summary>
        /// Checks to see if the specified ChessPiece is a Pawn.
        /// </summary>
        /// <param name="piece">Piece to check to see if it is a Pawn</param>
        /// <param name="pawn">PawnChessPiece to update with Piece</param>
        /// <returns>true if piece is a Pawn</returns>
        private static bool TryGetPawn(ChessPiece piece, out PawnChessPiece pawn)
        {
            bool isPawn = piece is PawnChessPiece;
            pawn = (isPawn) ? piece as PawnChessPiece : null;
            return isPawn;
        }

        /// <summary>
        /// Updates the MovedPawn and CaptureSquare.
        /// </summary>
        /// <param name="pawn"></param>
        private static void Update(PawnChessPiece pawn)
        {
            Clear();
            MovedPawn = pawn;
            UpdateCaptureSquare();
            DidCapture = false;
        }

        /// <summary>
        /// Updates the en passant capture and ensures that the enemy pawn 
        /// is at the correct position, and that the view is properly updated.
        /// </summary>
        /// <param name="pawn"></param>
        private static void UpdateCapture(PawnChessPiece pawn)
        {
            if (MovedPawn != null)
            {
                Game.Controller.UpdateSquareView(MovedPawn.Location, null);
                MovedPawn.Location.ClearPiece();
                MovedPawn.IsCaptured = true;
                MovedPawn = null;
                CaptureSquare.Piece = pawn;
            }
        }

        /// <summary>
        /// Clears out MovedPawn and CaptureSquares.
        /// </summary>
        private static void Clear()
        {
            // Clear out previously moved pawn if exists
            if (MovedPawn != null)
            {
                MovedPawn = null;
                if (CaptureSquare != null)
                {
                    CaptureSquare.Piece = null;
                }
            }
        }

        /// <summary>
        /// Updates the CaptureSquare, which is the ChessSquare that an enemy
        /// PawnChessPiece will land, if they capture the MovedPawn en passant.
        /// </summary>
        private static void UpdateCaptureSquare()
        {
            int rankMove = (MovedPawn.Color == ChessColor.LIGHT) ? -1 : 1;

            ChessSquare square = MovedPawn.Location;

            CaptureSquare = Game.Controller.BoardModel.SquareAt(square.File, square.Rank + rankMove);
            CaptureSquare.Piece = MovedPawn;
        }

        /// <summary>
        /// Checks to see if the PawnChessPiece moved 2 ranks so that it can be
        /// captured en passant.
        /// </summary>
        /// <param name="pawn">PawnChessPiece to check</param>
        /// <returns>true if PawnChessPiece moved 2 ranks</returns>
        private static bool DidMoveTwoRanks(PawnChessPiece pawn)
        {
            bool didMoveTwoRanks = (pawn.MoveCount == 1) && ValidRanks.Contains(pawn.Location.Rank);
            return didMoveTwoRanks;
        }
    }
}
