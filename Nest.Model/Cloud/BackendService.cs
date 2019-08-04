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
        IDictionary<string, string> data, string subPath = null, bool doCache = true) where PayloadT : ICloudObject, new();

    public class BackendService<UserT> : IBackendService<UserT>
        where UserT : User, new()
    {
        private BasicAuth _basicAuth;
        private ObjectStore _cache;

        public BackendService()
        {
            _cache = new ObjectStore(System.IO.Path.GetTempPath() +
                 Guid.NewGuid() + "-cache");
            _cache.Clear();
            DeviceSignature = "-nest.yt-";
            Endpoint = "https://api.nest.yt/";
        }

        public int Version { get; set; } = 1;

        public string DeviceSignature { get; set; }

        public string Endpoint { get; set; }

        public BasicAuth BasicAuth
        {
            get { return _basicAuth; }
            set { _basicAuth = value; }
        }

        public Permit<UserT> Permit { get; set; } = new Permit<UserT>();

        public bool AutoTokenRenew { get; set; } = true;

        public int RetryCount { get; set; } = 3;

        public int RetryBaseIntervalInSecs { get; set; } = 2;

        public IBackendServiceNotify Notifier { get; set; }

        public async Task<ResultSingle<Permit<UserT>>> SignupAsync(
            Dictionary<string, string> data = null)
        {
            Notifier?.BeginQuery();

            ResultSingle<Permit<UserT>> result = await PostAsync(Permit,
                CreateRequest(Permit, false, data));

            UpdatePermit(result);

            Notifier?.EndQuery();

            return result;
        }

        public async Task<ResultSingle<Permit<UserT>>> SetupPermitAsync(
            Dictionary<string, string> data = null)
        {
            Notifier?.BeginQuery();

            ResultSingle<Permit<UserT>> result = await PutAsync(Permit, 
                CreateRequest(Permit, true, data));

            UpdatePermit(result);

            Notifier?.EndQuery();

            return result;
        }

        public async Task<ResultSingle<Permit<UserT>>> RenewAccessAsync(
            Dictionary<string, string> data = null)
        {
            Notifier?.BeginQuery();

            Permit.UseRefreshToken();

            ResultSingle<Permit<UserT>> result = await GetAsync(Permit,
                CreateRequest(Permit, true, data));

            UpdatePermit(result);

            Notifier?.EndQuery();

            return result;
        }

        public async Task<ResultSingle<Permit<UserT>>> RevokeAccessAsync(
            Dictionary<string, string> data = null)
        {
            return await TrySend<Permit<UserT>, ResultSingle<Permit<UserT>>, Permit<UserT>>(
                new HttpRequest<Permit<UserT>, ResultSingle<Permit<UserT>>>(DeleteAsync),
                Permit, true, data, null, false);
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

        private void SetFailedResult<ObjectT>(
            Result<ObjectT> result, Exception ex)
        {
            Debug.Write(ex);

            if (ex is FlurlHttpException)
            {
                FlurlHttpException httpEx = ex as FlurlHttpException;
                string notes = "Failed to connect with " + Endpoint + "\n";
                result.Text = "NEST_RESULT_HTTP_ERROR";

                if (httpEx.Call.Response != null)
                {
                    result.HttpStatus = httpEx.Call.Response.StatusCode;

                    switch (result.HttpStatus)
                    {
                        case System.Net.HttpStatusCode.BadRequest:
                            result.Text = "NEST_RESULT_HTTP_400"; break;
                        case System.Net.HttpStatusCode.Unauthorized:
                            result.Text = "NEST_RESULT_HTTP_401"; break;
                        case System.Net.HttpStatusCode.Forbidden:
                            result.Text = "NEST_RESULT_HTTP_403"; break;
                        default:
                            result.Text = "NEST_RESULT_HTTP_ERROR";
                            result.Notes += "Http error " + result.HttpStatus.ToString() + "\n";
                            break;
                    }

                    var inner = httpEx.InnerException;
                    if (inner != null)
                    {
                        notes += $"Reason A : {inner.Message}";
                        if (inner.InnerException != null)
                        {
                            notes += $"Reason B : {inner.InnerException.Message}";
                        }
                    }
                }

                result.Notes = notes;
            }
            else
            {
                result.Text = "NEST_RESULT_ERROR";
                result.Notes = ex.Message;
            }
        }

        private IFlurlRequest CreateRequest<ObjectT>(ObjectT seed, bool keyRequest,
            IDictionary<string, string> data = null, string subPath = null)
                where ObjectT : ICloudObject, new()
        {
            string fullUrl = Endpoint;

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

            if (_basicAuth.Enabled)
                request.WithBasicAuth(_basicAuth.Username, _basicAuth.Password);

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
            PayloadT seed, bool keyRequest, IDictionary<string, string> data,
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
                catch (FlurlHttpException ex)
                {
                    SetFailedResult<ResultReturnT>(result, ex);

                    if (result.HttpStatus == System.Net.HttpStatusCode.Forbidden)
                    {
                        /* 
                         * The user does not have access rights
                         */
                        return result;
                    }
                    else if (result.HttpStatus == System.Net.HttpStatusCode.Unauthorized)
                    {
                        /* 
                         * The token has expired. Renew is auto-renew option
                         * has been requested.
                         */
                        if (Permit != null && AutoTokenRenew)
                        {
                            if (ex.Call.Response.Headers.Contains("Token-Expired"))
                            {
                                Permit.UseRefreshToken();

                                ResultSingle<Permit<UserT>> renewResult = await GetAsync(Permit,
                                    CreateRequest(Permit, true, data));

                                if (renewResult.Code < 0)
                                    return result;

                                UpdatePermit(renewResult);
                            }
                        }
                        else
                        {
                            return result;
                        }
                    }
                }
                catch (Exception ex)
                {
                    SetFailedResult<ResultReturnT>(result, ex);
                }

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

        public async Task<ResultSingle<ObjectT>> CreateAsync<ObjectT>(
            ObjectT seed, IDictionary<string, string> data = null,
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
            ObjectT seed, IDictionary<string, string> data = null,
            string subPath = null, bool doCache = true)
            where ObjectT : ICloudObject, new()
        {
            ResultSingle<ObjectT> result;
            Notifier?.BeginQuery();

            if (doCache && _cache.Load<ObjectT>(seed))
            {
                result = new ResultSingle<ObjectT>(0);
                result.Data = new DataContainer<ObjectT>();
                result.Data.Payload = seed;
                Notifier?.EndQuery();
                return result;
            }

            result = await TrySend<ObjectT, ResultSingle<ObjectT>, ObjectT>(
                new HttpRequest<ObjectT, ResultSingle<ObjectT>>(GetAsync),
                seed, true, data, subPath, doCache);

            Notifier?.EndQuery();

            return result;
        }

        public async Task<ResultMultiple<ObjectT>> QueryAsyncListAsync<ObjectT>(
            ObjectT seed, IDictionary<string, string> data = null,
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
            ObjectT seed, IDictionary<string, string> data = null,
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
            ObjectT seed, IDictionary<string, string> data = null,
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
    }
}
