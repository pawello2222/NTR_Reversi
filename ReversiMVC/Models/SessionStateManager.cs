using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReversiMVC.Models
{
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
}