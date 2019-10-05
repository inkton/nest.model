/*
    Copyright (c) 2017 Inkton.

    Permission is hereby granted, free of charge, to any person obtaining
    a copy of this software and associated documentation files (the "Software"),
    to deal in the Software without restriction, including without limitation
    the rights to use, copy, modify, merge, publish, distribute, sublicense,
    and/or sell copies of the Software, and to permit persons to whom the Software
    is furnished to do so, subject to the following conditions:

    The above copyright notice and this permission ServerStatusnotice shall be included in
    all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
    EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
    OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
    IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
    CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
    TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE
    OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Inkton.Nest.Model;
using Inkton.Nest.Storage;

namespace Inkton.Nest.Cloud
{
    public delegate Task<ResultT> HttpRequest<PayloadT, ResultT>(
        PayloadT seed, IFlurlRequest flurlRequestl) where PayloadT : ICloudObject, new();
    public delegate Task<ResultT> CachedHttpRequest<PayloadT, ResultT>(PayloadT seed,
        IDictionary<string, object> data, string subPath = null, bool doCache = true) where PayloadT : ICloudObject, new();

    public class BackendService<UserT> : IBackendService<UserT>
        where UserT : User, new()
    {
        public int Version { get; set; } = 1;

        public string DeviceSignature { get; set; } = "-nest.yt-";

        public string Address { get; set; }

        public string ApiEndpoint { get; set; } = "api/";

        public Permit<UserT> Permit { get; set; } = new Permit<UserT>();

        public bool AutoTokenRenew { get; set; } = true;

        public int RetryCount { get; set; } = 3;

        public int RetryBaseIntervalInSecs { get; set; } = 2;

        public IBackendServiceNotify Notifier { get; set; }

        public BasicAuth BasicAuth { get; set; }

        private ObjectStore _cache = new ObjectStore(Path.GetTempPath() +
                 Guid.NewGuid() + "-cache");

        public async Task<ResultSingle<Permit<UserT>>> SignupAsync(
            Dictionary<string, object> data = null)
        {
            Notifier?.BeginQuery();

            ResultSingle<Permit<UserT>> result = await
                Send<Permit<UserT>, ResultSingle<Permit<UserT>>, Permit<UserT>>(
                    new HttpRequest<Permit<UserT>, ResultSingle<Permit<UserT>>>(PostAsync),
                    Permit, false, data, null, false);

            UpdatePermit(result);

            Notifier?.EndQuery();

            return result;
        }

        public async Task<ResultSingle<Permit<UserT>>> SetupPermitAsync(
            Dictionary<string, object> data = null)
        {
            Notifier?.BeginQuery();

            ResultSingle<Permit<UserT>> result = await
                Send<Permit<UserT>, ResultSingle<Permit<UserT>>, Permit<UserT>>(
                    new HttpRequest<Permit<UserT>, ResultSingle<Permit<UserT>>>(PutAsync),
                    Permit, true, data, null, false);

            UpdatePermit(result);

            Notifier?.EndQuery();

            return result;
        }

        public async Task<ResultSingle<Permit<UserT>>> RenewAccessAsync(
            Dictionary<string, object> data = null)
        {
            Notifier?.BeginQuery();

            Permit.UseRefreshToken();

            ResultSingle<Permit<UserT>> result = await
                Send<Permit<UserT>, ResultSingle<Permit<UserT>>, Permit<UserT>>(
                    new HttpRequest<Permit<UserT>, ResultSingle<Permit<UserT>>>(GetAsync),
                    Permit, true, data, null, false);

            UpdatePermit(result);

            Notifier?.EndQuery();

            return result;
        }

        public async Task<ResultSingle<Permit<UserT>>> RevokeAccessAsync(
            Dictionary<string, object> data = null)
        {
            Notifier?.BeginQuery();

            ResultSingle<Permit<UserT>> result = await
                Send<Permit<UserT>, ResultSingle<Permit<UserT>>, Permit<UserT>>(
                    new HttpRequest<Permit<UserT>, ResultSingle<Permit<UserT>>>(DeleteAsync),
                    Permit, true, data, null, false);

            Notifier?.EndQuery();

            return result;
        }

        public async Task<ResultSingle<ObjectT>> CreateAsync<ObjectT>(
            ObjectT seed, IDictionary<string, object> data = null,
            string subPath = null, bool doCache = true)
            where ObjectT : ICloudObject, new()
        {
            Notifier?.BeginQuery();

            ResultSingle<ObjectT> result = await TrySend<ObjectT, ResultSingle<ObjectT>, ObjectT>(
                new HttpRequest<ObjectT, ResultSingle<ObjectT>>(PostAsync),
                seed, false, data, subPath, doCache);

            Notifier?.EndQuery();

            return result;
        }

        public async Task<ResultSingle<ObjectT>> QueryAsync<ObjectT>(
            ObjectT seed, IDictionary<string, object> data = null,
            string subPath = null, bool doCache = true)
            where ObjectT : ICloudObject, new()
        {
            Notifier?.BeginQuery();

            ResultSingle<ObjectT> result = null;

            if (doCache && _cache.Load<ObjectT>(seed))
            {
                // Use the cache
                result = new ResultSingle<ObjectT>(ServerStatus.NEST_RESULT_SUCCESS);
                result.Data = new DataContainer<ObjectT>();
                result.Data.Payload = seed;
            }
            else
            {
                result = await TrySend<ObjectT, ResultSingle<ObjectT>, ObjectT>(
                    new HttpRequest<ObjectT, ResultSingle<ObjectT>>(GetAsync),
                    seed, true, data, subPath, doCache);
            }

            Notifier?.EndQuery();

            return result;
        }

        public async Task<ResultMultiple<ObjectT>> QueryAsyncListAsync<ObjectT>(
            ObjectT seed, IDictionary<string, object> data = null,
            string subPath = null, bool doCache = true)
            where ObjectT : ICloudObject, new()
        {
            Notifier?.BeginQuery();

            ResultMultiple<ObjectT> result = await TrySend<ObjectT, ResultMultiple<ObjectT>,
                ObservableCollection<ObjectT>>(new HttpRequest<ObjectT, ResultMultiple<ObjectT>>(GetListAsync),
                seed, false, data, subPath, doCache);

            Notifier?.EndQuery();

            return result;
        }

        public async Task<ResultSingle<ObjectT>> UpdateAsync<ObjectT>(
            ObjectT seed, IDictionary<string, object> data = null,
            string subPath = null, bool doCache = true)
            where ObjectT : ICloudObject, new()
        {
            Notifier?.BeginQuery();

            ResultSingle<ObjectT> result = await TrySend<ObjectT, ResultSingle<ObjectT>, ObjectT>(
                new HttpRequest<ObjectT, ResultSingle<ObjectT>>(PutAsync),
                seed, true, data, subPath, doCache);

            Notifier?.EndQuery();

            return result;
        }

        public async Task<ResultSingle<ObjectT>> RemoveAsync<ObjectT>(
            ObjectT seed, IDictionary<string, object> data = null,
            string subPath = null, bool doCache = false)
            where ObjectT : ICloudObject, new()
        {
            Notifier?.BeginQuery();

            ResultSingle<ObjectT> result = await TrySend<ObjectT, ResultSingle<ObjectT>, ObjectT>(
                new HttpRequest<ObjectT, ResultSingle<ObjectT>>(DeleteAsync),
                seed, true, data, subPath, doCache);

            Notifier?.EndQuery();

            return result;
        }


        #region Utility

        private void UpdatePermit(ResultSingle<Permit<UserT>> result)
        {
            if (result.Code == 0)
            {
                result.Data.Payload.CopyTo(Permit);
                Permit.UseAccessToken();
            }
        }

        private void SetTransportError<ObjectT>(
            Result<ObjectT> result, Exception ex)
        {
            Debug.Write(ex);

            // Set default fallback error           
            result.Code = ServerStatus.NEST_RESULT_ERROR_TRANSPORT;
            result.Text = "NEST_RESULT_ERROR_TRANSPORT";

            if (ex is FlurlHttpException)
            {
                FlurlHttpException httpEx = ex as FlurlHttpException;

                if (httpEx.Call.Response != null)
                {
                    result.HttpStatus = httpEx.Call.Response.StatusCode;
                    result.Notes += "Http error " + result.HttpStatus.ToString() + "\n";

                    switch (result.HttpStatus)
                    {
                        case System.Net.HttpStatusCode.BadRequest:
                            result.Code = ServerStatus.NEST_RESULT_ERROR_TRANSPORT_BAD_REQUEST;
                            result.Text = "NEST_RESULT_ERROR_TRANSPORT_BAD_REQUEST";
                            break;
                        case System.Net.HttpStatusCode.Unauthorized:
                            {
                                if (httpEx.Call.Response.Headers.Contains("Token-Expired"))
                                {
                                    result.Code = ServerStatus.NEST_RESULT_ERROR_TRANSPORT_TOKEN_EXPIRED;
                                    result.Text = "NEST_RESULT_ERROR_TRANSPORT_TOKEN_EXPIRED";
                                }
                                else
                                {
                                    result.Code = ServerStatus.NEST_RESULT_ERROR_TRANSPORT_BAD_REQUEST;
                                    result.Text = "NEST_RESULT_ERROR_TRANSPORT_UNAUTHORIZED";
                                }
                            }
                            break;
                        case System.Net.HttpStatusCode.Forbidden:
                            result.Code = ServerStatus.NEST_RESULT_ERROR_TRANSPORT_FORBIDDEN;
                            result.Text = "NEST_RESULT_ERROR_TRANSPORT_FORBIDDEN";
                            break;
                        default:
                            break;
                    }

                    var inner = httpEx.InnerException;
                    if (inner != null)
                    {
                        result.Notes += $"Reason A : {inner.Message}";
                        if (inner.InnerException != null)
                        {
                            result.Notes += $"Reason B : {inner.InnerException.Message}";
                        }
                    }
                }
            }
            else
            {
                result.Notes = ex.Message;
                result.HttpStatus = System.Net.HttpStatusCode.Unused;
            }
        }

        private IFlurlRequest CreateRequest<ObjectT>(ObjectT seed, bool keyRequest,
            IDictionary<string, object> data = null, string subPath = null)
                where ObjectT : ICloudObject, new()
        {
            string fullUrl = Address + ApiEndpoint;

            if (keyRequest)
                fullUrl += seed.CollectionKey;
            else
                fullUrl += seed.CollectionPath;

            if (subPath != null)
                fullUrl = fullUrl + subPath;

            IFlurlRequest request = fullUrl.SetQueryParams(data)
                .WithHeader("x-device-signature", DeviceSignature)
                .WithHeader("x-api-version", string.Format("{0}.0", Version))
                .WithHeader("Accept", "application/json")
                .WithHeader("Accept", string.Format("application/vnd.nest.v{0}+json", Version));

            if (Permit.IsValid)
                request.WithOAuthBearerToken(Permit.ActiveToken);

            if (BasicAuth.Enabled)
                request.WithBasicAuth(BasicAuth.Username, BasicAuth.Password);

            return request;
        }

        private async Task<ResultSingle<ObjectT>> PostAsync<ObjectT>(ObjectT seed, IFlurlRequest flurlRequest)
            where ObjectT : ICloudObject, new()
        {
            string json = await flurlRequest.SendJsonAsync(
                HttpMethod.Post, seed)
                .ReceiveString();

            return ResultSingle<ObjectT>.ConvertObject(json, seed);
        }

        private async Task<ResultSingle<ObjectT>> GetAsync<ObjectT>(ObjectT seed, IFlurlRequest flurlRequest)
            where ObjectT : ICloudObject, new()
        {
            string json = await flurlRequest.GetStringAsync();

            return ResultSingle<ObjectT>.ConvertObject(json, seed);
        }

        private async Task<ResultMultiple<ObjectT>> GetListAsync<ObjectT>(ObjectT seed, IFlurlRequest flurlRequest)
            where ObjectT : ICloudObject, new()
        {
            string json = await flurlRequest.GetStringAsync();

            return ResultMultiple<ObjectT>.ConvertObject(json, seed);
        }

        private async Task<ResultSingle<ObjectT>> PutAsync<ObjectT>(ObjectT seed, IFlurlRequest flurlRequest)
            where ObjectT : ICloudObject, new()
        {
            string json = await flurlRequest.SendJsonAsync(
                HttpMethod.Put, seed)
                .ReceiveString();

            return ResultSingle<ObjectT>.ConvertObject(json, seed);
        }

        private async Task<ResultSingle<ObjectT>> DeleteAsync<ObjectT>(ObjectT seed, IFlurlRequest flurlRequest)
            where ObjectT : ICloudObject, new()
        {
            string json = await flurlRequest.SendJsonAsync(
                HttpMethod.Delete, seed)
                .ReceiveString();

            return ResultSingle<ObjectT>.ConvertObject(json, seed);
        }

        private async Task<ResultT> TrySend<PayloadT, ResultT, ResultReturnT>(
            HttpRequest<PayloadT, ResultT> request,
            PayloadT seed, bool keyRequest, IDictionary<string, object> data,
            string subPath = null, bool doCache = true)
                where PayloadT : ICloudObject, new()
                where ResultT : Result<ResultReturnT>, new()
        {
            // Try-send is used to send after a session has been established
            // it attaches the JWT token, attempts retry if failed and also 
            // handle cacheing

            ResultT result = new ResultT();

            for (int attempt = 0; attempt < RetryCount; attempt++)
            {
                result = await Send<PayloadT, ResultT, ResultReturnT>(
                        request, seed, keyRequest, data, subPath, doCache);

                switch (result.Code)
                {
                    // Manage transport issues at this level
                    case ServerStatus.NEST_RESULT_ERROR_TRANSPORT_BAD_REQUEST:
                    case ServerStatus.NEST_RESULT_ERROR_TRANSPORT_UNAUTHORIZED:
                    case ServerStatus.NEST_RESULT_ERROR_TRANSPORT_FORBIDDEN:
                        return result;
                    case ServerStatus.NEST_RESULT_ERROR_TRANSPORT_TOKEN_EXPIRED:
                        if (Permit != null && AutoTokenRenew)
                        {
                            // The token has expired. Renew is auto-renew option
                            // has been requested.
                            Permit.UseRefreshToken();

                            ResultSingle<Permit<UserT>> renewResult = await GetAsync(Permit,
                                CreateRequest(Permit, true, data));

                            if (renewResult.Code < 0)
                                return result;

                            UpdatePermit(renewResult);
                        }
                        else
                        {
                            return result;
                        }
                        break;
                    case ServerStatus.NEST_RESULT_ERROR_TRANSPORT:
                        {
                            if (attempt < RetryCount)
                            {
                                if (Notifier != null && !Notifier.CanProgress(attempt + 1))
                                {
                                    // Ask the notifier whether its okay to proceed
                                    return result;
                                }

                                // Wait period grows after each re-try. if interval is 2 seconds then 
                                // the each try will be in 2, 4, 8 second intervals etc. 
                                // (Exponential back-off)

                                int waitIntervalSecs = (int)Math.Pow(
                                    RetryBaseIntervalInSecs, attempt + 1);

                                Notifier?.Waiting(waitIntervalSecs);

                                Debug.Print("Re-attempt {0}, waiting for - {1} seconds ...",
                                    attempt + 1, waitIntervalSecs);

                                Task.Delay(waitIntervalSecs * 1000).Wait();
                            }
                        }
                        break;
                    default:
                        // Let the application handle higher errors
                        return result;
                }
            }

            return result;
        }

        private async Task<ResultT> Send<PayloadT, ResultT, ResultReturnT>(
            HttpRequest<PayloadT, ResultT> request,
            PayloadT seed, bool keyRequest, IDictionary<string, object> data,
            string subPath = null, bool doCache = true)
                where PayloadT : ICloudObject, new()
                where ResultT : Result<ResultReturnT>, new()
        {
            ResultT result = new ResultT();

            try
            {
                Debug.Print("Making a request to {0}",
                    seed.CollectionPath);

                result = await request(seed, CreateRequest(
                            seed, keyRequest, data, subPath));

                Debug.Print("The request to {0} resulted in code {1}",
                    seed.CollectionPath, result.Code);

                UpdateCache<PayloadT, ResultT, ResultReturnT>(seed, keyRequest, doCache, result);

                return result;

            }
            catch (Exception ex)
            {
                SetTransportError<ResultReturnT>(result, ex);
            }

            return result;
        }

        private void UpdateCache<PayloadT, ResultT, ResultReturnT>(
            PayloadT seed, bool keyRequest, bool doCache, Result<ResultReturnT> result)
                where PayloadT : ICloudObject, new()
                where ResultT : Result<ResultReturnT>, new()
        {
            if (result.Code == 0)
            {
                if (doCache)
                {
                    if (keyRequest)
                    {
                        _cache.Save(seed);
                    }
                    else
                    {
                        ObservableCollection<PayloadT> list = result.Data.Payload
                            as ObservableCollection<PayloadT>;

                        if (list != null)
                        {
                            list.All(obj =>
                            {
                                _cache.Save(obj);
                                return true;
                            });
                        }
                    }
                }
                else
                {
                    if (keyRequest)
                    {
                        _cache.Remove(seed);
                    }
                    else
                    {
                        ObservableCollection<PayloadT> list = result.Data.Payload
                            as ObservableCollection<PayloadT>;

                        if (list != null)
                        {
                            list.All(obj =>
                            {
                                _cache.Remove(obj);
                                return true;
                            });
                        }
                    }
                }
            }
        }

        #endregion
    }
}
