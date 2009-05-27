using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GitMenu
{
    public static class Extensions
    {
        public static T GetService<T>(this IServiceProvider provider)
        {
            return (T)provider.GetService(typeof(T));
        }

        public static T GetService<S, T>(this IServiceProvider provider)
        {
            return (T)provider.GetService(typeof(S));
        }
    }
}
