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

using System.Collections.Generic;
using System.Threading.Tasks;
using Inkton.Nest.Cloud;
using Inkton.Nest.Model;

namespace Inkton.Nest.Cloud
{
    public interface IBackendService<UserT> where UserT : User, new()
    {
        int Version { get; set; }
        string DeviceSignature { get; set; }
        string Endpoint { get; set; }
        BasicAuth BasicAuth { get; set; }
        Permit<UserT> Permit { get; set; }

        Task<ResultSingle<Permit<UserT>>> SignupAsync(
            Dictionary<string, object> data = null);
        Task<ResultSingle<Permit<UserT>>> SetupPermitAsync(
            Dictionary<string, object> data = null);
        Task<ResultSingle<Permit<UserT>>> RenewAccessAsync(
            Dictionary<string, object> data = null);
        Task<ResultSingle<Permit<UserT>>> RevokeAccessAsync(
            Dictionary<string, object> data = null);

        Task<ResultSingle<ObjectT>> CreateAsync<ObjectT>(
            ObjectT seed, IDictionary<string, object> data = null,
            string subPath = null, bool doCache = true)
            where ObjectT : ICloudObject, new();
        Task<ResultSingle<ObjectT>> QueryAsync<ObjectT>(
            ObjectT seed, IDictionary<string, object> data = null,
            string subPath = null, bool doCache = true)
            where ObjectT : ICloudObject, new();
        Task<ResultMultiple<ObjectT>> QueryAsyncListAsync<ObjectT>(
            ObjectT seed, IDictionary<string, object> data = null,
            string subPath = null, bool doCache = true)
            where ObjectT : ICloudObject, new();
        Task<ResultSingle<ObjectT>> UpdateAsync<ObjectT>(
            ObjectT seed, IDictionary<string, object> data = null,
            string subPath = null, bool doCache = true)
            where ObjectT : ICloudObject, new();
        Task<ResultSingle<ObjectT>> RemoveAsync<ObjectT>(
            ObjectT seed, IDictionary<string, object> data = null,
            string subPath = null, bool doCache = false)
            where ObjectT : ICloudObject, new();
    }
}
