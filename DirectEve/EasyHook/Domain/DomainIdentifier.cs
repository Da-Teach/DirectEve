// ------------------------------------------------------------------------------
//   <copyright from='2010' to='2015' company='THEHACKERWITHIN.COM'>
//     Copyright (c) TheHackerWithin.COM. All Rights Reserved.
// 
//     Please look in the accompanying license.htm file for the license that 
//     applies to this source code. (a copy can also be found at: 
//     http://www.thehackerwithin.com/license.htm)
//   </copyright>
// -------------------------------------------------------------------------------

namespace EasyHook.Domain
{
    using System;
    using System.Diagnostics;

    /// <summary>
    ///     Provides a system wide unique identifier for an application domain.
    /// </summary>
    [Serializable]
    public struct DomainIdentifier : IEquatable<DomainIdentifier>
    {
        #region Properties

        /// <summary>
        ///     Gets or sets the ID of the application domain.
        /// </summary>
        public int DomainId { get; set; }

        /// <summary>
        ///     Gets or sets the ID of the process.
        /// </summary>
        public int ProcessId { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Returns the identifier as a string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ProcessId + "." + DomainId;
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        ///     Gets a new <see cref="DomainIdentifier" /> which identifies the current application domain.
        /// </summary>
        /// <returns></returns>
        public static DomainIdentifier GetLocalDomainIdentifier()
        {
            return new DomainIdentifier
            {
                DomainId = AppDomain.CurrentDomain.Id,
                ProcessId = Process.GetCurrentProcess().Id
            };
        }

        #endregion

        #region IEquatable<DomainIdentifier> Members

        /// <summary>
        ///     Indicates whether the current <see cref="DomainIdentifier" /> is equal to the <paramref name="other" />
        ///     <see cref="DomainIdentifier" />.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(DomainIdentifier other)
        {
            return ProcessId == other.ProcessId && DomainId == other.DomainId;
        }

        #endregion
    }
}