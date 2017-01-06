using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReversiMVC.Models
{
    public class Board
    {
        private readonly Game game;

        public Field[,] Fields { get; set; }

        public Board( Game game )
        {
            this.game = game;
            this.Fields = new Field[ game.BoardLength, game.BoardLength ];
            this.ResetBoard();
        }

        public Board( Board board )
        {
            this.game = board.game;

            this.Fields = new Field[ this.game.BoardLength, this.game.BoardLength ];

            for ( int row = 0; row < this.game.BoardLength; row++ )
            {
                for ( int column = 0; column < this.game.BoardLength; column++ )
                {
                    this.Fields[ row, column ] = board.Fields[ row, column ];
                }
            }
        }

        public int CalculateScore( PieceColor pieceColor )
        {
            int score = 0;

            Field fieldColor = pieceColor == PieceColor.White ? Field.White : Field.Black;

            for ( int row = 0; row < 8; row++ )
            {
                for ( int column = 0; column < 8; column++ )
                {
                    if ( this.Fields[ row, column ] == fieldColor )
                    {
                        score++;
                    }
                    else if ( this.Fields[ row, column ] != Field.Empty )
                    {
                        score--;
                    }
                }
            }

            return score;
        }

        public List<PieceMove> GetAllValidMoves( PieceColor pieceColor )
        {
            var moves = new List<PieceMove>();

            for ( int row = 0; row < 8; row++ )
            {
                for ( int column = 0; column < 8; column++ )
                {
                    var move = new PieceMove( pieceColor, new PiecePosition( row, column ) );

                    if ( this.ValidateMove( move, false ) )
                    {
                        moves.Add( move );
                    }
                }
            }

            return moves;
        }

        public int GetPiecesCount( PieceColor pieceColor )
        {
            int score = 0;

            Field fieldColor = pieceColor == PieceColor.White ? Field.White : Field.Black;

            for ( int row = 0; row < 8; row++ )
            {
                for ( int column = 0; column < 8; column++ )
                {
                    if ( this.Fields[ row, column ] == fieldColor )
                    {
                        score++;
                    }
                }
            }

            return score;
        }

        public bool IsEndOfGame()
        {
            return ( !this.GetAllValidMoves( PieceColor.Black ).Any() && !this.GetAllValidMoves( PieceColor.White ).Any() )
                   || this.Fields.Cast<Field>().All( f => f != Field.Empty );
        }

        public void PlayPiece( PieceMove pieceMove )
        {
            if ( pieceMove == null )
            {
                return;
            }

            if ( !this.ValidateMove( pieceMove, true ) )
            {
                throw new Exception( "Invalid move" );
            }

            this.Fields[ pieceMove.Position.Row, pieceMove.Position.Column ] = pieceMove.PieceColor == PieceColor.White
                                                                               ? Field.White
                                                                               : Field.Black;
        }

        public void ResetBoard()
        {
            for ( int row = 0; row < this.game.BoardLength; row++ )
            {
                for ( int column = 0; column < this.game.BoardLength; column++ )
                {
                    this.Fields[ row, column ] = Field.Empty;
                }
            }
        }

        public bool ValidateMove( PieceMove pieceMove, bool flip )
        {
            bool isValid = false;

            int row = pieceMove.Position.Row;
            int column = pieceMove.Position.Column;
            PieceColor pieceColor = pieceMove.PieceColor;

            if ( this.Fields[ row, column ] != Field.Empty )
            {
                return false;
            }

            var pos = new PiecePosition( 0, 0 );
            Field startFieldColor = pieceColor == PieceColor.White ? Field.White : Field.Black;

            for ( int x = -1; x <= 1; x++ )
            {
                for ( int y = -1; y <= 1; y++ )
                {
                    pos.Row = row + x;
                    pos.Column = column + y;

                    if ( pos.Row < 0 || pos.Row > 7 )
                    {
                        continue;
                    }

                    if ( pos.Column < 0 || pos.Column > 7 )
                    {
                        continue;
                    }

                    Field currentField = this.Fields[ pos.Row, pos.Column ];

                    if ( currentField == Field.Empty || currentField == startFieldColor )
                    {
                        continue;
                    }

                    while ( true )
                    {
                        pos.Row += x;
                        pos.Column += y;

                        if ( pos.Row < 0 || pos.Row > 7 )
                        {
                            break;
                        }

                        if ( pos.Column < 0 || pos.Column > 7 )
                        {
                            break;
                        }

                        currentField = this.Fields[ pos.Row, pos.Column ];

                        if ( currentField == Field.Empty )
                        {
                            break;
                        }

                        if ( currentField == startFieldColor )
                        {
                            if ( flip )
                            {
                                pos.Row -= x;
                                pos.Column -= y;
                                currentField = this.Fields[ pos.Row, pos.Column ];

                                while ( currentField != Field.Empty )
                                {
                                    this.Fields[ pos.Row, pos.Column ] = startFieldColor;
                                    pos.Row -= x;
                                    pos.Column -= y;
                                    currentField = this.Fields[ pos.Row, pos.Column ];
                                }
                            }

                            isValid = true;
                            break;
                        }
                    }
                }
            }

            return isValid;
        }
    }
}