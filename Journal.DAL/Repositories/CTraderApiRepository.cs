using Google.Protobuf;
using Google.Protobuf.Collections;
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

            await Task.Delay(300);

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

            await Task.Delay(300);

            return taskCompletionSource.Task.Result;
        }

        public async Task<ProtoOATrader> AccountStateRequest(string accessToken, long accountId, bool isLive)
        {

            var _token = new Token()
            {
                AccessToken = accessToken,
            };

            var host = ApiInfo.GetHost(mode: isLive ? Mode.Live : Mode.Demo);
            var client = new OpenClient(host, ApiInfo.Port, TimeSpan.FromSeconds(10), useWebSocket: useWebScoket);

            _disposables.Add(client.Where(iMessage => iMessage is not ProtoHeartbeatEvent).Subscribe(OnMessageReceived, OnException));

            await client.Connect();

            var applicationAuthReq = new ProtoOAApplicationAuthReq
            {
                ClientId = _app.ClientId,
                ClientSecret = _app.Secret,
            };


            await client.SendMessage(applicationAuthReq);

            await Task.Delay(300);

            var authRequst = new ProtoOAAccountAuthReq
            {
                CtidTraderAccountId = accountId,
                AccessToken = _token.AccessToken
            };

            await client.SendMessage(authRequst);

            await Task.Delay(100);

            var taskCompletionSource = new TaskCompletionSource<ProtoOATraderRes>();

            IDisposable disposable = null;

            disposable = client.OfType<ProtoOATraderRes>().Where(response => response.CtidTraderAccountId == accountId).Subscribe(response =>
            {
                taskCompletionSource.SetResult(response);

                disposable?.Dispose();
            });

            var request = new ProtoOATraderReq
            {
                CtidTraderAccountId = accountId
            };

            await client.SendMessage(request);

            return taskCompletionSource.Task.Result.Trader;
        }

        public async Task<RepeatedField<ProtoOADeal>> GetDeals(string accessToken, long accountId, bool isLive)
        {
            var _token = new Token()
            {
                AccessToken = accessToken,
            };

            var host = ApiInfo.GetHost(mode: isLive ? Mode.Live : Mode.Demo);
            var client = new OpenClient(host, ApiInfo.Port, TimeSpan.FromSeconds(10), useWebSocket: useWebScoket);

            _disposables.Add(client.Where(iMessage => iMessage is not ProtoHeartbeatEvent).Subscribe(OnMessageReceived, OnException));

            await client.Connect();

            var applicationAuthReq = new ProtoOAApplicationAuthReq
            {
                ClientId = _app.ClientId,
                ClientSecret = _app.Secret,
            };


            await client.SendMessage(applicationAuthReq);

            await Task.Delay(300);

            var authRequst = new ProtoOAAccountAuthReq
            {
                CtidTraderAccountId = accountId,
                AccessToken = _token.AccessToken
            };

            await client.SendMessage(authRequst);

            await Task.Delay(100);

            var taskCompletionSource = new TaskCompletionSource<ProtoOADealListRes>();

            IDisposable disposable = null;

            disposable = client.OfType<ProtoOADealListRes>().Where(response => response.CtidTraderAccountId == accountId).Subscribe(response =>
            {
                taskCompletionSource.SetResult(response);

                disposable?.Dispose();
            });

            var request = new ProtoOADealListReq
            {
                CtidTraderAccountId = accountId,
                FromTimestamp = 0,
                ToTimestamp = 2147483645000,
                MaxRows = 1000,
            };

            await client.SendMessage(request);

            return taskCompletionSource.Task.Result.Deal;
        }

        public async Task<RepeatedField<ProtoOAOrder>> GetOrders(string accessToken, long accountId, bool isLive)
        {
            var _token = new Token()
            {
                AccessToken = accessToken,
            };

            var host = ApiInfo.GetHost(mode: isLive ? Mode.Live : Mode.Demo);
            var client = new OpenClient(host, ApiInfo.Port, TimeSpan.FromSeconds(10), useWebSocket: useWebScoket);

            _disposables.Add(client.Where(iMessage => iMessage is not ProtoHeartbeatEvent).Subscribe(OnMessageReceived, OnException));

            await client.Connect();

            var applicationAuthReq = new ProtoOAApplicationAuthReq
            {
                ClientId = _app.ClientId,
                ClientSecret = _app.Secret,
            };


            await client.SendMessage(applicationAuthReq);

            await Task.Delay(300);

            var authRequst = new ProtoOAAccountAuthReq
            {
                CtidTraderAccountId = accountId,
                AccessToken = _token.AccessToken
            };

            await client.SendMessage(authRequst);

            await Task.Delay(100);

            var taskCompletionSource = new TaskCompletionSource<ProtoOAReconcileRes>();

            IDisposable disposable = null;

            disposable = client.OfType<ProtoOAReconcileRes>().Where(response => response.CtidTraderAccountId == accountId).Subscribe(response =>
            {
                taskCompletionSource.SetResult(response);

                disposable?.Dispose();
            });

            var request = new ProtoOAReconcileReq
            {
                CtidTraderAccountId = accountId,
            };

            await client.SendMessage(request);

            return taskCompletionSource.Task.Result.Order;
        }

        public async Task<RepeatedField<ProtoOAPosition>> GetPositions(string accessToken, long accountId, bool isLive)
        {
            var _token = new Token()
            {
                AccessToken = accessToken,
            };

            var host = ApiInfo.GetHost(mode: isLive ? Mode.Live : Mode.Demo);
            var client = new OpenClient(host, ApiInfo.Port, TimeSpan.FromSeconds(10), useWebSocket: useWebScoket);

            _disposables.Add(client.Where(iMessage => iMessage is not ProtoHeartbeatEvent).Subscribe(OnMessageReceived, OnException));

            await client.Connect();

            var applicationAuthReq = new ProtoOAApplicationAuthReq
            {
                ClientId = _app.ClientId,
                ClientSecret = _app.Secret,
            };


            await client.SendMessage(applicationAuthReq);

            await Task.Delay(300);

            var authRequst = new ProtoOAAccountAuthReq
            {
                CtidTraderAccountId = accountId,
                AccessToken = _token.AccessToken
            };

            await client.SendMessage(authRequst);

            await Task.Delay(100);

            var taskCompletionSource = new TaskCompletionSource<ProtoOAReconcileRes>();

            IDisposable disposable = null;

            disposable = client.OfType<ProtoOAReconcileRes>().Where(response => response.CtidTraderAccountId == accountId).Subscribe(response =>
            {
                taskCompletionSource.SetResult(response);

                disposable?.Dispose();
            });

            var request = new ProtoOAReconcileReq
            {
                CtidTraderAccountId = accountId,
            };

            await client.SendMessage(request);

            return taskCompletionSource.Task.Result.Position;
        }

        public async Task<bool> PlaceOrder(string accessToken, long accountId, bool isLive, string symbol, byte type, long volume, double stopLoss, double takeProfit, double price)
        {
            
            
            var _token = new Token()
            {
                AccessToken = accessToken,
            };

            var host = ApiInfo.GetHost(mode: isLive ? Mode.Live : Mode.Demo);
            var client = new OpenClient(host, ApiInfo.Port, TimeSpan.FromSeconds(10), useWebSocket: useWebScoket);

            var symbols = await GetSymbols(accessToken, accountId, isLive);
            var symbolId = symbols.FirstOrDefault(x => x.SymbolName == symbol).SymbolId;

            ProtoOAOrderType orderType = ProtoOAOrderType.Market;
            ProtoOATradeSide tradeSide = ProtoOATradeSide.Buy;

            switch (type)
            {
                case 1:
                    orderType = ProtoOAOrderType.Market;
                    tradeSide = ProtoOATradeSide.Buy;
                    break;
                case 2:
                    orderType = ProtoOAOrderType.Market;
                    tradeSide = ProtoOATradeSide.Sell;
                    break;
                case 3:
                    orderType = ProtoOAOrderType.Limit;
                    tradeSide = ProtoOATradeSide.Buy;
                    break;
                case 4:
                    orderType = ProtoOAOrderType.Limit;
                    tradeSide = ProtoOATradeSide.Sell;
                    break;
                default:
                    return false;
            }

            int countBefore = 0;

            if (orderType == ProtoOAOrderType.Limit)
            {
                countBefore = (await GetDeals(accessToken, accountId, isLive)).Count();
            }
            else
            {
                countBefore = (await GetPositions(accessToken, accountId, isLive)).Count();
            }

            _disposables.Add(client.Where(iMessage => iMessage is not ProtoHeartbeatEvent).Subscribe(OnMessageReceived, OnException));

            await client.Connect();

            var applicationAuthReq = new ProtoOAApplicationAuthReq
            {
                ClientId = _app.ClientId,
                ClientSecret = _app.Secret,
            };


            await client.SendMessage(applicationAuthReq);

            await Task.Delay(300);

            var authRequst = new ProtoOAAccountAuthReq
            {
                CtidTraderAccountId = accountId,
                AccessToken = _token.AccessToken
            };

            await client.SendMessage(authRequst);

            await Task.Delay(100);

            ProtoOANewOrderReq request = new ProtoOANewOrderReq();
            if (orderType == ProtoOAOrderType.Market)
            {
                request = new ProtoOANewOrderReq
                {
                    CtidTraderAccountId = accountId,
                    SymbolId = symbolId,
                    OrderType = orderType,
                    TradeSide = tradeSide,
                    Volume = volume
                };
            }
            else {
                if(stopLoss != 0 && takeProfit != 0)
                request = new ProtoOANewOrderReq
                {
                    CtidTraderAccountId = accountId,
                    SymbolId = symbolId,
                    OrderType = orderType,
                    TradeSide = tradeSide,
                    Volume = volume,
                    LimitPrice = price,
                    StopLoss = stopLoss,
                    TakeProfit = takeProfit,
                };
                else if(stopLoss == 0)
                {
                    request = new ProtoOANewOrderReq
                    {
                        CtidTraderAccountId = accountId,
                        SymbolId = symbolId,
                        OrderType = orderType,
                        TradeSide = tradeSide,
                        Volume = volume,
                        LimitPrice = price,
                        TakeProfit = takeProfit,
                    };
                }
                else if(takeProfit == 0)
                {
                    request = new ProtoOANewOrderReq
                    {
                        CtidTraderAccountId = accountId,
                        SymbolId = symbolId,
                        OrderType = orderType,
                        TradeSide = tradeSide,
                        Volume = volume,
                        LimitPrice = price,
                        StopLoss = stopLoss,
                    };
                }
                else
                {
                    request = new ProtoOANewOrderReq
                    {
                        CtidTraderAccountId = accountId,
                        SymbolId = symbolId,
                        OrderType = orderType,
                        TradeSide = tradeSide,
                        Volume = volume,
                        LimitPrice = price,
                    };
                }
            }

            await client.SendMessage(request);

            int countAfter = 0;

            if (orderType == ProtoOAOrderType.Limit)
            {
                countAfter = (await GetDeals(accessToken, accountId, isLive)).Count();
            }
            else
            {
                countAfter = (await GetPositions(accessToken, accountId, isLive)).Count();
            }


            return countAfter > countBefore;
        }

        public async Task DeleteOrder(string accessToken, long accountId, bool isLive, long id)
        {
            var _token = new Token()
            {
                AccessToken = accessToken,
            };

            var host = ApiInfo.GetHost(mode: isLive ? Mode.Live : Mode.Demo);
            var client = new OpenClient(host, ApiInfo.Port, TimeSpan.FromSeconds(10), useWebSocket: useWebScoket);

            _disposables.Add(client.Where(iMessage => iMessage is not ProtoHeartbeatEvent).Subscribe(OnMessageReceived, OnException));

            await client.Connect();

            var applicationAuthReq = new ProtoOAApplicationAuthReq
            {
                ClientId = _app.ClientId,
                ClientSecret = _app.Secret,
            };


            await client.SendMessage(applicationAuthReq);

            await Task.Delay(300);

            var authRequst = new ProtoOAAccountAuthReq
            {
                CtidTraderAccountId = accountId,
                AccessToken = _token.AccessToken
            };

            await client.SendMessage(authRequst);

            await Task.Delay(100);


            var request = new ProtoOACancelOrderReq
            {
                CtidTraderAccountId = accountId,
                OrderId = id
            };
            await client.SendMessage(request);
        }

        public async Task DeletePosition(string accessToken, long accountId, bool isLive, long id, long volume)
        {
            var _token = new Token()
            {
                AccessToken = accessToken,
            };

            var host = ApiInfo.GetHost(mode: isLive ? Mode.Live : Mode.Demo);
            var client = new OpenClient(host, ApiInfo.Port, TimeSpan.FromSeconds(10), useWebSocket: useWebScoket);

            _disposables.Add(client.Where(iMessage => iMessage is not ProtoHeartbeatEvent).Subscribe(OnMessageReceived, OnException));

            await client.Connect();

            var applicationAuthReq = new ProtoOAApplicationAuthReq
            {
                ClientId = _app.ClientId,
                ClientSecret = _app.Secret,
            };


            await client.SendMessage(applicationAuthReq);

            await Task.Delay(300);

            var authRequst = new ProtoOAAccountAuthReq
            {
                CtidTraderAccountId = accountId,
                AccessToken = _token.AccessToken
            };

            await client.SendMessage(authRequst);

            await Task.Delay(100);


            var request = new ProtoOAClosePositionReq
            {
                CtidTraderAccountId = accountId,
                PositionId = id,
                Volume = volume
            };
            await client.SendMessage(request);
        }

        public async Task<RepeatedField<ProtoOALightSymbol>> GetSymbols(string accessToken, long accountId, bool isLive)
        {
            var _token = new Token()
            {
                AccessToken = accessToken,
            };

            var host = ApiInfo.GetHost(mode: isLive ? Mode.Live : Mode.Demo);
            var client = new OpenClient(host, ApiInfo.Port, TimeSpan.FromSeconds(10), useWebSocket: useWebScoket);

            _disposables.Add(client.Where(iMessage => iMessage is not ProtoHeartbeatEvent).Subscribe(OnMessageReceived, OnException));

            await client.Connect();

            var applicationAuthReq = new ProtoOAApplicationAuthReq
            {
                ClientId = _app.ClientId,
                ClientSecret = _app.Secret,
            };


            await client.SendMessage(applicationAuthReq);

            await Task.Delay(300);

            var authRequst = new ProtoOAAccountAuthReq
            {
                CtidTraderAccountId = accountId,
                AccessToken = _token.AccessToken
            };

            await client.SendMessage(authRequst);

            await Task.Delay(100);

            var taskCompletionSource = new TaskCompletionSource<ProtoOASymbolsListRes>();

            IDisposable disposable = null;

            disposable = client.OfType<ProtoOASymbolsListRes>().Where(response => response.CtidTraderAccountId == accountId).Subscribe(response =>
            {
                taskCompletionSource.SetResult(response);

                disposable?.Dispose();
            });

            var request = new ProtoOASymbolsListReq
            {
                CtidTraderAccountId = accountId,
            };

            await client.SendMessage(request);

            return taskCompletionSource.Task.Result.Symbol;
        }

        public async Task<RepeatedField<ProtoOASymbolCategory>> GetSymbolCategories(string accessToken, long accountId, bool isLive)
        {
            var _token = new Token()
            {
                AccessToken = accessToken,
            };

            var host = ApiInfo.GetHost(mode: isLive ? Mode.Live : Mode.Demo);
            var client = new OpenClient(host, ApiInfo.Port, TimeSpan.FromSeconds(10), useWebSocket: useWebScoket);

            _disposables.Add(client.Where(iMessage => iMessage is not ProtoHeartbeatEvent).Subscribe(OnMessageReceived, OnException));

            await client.Connect();

            var applicationAuthReq = new ProtoOAApplicationAuthReq
            {
                ClientId = _app.ClientId,
                ClientSecret = _app.Secret,
            };


            await client.SendMessage(applicationAuthReq);

            await Task.Delay(300);

            var authRequst = new ProtoOAAccountAuthReq
            {
                CtidTraderAccountId = accountId,
                AccessToken = _token.AccessToken
            };

            await client.SendMessage(authRequst);

            await Task.Delay(100);

            var taskCompletionSource = new TaskCompletionSource<ProtoOASymbolCategoryListRes>();

            IDisposable disposable = null;

            disposable = client.OfType<ProtoOASymbolCategoryListRes>().Where(response => response.CtidTraderAccountId == accountId).Subscribe(response =>
            {
                taskCompletionSource.SetResult(response);

                disposable?.Dispose();
            });

            var request = new ProtoOASymbolCategoryListReq
            {
                CtidTraderAccountId = accountId,
            };

            await client.SendMessage(request);

            return taskCompletionSource.Task.Result.SymbolCategory;
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