using Newtonsoft.Json;

namespace Coinigy.API.Responses
{
    [JsonObject(MemberSerialization = MemberSerialization.Fields)]
    public class Balance
    {
        //public string balance_amount_avail;
        //public string balance_amount_held;
        //public string balance_amount_total;
        //public string balance_curr_code;
        //public string btc_balance;
        //public string last_price;

        public string balanceId;
        public string balanceAuthId;
        public bool  fiat;
        public string balanceCurrCode;
        public int balanceCurrId;
        public decimal balanceAmountAvailable;
        public decimal balanceAmountHeld;
        public decimal balanceAmountTotal;
        public decimal quoteBalance;
        public decimal lastTrade;
        public string balanceQuoteCurrCode;
        public decimal lastPrice;
        public string balanceDate;
        public bool balanceHidden;
        public bool hasImage;
        public string balanceCurrSlug;
        public string balanceCurrName;
        public string balanceCurrImgName;

    }
}