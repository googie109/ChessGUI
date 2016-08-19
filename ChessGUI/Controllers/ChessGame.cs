﻿using Chess.Models.Base;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows;
using ChessGUI.Models.AI;
using Chess.Models.Pieces;
using ChessGUI.Models.SpecialMoves;
using System.Windows.Media;
using System.Text;
using System;
using ChessGUI.Dialogs;
using ChessGUI.Views;

namespace ChessGUI.Controllers
{
    /// <summary>
    /// This class controls alternating between player movement for a ChessGame.
    /// </summary>
    public class ChessGame
    {
        private bool _isGameOver;

        private List<SpecialMove> _specialMoves;

        public ChessBoardController Controller { get; private set; }
        public ChessPlayer LightPlayer { get; private set; }
        public ChessPlayer DarkPlayer { get; private set; }
        // Very basic AI test player
        public BasicAI AIPlayer { get; private set; }

        // The player who is currently playing
        public ChessPlayer ActivePlayer { get; set; }

        public ChessGame()
        {
            Init();
            BeginGame();
        }

        /// <summary>
        /// Initializes everything needed to get start a game.
        /// </summary>
        private void Init()
        {
            Debug.SHOW_MESSAGES = true;
            MovementController.Game = this;

            InitSpecialMoves();

            Controller  = new ChessBoardController();
            LightPlayer = new ChessPlayer(ChessColor.LIGHT, Controller.BoardModel.LightPieces);
            DarkPlayer  = new ChessPlayer(ChessColor.DARK, Controller.BoardModel.DarkPieces);

            // TESTING
            AIPlayer = new BasicAI(DarkPlayer);
        }

        /// <summary>
        /// Creates the list of special moves that are checked after each player's move.
        /// </summary>
        private void InitSpecialMoves()
        {
            // Make sure special moves can communicate with this game
            SpecialMove.Init(this);

            _specialMoves = new List<SpecialMove>()
            {
                new EnPassant(), new PawnPromotion(),
            };
        }

        /// <summary>
        /// Kicks off gameplay of Chess.
        /// </summary>
        private void BeginGame()
        {
            // fake out first time (really is light player who goes first)
            ActivePlayer = DarkPlayer;
            NextTurn();
            Play();
        }

        /// <summary>
        /// Main game loop. Waits until the active player has moved, and then evaluates
        /// whether or not we have reached CheckMate (game over). If we haven't then 
        /// all of the special mvoes are evaluated and control goes to the next player.
        /// </summary>
        private async void Play()
        {
            while (!_isGameOver)
            {
                //Controller.PrintBoardDebug();
                if (IsCheckMate())
                {
                    MessageBox.Show("CHECKMATE!");
                    _isGameOver = true;
                    break;
                }
                do
                {
                    await Task.Delay(50);
                } while (!ActivePlayer.DidMove);

                //TODO:: after player has moved, check to see if we are in CHECKMATE!!!
                // IF WE ARE, THEN GAME IS OVER!!
                _specialMoves.ForEach(m => m.Check());
                NextTurn();
            }
        }

        private bool IsCheckMate()
        {
            List<ChessSquare> validMoves = new List<ChessSquare>();
            List<ChessPiece> playerPieces = GetPlayerPieces(ActivePlayer);

            //StringBuilder sb = new StringBuilder();

            playerPieces.ForEach(p =>
            {
                //sb.Append(p + " can move to: ");

                List<ChessSquare> movesForPiece = 
                    MovementController.GetValidMoves(p, ActivePlayer.KingPiece, GetOpponent());
                validMoves.AddRange(movesForPiece);

                //movesForPiece.ForEach(s => sb.Append(s + ", "));
                //sb.Append("\n");
            });
            //MessageBox.Show(sb.ToString(), "Valid Moves");

            bool isCheckMate = (validMoves.Count == 0);
            return isCheckMate;
        }

        /// <summary>
        /// Advances to the next Player so that they can take their turn.
        /// </summary>
        private void NextTurn()
        {
            ClearActivePlayer();
            AdvanceActivePlayer();

            //TestAIPlayerMove();
        }

        /// <summary>
        /// Clears out the active player movement information.
        /// </summary>
        private void ClearActivePlayer()
        {
            ActivePlayer.DidMove = false;
        }

        /// <summary>
        /// Advances to the next player based on who the previous active player was.
        /// </summary>
        private void AdvanceActivePlayer()
        {
            ActivePlayer = (ActivePlayer == LightPlayer) ? DarkPlayer : LightPlayer;

            // Updates ChessMovement to only allow movement from pieces belonging
            // to the ActivePlayer
            MovementController.ActivePlayer = ActivePlayer;
        }

        [Obsolete]
        /// <summary>
        /// Toggles the InCheck indicator for the specified king.
        /// </summary>
        /// <param name="king"></param>
        public void ToggleCheck(KingChessPiece king)
        {
            int squareIndex = Controller.BoardModel.Squares.IndexOf(king.Location);
            ChessSquareView squareView = Controller.BoardView.Squares[squareIndex];

            if (king.InCheck)
            {
                squareView.ToggleCheck();
                MessageBox.Show(king + " in check!");
            } else
            {
                squareView.ResetBackground();
            }
        }

        /// <summary>
        /// Shows the pawn promotion dialog to allow player to select which piece
        /// they wish to promote their pawn to.
        /// </summary>
        public void ShowPromotionDialog()
        {
            Window w = new Window();
            w.Width = 300;
            w.Height = 350;
            w.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            w.Content = new PromotionDialog(this);
            w.ShowDialog();
        }

        /// <summary>
        /// Gets a list of all available moves for the specified Player.
        /// </summary>
        /// <param name="player">Player to get moves from</param>
        /// <returns>List of all available moves for specified Player</returns>
        public List<ChessSquare> GetPlayerMoves(ChessPlayer player)
        {
            List<ChessPiece> playerPieces = GetPlayerPieces(player);
            List<ChessSquare> playerMoves = new List<ChessSquare>();
            playerPieces.ForEach(p => playerMoves.AddRange(p.GetAvailableMoves()));
            return playerMoves;
        }

        public List<ChessPiece> GetPlayerPieces(ChessPlayer player)
        {
            List<ChessPiece> playerPieces = player.Pieces.FindAll(p => (!p.Ignore && !p.IsCaptured));
            return playerPieces;
        }

        /// <summary>
        /// Gets the opponent player from the specified player.
        /// </summary>
        /// <param name="player">Player to get opponent from</param>
        /// <returns>Opponent player</returns>
        public ChessPlayer GetOpponent()
        {
            ChessPlayer opponent = (ActivePlayer == LightPlayer) ? DarkPlayer : LightPlayer;
            return opponent;
        }


        // TESTING BASIC BASIC AIPLAYER MOVEMENT. TODO REMOVE THIS AND REPLACE WITH
        // BETTER AI IN THE FUTURE!!!
        private async void TestAIPlayerMove()
        {
            // TESTING AI PLAYER MOVEMENT
            if (AIPlayer.Player == ActivePlayer)
            {
                await Task.Delay(3000);
                AIPlayer.GetMove(Controller);
                AIPlayer.Player.DidMove = true;
            }
        }
    }
}