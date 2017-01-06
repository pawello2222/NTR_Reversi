using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReversiMVC.Models
{
    public class PieceMove
    {
        public PieceMove( PieceColor pieceColor, PiecePosition position )
        {
            this.PieceColor = pieceColor;
            this.Position = position;
        }

        public PieceColor PieceColor { get; set; }

        public PiecePosition Position { get; set; }

        public override string ToString()
        {
            char c = this.PieceColor == PieceColor.White ? 'w' : 'b';
            return "Move: " + c + "[" + this.Position.Row + ", " + this.Position.Column + "]";
        }
    }
}