// ------------------------------------------------------------------------------
//   <copyright from='2010' to='2015' company='THEHACKERWITHIN.COM'>
//     Copyright (c) TheHackerWithin.COM. All Rights Reserved.
// 
//     Please look in the accompanying license.htm file for the license that 
//     applies to this source code. (a copy can also be found at: 
//     http://www.thehackerwithin.com/license.htm)
//   </copyright>
// -------------------------------------------------------------------------------
namespace DirectEve
{
    using System.Collections.Generic;
    using global::DirectEve.PySharp;

    public class DirectReprocessingQuote : DirectObject
    {
        internal DirectReprocessingQuote(DirectEve directEve, long itemId, PyObject quote) : base(directEve)
        {
            ItemId = itemId;
            QuantityToProcess = (long) quote.Attribute("quantityToProcess");
            LeftOvers = (long) quote.Attribute("leftOvers");
            PlayerStanding = (float) quote.Attribute("olayerStanding");

            Recoverables = new List<DirectReprocessingQuoteRecoverable>();
            foreach (var recoverable in quote.Attribute("recoverables").Attribute("lines").ToList())
                Recoverables.Add(new DirectReprocessingQuoteRecoverable(DirectEve, recoverable));
        }

        public long ItemId { get; private set; }
        public long QuantityToProcess { get; private set; }
        public long LeftOvers { get; private set; }
        public float PlayerStanding { get; private set; }
        public List<DirectReprocessingQuoteRecoverable> Recoverables { get; private set; }
    }
}