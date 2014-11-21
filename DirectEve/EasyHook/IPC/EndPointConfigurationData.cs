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
    using System.Collections.Generic;
    using System.Runtime.Remoting;
    using System.Security.Principal;

    /// <summary>
    ///     Wraps all data required for instantiating the local endpoint's server for an IPC channel.
    /// </summary>
    /// <remarks>
    ///     This class is generic in order to be able to check <see cref="RemoteObjectType" /> at compile time.
    /// </remarks>
    public struct EndPointConfigurationData<TEndPoint>
        where TEndPoint : EndPointObject
    {
        #region Variables

        private ICollection<WellKnownSidType> _allowedClients;
        private WellKnownObjectMode _objectMode;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the type which provides the method implementations this server should provide.
        /// </summary>
        public Type RemoteObjectType
        {
            get { return typeof (TEndPoint); }
        }

        /// <summary>
        ///     Gets or sets the <see cref="WellKnownObjectMode" /> specifying how calls to the server must be handled.
        /// </summary>
        /// <remarks>
        ///     Use <see cref="WellKnownObjectMode.SingleCall" /> if you want to handle each call in an new object instance,
        ///     <see cref="WellKnownObjectMode.Singleton" /> otherwise.
        ///     The latter will implicitly allow you to use "static" remote variables.
        /// </remarks>
        public WellKnownObjectMode ObjectMode
        {
            get { return _objectMode; }
            set { _objectMode = value; }
        }

        /// <summary>
        ///     Gets  or sets the collection of all authenticated users allowed to access the remoting channel.
        /// </summary>
        public ICollection<WellKnownSidType> AllowedClients
        {
            get { return _allowedClients; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                if (value.Count == 0)
                    throw new ArgumentException();
                _allowedClients = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Initializes a new instance of <see cref="EndPointConfigurationData{T}" />
        ///     using the given <see cref="Type" /> and default values.
        /// </summary>
        public static EndPointConfigurationData<TEndPoint> InitializeDefault()
        {
            return new EndPointConfigurationData<TEndPoint>
            {
                _allowedClients =
                    new List<WellKnownSidType> {WellKnownSidType.BuiltinAdministratorsSid, WellKnownSidType.WorldSid},
                _objectMode = WellKnownObjectMode.Singleton
            };
        }

        #endregion
    }
}