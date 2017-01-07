using ReversiMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

            if ( game != null && game.CurrentGameState == GameState.GameEnded )
                game = null;

            stateManager.save( "game", game );

            return View( game != null );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Setup( int? levelDifficulty, string color )
        {
            Game game;
            int level;

            if ( levelDifficulty != null && color != null )
            {
                level = levelDifficulty ?? default( int );
                game = new Game( level );

                if ( color == PieceColor.Black.ToString() )
                    game.PlayHumanComputer( PieceColor.Black );
                else
                {
                    game.PlayHumanComputer( PieceColor.White );
                }

                stateManager.save( "game", game );
            }
            else
            {
                game = stateManager.load( "game" );
                if ( game == null )
                    return RedirectToAction( "Index", false );
            }

            return RedirectToAction( "Board" );
        }

        public ActionResult Board()
        {
            Game game = stateManager.load( "game" );
            if ( game == null )
                return RedirectToAction( "Index", false );
            
            if ( game.CurrentGameState != GameState.GameEnded
                && game.Turn != game.GetHumanPieceColor() )
            {
                game.PlayPieceAI();
                stateManager.save( "game", game );
            }

            if ( game.CurrentGameState == GameState.GameEnded )
            {
                string winnerText;
                Winner winner = game.GetWinner();
                switch ( winner )
                {
                    case Winner.Black:
                        winnerText = "Czarne wygrywają!";
                        break;
                    case Winner.White:
                        winnerText = "Białe wygrywają!";
                        break;
                    default:
                        winnerText = "Remis!";
                        break;
                }

                ViewBag.Winner = winnerText;
            }

            return View( game );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Board( string field )
        {
            Game game = stateManager.load( "game" );
            if ( game == null )
                return RedirectToAction( "Index", false );

            if ( field != null && field != string.Empty )
            {
                int row = field[ 0 ] - '0';
                int column = field[ 1 ] - '0';
                PiecePosition piecePosition = new PiecePosition( row, column );

                if ( game.PlayPieceHuman( piecePosition ) )
                {
                    if ( game.CurrentGameState != GameState.GameEnded
                        && game.Turn != game.GetHumanPieceColor() )
                    {
                        game.PlayPieceAI();
                    }
                    stateManager.save( "game", game );
                }
            }

            if ( game.CurrentGameState == GameState.GameEnded )
            {
                string winnerText;
                Winner winner = game.GetWinner();
                switch ( winner )
                {
                    case Winner.Black:
                        winnerText = "Czarne wygrywają!";
                        break;
                    case Winner.White:
                        winnerText = "Białe wygrywają!";
                        break;
                    default:
                        winnerText = "Remis!";
                        break;
                }

                ViewBag.Winner = winnerText;
            }

            return View( game );
        }
    }
}