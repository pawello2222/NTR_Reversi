using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReversiMVC.Models
{
    public class Game
    {
        public readonly int AILevel;
        
        public readonly int BoardLength = 8;
        
        public Board Board;
        public PieceColor Turn { get; set; }

        public GameState CurrentGameState = GameState.MainMenu;
        
        private PieceColor humanPieceColor;
        
        private IMoveProvider moveProvider;

        public Game( int level )
        {
            this.Board = new Board( this );

            this.NewGame( level );

            this.AILevel = level;

            this.Turn = PieceColor.Black;
        }

        public PieceColor GetHumanPieceColor()
        {
            if ( this.CurrentGameState != GameState.PlayingHumanComputer )
            {
                throw new Exception( "GetHumanPieceColor can be only called when game is AI vs. Human" );
            }

            return this.humanPieceColor;
        }

        public Winner GetWinner()
        {
            if ( this.CurrentGameState != GameState.GameEnded )
            {
                throw new Exception( "Game is not ended yet." );
            }

            int blackScore = this.Board.CalculateScore( PieceColor.Black );
            int whiteScore = this.Board.CalculateScore( PieceColor.White );

            if ( blackScore == whiteScore )
            {
                return Winner.Draw;
            }

            return whiteScore > blackScore ? Winner.White : Winner.Black;
        }

        public void NewGame( int level )
        {
            this.moveProvider = new MinMaxMoveProvider( level );

            this.Board.ResetBoard();

            this.Board.Fields[ 3, 3 ] = Field.White;
            this.Board.Fields[ 4, 4 ] = Field.White;
            this.Board.Fields[ 4, 3 ] = Field.Black;
            this.Board.Fields[ 3, 4 ] = Field.Black;

            this.Turn = PieceColor.Black;
        }

        public void NextTurn()
        {
            this.Turn = this.Turn == PieceColor.Black ? PieceColor.White : PieceColor.Black;

            if ( this.Board.GetAllValidMoves( this.Turn ).Count == 0 )
            {
                this.Turn = this.Turn == PieceColor.Black ? PieceColor.White : PieceColor.Black;

                if ( this.Board.GetAllValidMoves( this.Turn ).Count == 0 )
                {
                    this.CurrentGameState = GameState.GameEnded;
                }
            }
        }

        public void PlayHumanComputer( PieceColor humanPieceColor )
        {
            this.CurrentGameState = GameState.PlayingHumanComputer;

            this.humanPieceColor = humanPieceColor;
        }

        public void PlayPieceAI()
        {
            this.Board.PlayPiece( this.moveProvider.GetNextMove( this.Board, this.Turn ) );
            this.NextTurn();

            if ( this.Board.IsEndOfGame() )
            {
                CurrentGameState = GameState.GameEnded;
            }
        }

        public bool PlayPieceHuman( PiecePosition piecePosition )
        {
            var pieceMove = new PieceMove( this.CurrentGameState == GameState.PlayingHumanComputer ? this.humanPieceColor : this.Turn, piecePosition );
            if ( this.Board.ValidateMove( pieceMove, false ) )
            {
                this.Board.PlayPiece( pieceMove );
                this.NextTurn();

                if ( this.Board.IsEndOfGame() )
                {
                    CurrentGameState = GameState.GameEnded;
                }

                return true;
            }

            return false;
        }
    }
}