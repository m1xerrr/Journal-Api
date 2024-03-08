using Google.Protobuf;
using Journal.DAL.Interfaces;
using Newtonsoft.Json.Linq;
using OpenAPI.Net;
using OpenAPI.Net.Auth;
using OpenAPI.Net.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.DAL.Repositories
{
    public class CTraderApiRepository : ICTraderApiRepository
    {
        private static string appId = "8551_8nBBM0Gv2ktbYuHUbX9O1nggVPtnaGUXikE4V79esNbAMVtPWL";

        private static string appSecret = "31illrh4GurxK3ER5o7gzDE1q9CYLv8K3XYzhBw4aSC4FDOE7i";

        private static bool useWebScoket = true;

        private static string redirectUrl = "https://openapi.ctrader.com/apps/8551/playground";

        private static App _app = new App(appId, appSecret, redirectUrl);

        private static Uri authUri = _app.GetAuthUri();

        private static readonly List<IDisposable> _disposables = new();

        public async Task<ProtoOAGetAccountListByAccessTokenRes> AccountListRequest(string accessToken)
        {
            
            var _token = new Token()
            {
                AccessToken = accessToken,
            };

            var host = ApiInfo.GetHost(mode: Mode.Live);
            var client = new OpenClient(host, ApiInfo.Port, TimeSpan.FromSeconds(10), useWebSocket: useWebScoket);

            _disposables.Add(client.Where(iMessage => iMessage is not ProtoHeartbeatEvent).Subscribe(OnMessageReceived, OnException));

            await client.Connect();

            var applicationAuthReq = new ProtoOAApplicationAuthReq
            {
                ClientId = _app.ClientId,
                ClientSecret = _app.Secret,
            };


            await client.SendMessage(applicationAuthReq);

            Task.Delay(300).Wait();

            var taskCompletionSource = new TaskCompletionSource<ProtoOAGetAccountListByAccessTokenRes>();

            IDisposable disposable = null;

            disposable = client.OfType<ProtoOAGetAccountListByAccessTokenRes>().Where(response => response.AccessToken == accessToken).Subscribe(response =>
            {
                taskCompletionSource.SetResult(response);

                disposable?.Dispose();
            });

            var request = new ProtoOAGetAccountListByAccessTokenReq
            {
                AccessToken = _token.AccessToken,
            };

            await client.SendMessage(request);

            return taskCompletionSource.Task.Result;
        }

        private static void OnMessageReceived(IMessage message)
        {
            Console.WriteLine($"\nMessage Received:\n{message}");

            Console.WriteLine();
        }

        private static void OnException(Exception ex)
        {
            Console.WriteLine($"\nException\n: {ex}");
        }

        public async Task<string> GetAccessToken(string authorizationCode)
        {       
            var _token = await TokenFactory.GetToken(authorizationCode, _app);
                
            return _token.AccessToken;
            
        }
    }
}