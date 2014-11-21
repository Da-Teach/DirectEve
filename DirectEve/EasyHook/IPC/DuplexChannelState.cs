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

    /// <summary>
    ///     Defines the state of a <see cref="DuplexChannel{T}" />.
    /// </summary>
    [Flags]
    internal enum DuplexChannelState
    {
        /// <summary>
        ///     The channel is down.
        /// </summary>
        Down = 0x00,

        /// <summary>
        ///     The remote endpoint is able to send messages to the local endpoint.
        /// </summary>
        ServerUp = 0x01,

        /// <summary>
        ///     The local endpoint is able to send messages to the remote endpoint.
        /// </summary>
        ClientUp = 0x02,

        /// <summary>
        ///     Both endpoints can communicate in either direction.
        /// </summary>
        FullDuplex = 0x03
    }
}