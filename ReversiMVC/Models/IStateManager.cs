using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReversiMVC.Models
{
    public interface IStateManager<T>
    {
        void save( string name, T state );
        T load( string name );
    }
}