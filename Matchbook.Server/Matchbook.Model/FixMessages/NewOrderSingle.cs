using System;

namespace Matchbook.Model
{
    /// <summary>
    /// New Order - Single
    /// FIX Reference: https://www.onixs.biz/fix-dictionary/4.4/msgType_D_68.html
    /// </summary>
    public class NewOrderSingle
    {
        /// <summary>
        /// Account mnemonic as agreed between buy and sell sides
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// Unique identifier for Order as assigned by the buy-side
        /// </summary>
        public string ClOrdId { get; set; }
        
        /// <summary>
        /// Entities both central and peripheral to the financial transaction
        /// https://www.onixs.biz/fix-dictionary/4.4/compBlock_Parties.html
        /// </summary>
        public Party[] Parties { get; set; }

        /// <summary>
        /// Instructions for order handling on Broker trading floor
        /// https://www.onixs.biz/fix-dictionary/4.4/tagNum_21.html
        /// </summary>
        public char HandlInst { get; set; }                 // TODO: https://www.onixs.biz/fix-dictionary/4.4/tagNum_21.html
        
        /// <summary>
        /// Overall/total quantity
        /// </summary>
        public decimal OrderQty { get; set; }

        /// <summary>
        /// Order type
        /// https://www.onixs.biz/fix-dictionary/4.4/tagNum_40.html
        /// </summary>
        public char OrderType { get; set; }                 // TODO: https://www.onixs.biz/fix-dictionary/4.4/tagNum_40.html

        /// <summary>
        /// Assigned value used to identify specific message originator
        /// </summary>
        public string SenderSubId { get; set; }
        public string Text { get; set; }
        public string Unknown_7928 { get; set; }
        public string Unknown_8000 { get; set; }

        /// <summary>
        /// Price per unit of quantity
        /// </summary>
        public decimal Price { get; set; }
                
        /// <summary>
        /// Time the details within the message should take effect, always UTC
        /// </summary>
        public DateTime EffectiveTime { get; set; }

        /// <summary>
        /// Free format text defining the designation to be associated with a holding on the register
        /// </summary>
        public string Designation { get; set; }

        /// <summary>
        /// Side of order
        /// </summary>
        public char Side { get; set; }                      // TODO: https://www.onixs.biz/fix-dictionary/4.4/tagNum_54.html
        
        /// <summary>
        /// Ticker symbol
        /// </summary>
        public string Symbol { get; set; }
        
        /// <summary>
        /// Specifies how long the order remains in effect. 
        /// Absence of this field is interpreted as DAY.
        /// </summary>
        public char TimeInForce { get; set; }               // TODO: https://www.onixs.biz/fix-dictionary/4.4/tagNum_59.html

        /// <summary>
        /// Time of execution/order creation, UTC
        /// </summary>
        public DateTime TransactTime { get; set; }

        /// <summary>
        /// Identifies specific message originator's location
        /// </summary>
        public string SenderLocationId { get; set; }

        /// <summary>
        /// Month and Year of the maturity
        /// https://www.onixs.biz/fix-dictionary/4.4/tagNum_200.html
        /// TODO: review validation, if we actually need it
        /// </summary>
        public string MaturityMonthYear { get; set; }

        /// <summary>
        /// Market used to help identify a security.
        /// https://www.onixs.biz/fix-dictionary/4.4/tagNum_207.html
        /// </summary>
        public string SecurityExchange { get; set; }

        /// <summary>
        /// Type of security using ISO 0962 standard, Classification of Financial Instruments (CFI code) values
        /// </summary>
        public string CFICode { get; set; }
    }

    public class Party
    {
        public string Id { get; set; }
        public char PartyIdSource { get; set; }     // TODO: https://www.onixs.biz/fix-dictionary/4.4/tagNum_447.html
        public int PartyRole { get; set; }          // TODO: https://www.onixs.biz/fix-dictionary/4.4/tagNum_452.html
        public PartySubId[] PartySubIds { get; set; }
    }

    public class PartySubId
    {
        public string Id { get; set; }
        public string Type { get; set; } // TODO: https://www.onixs.biz/fix-dictionary/4.4/tagNum_803.html
    }
}
