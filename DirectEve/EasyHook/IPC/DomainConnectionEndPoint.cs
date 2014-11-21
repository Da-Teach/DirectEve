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
    using Domain;

    /// <summary>
    ///     Represents the endpoint in an inter domain connection.
    /// </summary>
    internal sealed class DomainConnectionEndPoint : DuplexChannelEndPointObject
    {
        #region Variables

        /// <summary>
        ///     The identifier of the current application domain.
        /// </summary>
        private static readonly DomainIdentifier _id;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the identifier for the endpoint.
        /// </summary>
        public DomainIdentifier Id
        {
            get { return _id; }
        }

        #endregion

        #region Constructors

        static DomainConnectionEndPoint()
        {
            _id = DomainIdentifier.GetLocalDomainIdentifier();
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Creates or opens a connection to an instance of <typeparamref name="TEndPoint" />.
        /// </summary>
        /// <typeparam name="TEndPoint">The type of the endpoint to create or open.</typeparam>
        /// <returns>The url to the endpoint of the connection.</returns>
        public string CreateChannel<TEndPoint>()
            where TEndPoint : EndPointObject
        {
            return DummyCore.ConnectionManager.CreateChannel<TEndPoint>();
        }

        #endregion

        #region Public Overrides

        public override object InitializeLifetimeService()
        {
            // Returning null ensures the endpoint stays available during the complete domain's lifetime.
            return null;
        }

        #endregion
    }
}