using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReversiMVC.Models
{
    public enum GameState
    {
        MainMenu,
        Options,
        HumanComputerSelectSide,
        PlayingHumanHuman,
        PlayingHumanComputer,
        PlayingComputerComputer,
        GameEnded
    }
}