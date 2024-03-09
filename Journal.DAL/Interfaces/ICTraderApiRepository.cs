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

        Task<string> GetAccessToken(string authorizationCode);

        Task<RepeatedField<ProtoOADeal>> GetDeals(string accessToken, long accountId, bool isLive);


        Task<RepeatedField<ProtoOALightSymbol>> GetSymbols(string accessToken, long accountId, bool isLive);
    }
}
