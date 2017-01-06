using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReversiMVC.Models
{
    public class MinMaxMoveProvider : IMoveProvider
    {
        private readonly int maxDepth;

        private readonly Random random;

        private readonly int[,] riskRegions =
            {
                { 5, 4, 3, 3, 3, 3, 4, 5 }, { 4, 4, 2, 2, 2, 2, 4, 4 },
                { 3, 2, 1, 1, 1, 1, 2, 3 }, { 3, 2, 1, 1, 1, 1, 2, 3 },
                { 3, 2, 1, 1, 1, 1, 2, 3 }, { 3, 2, 1, 1, 1, 1, 2, 3 },
                { 4, 4, 2, 2, 2, 2, 4, 4 }, { 5, 4, 3, 3, 3, 3, 4, 5 }
            };

        public MinMaxMoveProvider( int maxDepth )
        {
            this.random = new Random();
            this.maxDepth = maxDepth;
        }

        public PieceMove GetNextMove( Board board, PieceColor pieceColor )
        {
            var moves = new List<PieceMove>();

            List<PieceMove> validMoves = board.GetAllValidMoves( pieceColor );

            int alpha = int.MinValue;
            const int Beta = int.MaxValue;
            foreach ( PieceMove validMove in validMoves )
            {
                var tmpBoard = new Board( board );
                tmpBoard.PlayPawn( validMove );

                int v = this.MinMax( tmpBoard, this.GetNextColor( pieceColor ), alpha, Beta, 1 );
                if ( v > alpha )
                {
                    alpha = v;
                    moves.Clear();
                    moves.Add( validMove );
                }
                else if ( v == alpha )
                {
                    moves.Add( validMove );
                }
            }

            if ( moves.Count == 0 )
            {
                return null;
            }

            // int index = this.random.Next(0, moves.Count);
            int index = this.RiskRegionsBestIndex( moves );
            return moves[ index ];
        }

        private PieceColor GetNextColor( PieceColor pieceColor )
        {
            return pieceColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
        }

        private int MinMax( Board board, PieceColor pieceColor, int alpha, int beta, int depth )
        {
            if ( depth >= this.maxDepth || board.IsEndOfGame() )
            {
                return 0;
            }

            List<PieceMove> validMoves = board.GetAllValidMoves( pieceColor );

            bool isMax = depth % 2 == 1;
            if ( isMax )
            {
                foreach ( PieceMove validMove in validMoves )
                {
                    var tmpBoard = new Board( board );
                    tmpBoard.PlayPawn( validMove );

                    alpha = Math.Max( alpha, this.MinMax( tmpBoard, this.GetNextColor( pieceColor ), alpha, beta, depth + 1 ) );

                    if ( beta <= alpha )
                    {
                        return beta;
                    }
                }

                return alpha;
            }

            // isMin
            foreach ( PieceMove validMove in validMoves )
            {
                var tmpBoard = new Board( board );
                tmpBoard.PlayPawn( validMove );

                beta = Math.Min( beta, this.MinMax( tmpBoard, this.GetNextColor( pieceColor ), alpha, beta, depth + 1 ) );

                if ( beta <= alpha )
                {
                    return alpha;
                }
            }

            return beta;
        }

        private int RiskRegionIndex( int region, IEnumerable<PieceMove> moves )
        {
            List<PieceMove> regions =
                moves.Where( x => this.riskRegions[ x.Position.Row, x.Position.Column ] == region ).ToList();
            if ( regions.Count != 0 )
            {
                return this.random.Next( 0, regions.Count );
            }

            return -1;
        }

        private int RiskRegionsBestIndex( List<PieceMove> moves )
        {
            int indexRegion5 = this.RiskRegionIndex( 5, moves );
            if ( indexRegion5 != -1 )
            {
                return indexRegion5;
            }

            int indexRegion3 = this.RiskRegionIndex( 3, moves );
            if ( indexRegion3 != -1 )
            {
                return indexRegion3;
            }

            int indexRegion1 = this.RiskRegionIndex( 1, moves );
            if ( indexRegion1 != -1 )
            {
                return indexRegion1;
            }

            int indexRegion2 = this.RiskRegionIndex( 2, moves );
            if ( indexRegion2 != -1 )
            {
                return indexRegion2;
            }

            int indexRegion4 = this.RiskRegionIndex( 4, moves );
            return indexRegion4;
        }
    }
}