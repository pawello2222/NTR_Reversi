using ReversiMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ReversiMVC.Controllers
{
    public class GameController : Controller
    {
        protected IStateManager<Game> stateManager = new SessionStateManager<Game>();
        public void setStateManager( IStateManager<Game> manager )
        {
            stateManager = manager;
        }

        public ActionResult Index()
        {
            Game game = stateManager.load( "game" );

            return View( game != null );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Setup( int? param )
        {
            Game game;
            int level;

            if ( param != null )
            {
                level = param ?? default( int );
                game = new Game( level );
                stateManager.save( "game", game );
            }
            else
            {
                game = stateManager.load( "game" );
                if ( game == null )
                    return View( "Index", false );
            }

            return RedirectToAction( "Board" );
        }

        public ActionResult Board()
        {
            Game game = stateManager.load( "game" );
            if ( game == null )
                return View( "Index", false );

            return View( game );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Board( string field )
        {
            Game game = stateManager.load( "game" );
            if ( game == null )
                return View( "Index", false );

            if ( field != null && field != string.Empty )
            {
                int row = field[ 0 ] - '0';
                int column = field[ 1 ] - '0';
                PiecePosition piecePosition = new PiecePosition( row, column );

                if ( game.PlayPieceHuman( piecePosition ) )
                {
                    game.PlayPieceAI();
                }

                if ( game.Board.IsEndOfGame() )
                {
                    game.CurrentGameState = GameState.GameEnded;
                }
            }

            stateManager.save( "game", game );

            return View( "Board", game );
        }
    }
}