﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GitMenu
{
    /// <summary>
    /// Basic elegant implementation of a thread-safe singleton class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton<T>
        where T : Singleton<T>
    {

        #region Properties

        /// <summary>
        /// Gets a boolean indicating if the instance has been created yet
        /// </summary>
        public static bool HasInstance { get; private set; }

        /// <summary>
        /// Gets the single instance of the object
        /// </summary>
        public static T Instance
        {
            get { return InstanceClass.Instance; }
        }

        #endregion

        #region Instance Class

        /// <summary>
        /// Helper class used to create the instance in a thread-safe way without any explicit locking
        /// </summary>
        protected class InstanceClass
        {

            static InstanceClass()
            {
                //since there's no protected constraint for generics we have to do this
                //it's a lot slower... but since it's a one time deal it shouldn't be a problem
                Instance = Activator.CreateInstance(typeof(T), true) as T;
                HasInstance = true;
            }

            internal static readonly T Instance;

        }

        #endregion

    }
}
