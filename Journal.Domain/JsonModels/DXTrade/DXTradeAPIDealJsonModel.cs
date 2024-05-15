using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.JsonModels.DXTrade
{
    public class DXTradeAPIDealLegJsonModel
    {
        public string Instrument { get; set; }
        public string PositionEffect { get; set; }
        public string PositionCode { get; set; }
        public double Price { get; set; }
        public double LegRatio { get; set; }
        public double? Quantity { get; set; }
        public double FilledQuantity { get; set; }
        public double RemainingQuantity { get; set; }
        public double AveragePrice { get; set; }
    }

    public class DXTradeAPIDealExecutionJsonModel
    {
        public string Account { get; set; }
        public string ExecutionCode { get; set; }
        public string OrderCode { get; set; }
        public int UpdateOrderId { get; set; }
        public int Version { get; set; }
        public string ClientOrderId { get; set; }
        public string ActionCode { get; set; }
        public string Status { get; set; }
        public bool FinalStatus { get; set; }
        public double FilledQuantity { get; set; }
        public double LastQuantity { get; set; }
        public double FilledQuantityNotional { get; set; }
        public double LastQuantityNotional { get; set; }
        public DateTime TransactionTime { get; set; }
        public double? LastPrice { get; set; }
        public double? AveragePrice { get; set; }
        public double? RemainingQuantity { get; set; }
        public string Instrument { get; set; }
    }

    public class DXTradeAPIDealCashTransactionJsonModel
    {
        public string Account { get; set; }
        public string TransactionCode { get; set; }
        public string OrderCode { get; set; }
        public string TradeCode { get; set; }
        public int Version { get; set; }
        public string ClientOrderId { get; set; }
        public string Type { get; set; }
        public double Value { get; set; }
        public string Currency { get; set; }
        public DateTime TransactionTime { get; set; }
    }

    public class DXTradeAPIDealOrderJsonModel
    {
        public string Account { get; set; }
        public int Version { get; set; }
        public int OrderId { get; set; }
        public string OrderCode { get; set; }
        public string ClientOrderId { get; set; }
        public string ActionCode { get; set; }
        public int LegCount { get; set; }
        public string Type { get; set; }
        public string Instrument { get; set; }
        public string Status { get; set; }
        public bool FinalStatus { get; set; }
        public List<DXTradeAPIDealLegJsonModel> Legs { get; set; }
        public string Side { get; set; }
        public string Tif { get; set; }
        public DateTime IssueTime { get; set; }
        public DateTime TransactionTime { get; set; }
        public List<DXTradeAPIDealExecutionJsonModel> Executions { get; set; }
        public List<DXTradeAPIDealCashTransactionJsonModel> CashTransactions { get; set; }
    }

    public class DXTradeAPIDealJsonModel
    {
        public List<DXTradeAPIDealOrderJsonModel> Orders { get; set; }
    }
}
