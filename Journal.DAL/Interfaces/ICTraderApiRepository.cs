using Google.Protobuf;
using Google.Protobuf.Collections;
using OpenAPI.Net;
using OpenAPI.Net.Auth;
using OpenAPI.Net.Helpers;
using System.Diagnostics.Eventing.Reader;

namespace Journal.DAL.Interfaces
{
    public interface ICTraderApiRepository
    {
        Task<ProtoOAGetAccountListByAccessTokenRes> AccountListRequest(string accessToken);

        Task<ProtoOATrader> AccountStateRequest(string accessToken, long accountId, bool isLive);

        Task<RepeatedField<ProtoOAOrder>> GetOrders(string accessToken, long accountId, bool isLive);

        Task<RepeatedField<ProtoOAPosition>> GetPositions(string accessToken, long accountId, bool isLive);

        Task<bool> PlaceOrder(string accessToken, long accountId, bool isLive, string symbol, byte type, long volume, double stopLoss, double takeProfit, double price);

        Task<string> GetAccessToken(string authorizationCode);

        Task<RepeatedField<ProtoOADeal>> GetDeals(string accessToken, long accountId, bool isLive);

        Task DeletePosition(string accessToken, long accountId, bool isLive, long id, long volume);

        Task DeleteOrder(string accessToken, long accountId, bool isLive, long id);

        Task<RepeatedField<ProtoOALightSymbol>> GetSymbols(string accessToken, long accountId, bool isLive);

        Task<RepeatedField<ProtoOASymbolCategory>> GetSymbolCategories(string accessToken, long accountId, bool isLive);
    }
}
