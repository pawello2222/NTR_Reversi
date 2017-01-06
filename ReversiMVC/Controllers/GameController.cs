using ReversiMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ReversiMVC.Controllers
{
    public interface IStateManager<T>
    {
        void save( string name, T state );
        T load( string name );
    }

    public class SessionStateManager<T> : IStateManager<T>
    {
        public void save( string name, T state )
        {
            HttpContext.Current.Session[ name ] = state;
        }
        public T load( string name )
        {
            return ( T ) HttpContext.Current.Session[ name ];
        }
    }

    public class GameController : Controller
    {
        protected IStateManager<Game> stateManager = new SessionStateManager<Game>();
        public void setStateManager( IStateManager<Game> manager )
        {
            stateManager = manager;
        }

        public ActionResult Index( string field )
        {
            Game game = stateManager.load( "game" );
            if ( game == null )
                game = new Game( 1 );

            if ( field != null && field != string.Empty )
            {
                int row = field[ 0 ] - '0';
                int column = field[ 1 ] - '0';
                PiecePosition piecePosition = new PiecePosition( row, column );

                if ( game.PlayPieceHuman( piecePosition ) )
                {
                    game.PlayPieceAI();
                }
            }

            stateManager.save( "game", game );

            return View( game.Board.Fields );
        }
    }
}