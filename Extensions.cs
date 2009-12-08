using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE;
using GitMenu.Commands;

namespace GitMenu
{
    public static class Extensions
    {
        public static T GetService<T>(this IServiceProvider provider)
        {
            return (T)provider.GetService(typeof(T));
        }

        public static T GetService<TService, T>(this IServiceProvider provider)
        {
            return (T)provider.GetService(typeof(TService));
        }

        public static Property GetPropertyByName(this Properties props, string propName)
        {
            return (from prop in props.OfType<Property>()
                        where prop.Name == propName
                        select prop).FirstOrDefault();
        }

        public static bool IsSet(this CommandFlags flags, CommandFlags flag)
        {
            return ((flags & flag) == flag);
        }

        public static string GetFullPath(this ProjectItem item)
        {
            return item.Properties.GetPropertyByName("FullPath").Value.ToString();
        }
    }
}
