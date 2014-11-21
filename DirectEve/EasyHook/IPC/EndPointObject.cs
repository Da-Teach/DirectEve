// ------------------------------------------------------------------------------
//   <copyright from='2010' to='2015' company='THEHACKERWITHIN.COM'>
//     Copyright (c) TheHackerWithin.COM. All Rights Reserved.
// 
//     Please look in the accompanying license.htm file for the license that 
//     applies to this source code. (a copy can also be found at: 
//     http://www.thehackerwithin.com/license.htm)
//   </copyright>
// -------------------------------------------------------------------------------

namespace EasyHook.IPC
{
    using System;
    using System.Runtime.Remoting;

    /// <summary>
    ///     The base class of any type representing an endpoint in <see cref="EasyHook" />'s IPC framework.
    /// </summary>
    public abstract class EndPointObject : MarshalByRefObject
    {
        #region Public Members

        // ToDo: If project is updated to .NET3.5 or above, implement extension method TryPing()
        /// <summary>
        ///     If no exception is thrown, the IPC connection is expected to be working.
        /// </summary>
        /// <exception cref="RemotingException">
        ///     A <see cref="RemotingException" /> is thrown by the CLR if the current object can't be reached.
        /// </exception>
        public void Ping()
        {
        }

        #endregion

        #region Internal Members

        /// <summary>
        ///     Asserts that the given <paramref name="type" /> is a <see cref="Type" /> deriving from
        ///     <see cref="EndPointObject" />.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="paramName"></param>
        internal static void AssertDerivedType(Type type, string paramName)
        {
            if (type == null)
                throw new ArgumentNullException(paramName);
            if (!typeof (EndPointObject).IsAssignableFrom(type))
                throw new ArgumentException("The given type must be a type deriving from EndPointObject", paramName);
        }

        #endregion
    }
}