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
    using System.Runtime.Remoting.Channels;
    using System.Runtime.Remoting.Channels.Ipc;
    using System.Runtime.Serialization.Formatters;
    using System.Security.AccessControl;
    using System.Security.Principal;

    /// <summary>
    ///     <see cref="SimplexChannel{TEndPoint}" /> provides a simplex channel (one way communication channel)
    ///     for communication from one <see cref="AppDomain" /> to an other.
    /// </summary>
    internal class SimplexChannel<TEndPoint>
        where TEndPoint : EndPointObject
    {
        #region Variables

        private readonly ChannelProperties _channelProperties;
        private readonly EndPointConfigurationData<TEndPoint> _endPointConfig;
        private IpcServerChannel _serverChannel;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the full url of the current <see cref="SimplexChannel{TEndPoint}" />.
        /// </summary>
        public string EndPointUrl
        {
            get { return _channelProperties.Url; }
        }

        /// <summary>
        ///     Gets whether the current <see cref="SimplexChannel{TEndPoint}" /> is initialized.
        /// </summary>
        public bool IsInitialized
        {
            get { return _serverChannel != null; }
        }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of <see cref="SimplexChannel{TEndPoint}" /> using the given configuration data.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <param name="endPointConfigurationData"></param>
        public SimplexChannel(EndPointConfigurationData<TEndPoint> endPointConfigurationData)
        {
            AssertEndpointConfigurationData(endPointConfigurationData, "endPointConfigurationData");
            _channelProperties = ChannelProperties.CreateRandomChannelProperties();
            _endPointConfig = endPointConfigurationData;
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Initializes the <see cref="SimplexChannel{TEndPoint}" />.
        /// </summary>
        public void InitializeChannel()
        {
            if (IsInitialized)
                return;
            var provider = new BinaryServerFormatterSinkProvider {TypeFilterLevel = TypeFilterLevel.Full};
            var securityDescriptor = CreateSecurityDescriptor(_endPointConfig.AllowedClients);
            _serverChannel = new IpcServerChannel(_channelProperties.AsDictionary(), provider, securityDescriptor);
            ChannelServices.RegisterChannel(_serverChannel, false);
            RemotingConfiguration.RegisterWellKnownServiceType(_endPointConfig.RemoteObjectType,
                _channelProperties.EndPointName,
                _endPointConfig.ObjectMode);
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        ///     Asserts the given <see cref="EndPointConfigurationData{TEndPoint}" /> to be valid.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <param name="configData"></param>
        /// <param name="paramName"></param>
        private static void AssertEndpointConfigurationData(EndPointConfigurationData<TEndPoint> configData, string paramName)
        {
            if (configData.AllowedClients == null)
                throw new ArgumentException(
                    "The given EndPointConfigurationData specifies an illegal value for " + "AllowedClients", paramName);
        }

        /// <summary>
        ///     Returns a default <see cref="CommonSecurityDescriptor" /> based on the given collection of
        ///     <see cref="WellKnownSidType" />.
        /// </summary>
        /// <param name="allowedClients"></param>
        /// <returns></returns>
        private static CommonSecurityDescriptor CreateSecurityDescriptor(ICollection<WellKnownSidType> allowedClients)
        {
            var dacl = new DiscretionaryAcl(false, false, allowedClients.Count);
            foreach (var sid in allowedClients)
            {
                var securityId = new SecurityIdentifier(sid, null);
                dacl.AddAccess(AccessControlType.Allow, securityId, -1, InheritanceFlags.None, PropagationFlags.None);
            }
            const ControlFlags controlFlags =
                ControlFlags.GroupDefaulted | ControlFlags.OwnerDefaulted | ControlFlags.DiscretionaryAclPresent;
            var securityDescriptor = new CommonSecurityDescriptor(false, false, controlFlags, null, null, null, dacl);
            return securityDescriptor;
        }

        #endregion
    }
}