using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReversiMVC.Models
{
    public class PiecePosition
    {
        public PiecePosition( int row, int column )
        {
            this.Row = row;
            this.Column = column;
        }
       
        public int Column { get; set; }

        public int Row { get; set; }
    }
}