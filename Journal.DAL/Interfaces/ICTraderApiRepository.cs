using Google.Protobuf;
using OpenAPI.Net;
using OpenAPI.Net.Auth;
using OpenAPI.Net.Helpers;

namespace Journal.DAL.Interfaces
{
    public interface ICTraderApiRepository
    {
        Task<ProtoOAGetAccountListByAccessTokenRes> AccountListRequest(string accessToken);

        Task<string> GetAccessToken(string authorizationCode);
    }
}
