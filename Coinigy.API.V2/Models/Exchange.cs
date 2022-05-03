using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coinigy.API.V2.Models
{
    public class Exchange
    {
        [JsonProperty("exchJson")]
        public string ExchJson { get; set; }

        [JsonProperty("exchFee")]
        public decimal ExchFee { get; set; }

        [JsonProperty("exchFeeUpfront")]
        public bool ExchFeeUpfront { get; set; }

        [JsonProperty("exchFeeBuy")]
        public bool ExchFeeBuy { get; set; }

        [JsonProperty("exchFeeSell")]
        public bool ExchFeeSell { get; set; }

        [JsonProperty("exchTimeOffset")]
        public string ExchTimeOffset { get; set; }

        [JsonProperty("exchAggEnabled")]
        public bool ExchAggEnabled { get; set; }

        [JsonProperty("exchBalancesUnavailable")]
        public bool ExchBalancesUnavailable { get; set; }

        [JsonProperty("exchTradingUnavailable")]
        public bool ExchTradingUnavailable { get; set; }

        [JsonProperty("exchAggPriority")]
        public int? ExchAggPriority { get; set; }

        [JsonProperty("exchAlertThreshold")]
        public int? ExchAlertThreshold { get; set; }

        [JsonProperty("exchAlertStatus")]
        public bool ExchAlertStatus { get; set; }

        [JsonProperty("exchAlertLastChecked")]
        public DateTime ExchAlertLastChecked { get; set; }

        [JsonProperty("exchAlertLastSuccess")]
        public DateTime ExchAlertLastSuccess { get; set; }

        [JsonProperty("exchWsOrders")]
        public bool ExchWsOrders { get; set; }

        [JsonProperty("exchAggInstanceId")]
        public int? ExchAggInstanceId { get; set; }

        [JsonProperty("exchMonitorEnabled")]
        public bool ExchMonitorEnabled { get; set; }

        [JsonProperty("exchId")]
        public int ExchId { get; set; }

        [JsonProperty("exchName")]
        public string ExchName { get; set; }

        [JsonProperty("exchCode")]
        public string ExchCode { get; set; }

        [JsonProperty("exchTradeEnabled")]
        public bool ExchTradeEnabled { get; set; }

        [JsonProperty("exchEnabled")]
        public bool ExchEnabled { get; set; }

        [JsonProperty("exchBalanceEnabled")]
        public bool ExchBalanceEnabled { get; set; }

        [JsonProperty("exchUrl")]
        public string ExchUrl { get; set; }

        [JsonProperty("exchAdded")]
        public DateTime ExchAdded { get; set; }

        [JsonProperty("exchPricesIndexed")]
        public bool ExchPricesIndexed { get; set; }

        [JsonProperty("exchMarketsInverted")]
        public bool ExchMarketsInverted { get; set; }

        [JsonProperty("hasLimitExchange")]
        public bool HasLimitExchange { get; set; }

        [JsonProperty("hasLimitPaper")]
        public bool HasLimitPaper { get; set; }

        [JsonProperty("hasStopLimitExchange")]
        public bool HasStopLimitExchange { get; set; }

        [JsonProperty("hasStopLimitCoinigy")]
        public bool HasStopLimitCoinigy { get; set; }

        [JsonProperty("hasStopLimitPaper")]
        public bool HasStopLimitPaper { get; set; }

        [JsonProperty("hasMarketExchange")]
        public bool HasMarketExchange { get; set; }

        [JsonProperty("hasMarketCoinigy")]
        public bool HasMarketCoinigy { get; set; }

        [JsonProperty("hasMarketPaper")]
        public bool HasMarketPaper { get; set; }

        [JsonProperty("hasStopMarketExchange")]
        public bool HasStopMarketExchange { get; set; }

        [JsonProperty("hasStopMarketCoinigy")]
        public bool HasStopMarketCoinigy { get; set; }

        [JsonProperty("hasStopMarketPaper")]
        public bool HasStopMarketPaper { get; set; }

        [JsonProperty("exchTutorialUrl")]
        public string ExchTutorialUrl { get; set; }

        [JsonProperty("optionalRequired")]
        public bool OptionalRequired { get; set; }

        [JsonProperty("optionalFieldName")]
        public string OptionalFieldName { get; set; }

        [JsonProperty("optionalFieldDesc")]
        public string OptionalFieldDesc { get; set; }

        [JsonProperty("exchHasWhitelisting")]
        public bool ExchHasWhitelisting { get; set; }

        [JsonProperty("exchBalanceEnabledV2")]
        public bool ExchBalanceEnabledV2 { get; set; }

        [JsonProperty("exchTradeEnabledV2")]
        public bool ExchTradeEnabledV2 { get; set; }

        [JsonProperty("hasSpotTrading")]
        public bool HasSpotTrading { get; set; }

        [JsonProperty("hasFuturesTrading")]
        public bool HasFuturesTrading { get; set; }

        [JsonProperty("hasSpotMarginTrading")]
        public bool HasSpotMarginTrading { get; set; }

        [JsonProperty("hasFuturesMarginTrading")]
        public bool HasFuturesMarginTrading { get; set; }

        [JsonProperty("exchVolume24")]
        public decimal ExchVolume24 { get; set; }

        [JsonProperty("exchVolume1")]
        public decimal ExchVolume1 { get; set; }

        [JsonProperty("favoritesScore")]
        public int FavoritesScore { get; set; }

        [JsonProperty("trafficRank")]
        public int? TrafficRank { get; set; }

        [JsonProperty("trafficRank90dChange")]
        public int? TrafficRank90dChange { get; set; }
    }
}
