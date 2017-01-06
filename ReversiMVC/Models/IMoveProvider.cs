using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReversiMVC.Models
{
    public interface IMoveProvider
    {
        PieceMove GetNextMove( Board board, PieceColor pieceColor );
    }
}