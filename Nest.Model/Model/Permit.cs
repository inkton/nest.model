/*
    Copyright (c) 2017 Inkton.

    Permission is hereby granted, free of charge, to any person obtaining
    a copy of this software and associated documentation files (the "Software"),
    to deal in the Software without restriction, including without limitation
    the rights to use, copy, modify, merge, publish, distribute, sublicense,
    and/or sell copies of the Software, and to permit persons to whom the Software
    is furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in
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
using Newtonsoft.Json;
using Inkton.Nest.Cloud;

namespace Inkton.Nest.Model
{
    [Cloudname("permit")]
    public class Permit<T> : CloudObject where T : User, new()
    {
        private T _user;
        private string _refreshToken;
        private string _accessToken;
        private string _activeToken;

        public Permit()
        {
            _user = new T();
        }

        public bool IsValid
        {
            get 
            { 
                return (_user != null &&
                    !string.IsNullOrEmpty(_activeToken)); 
            }
        }

        public override string CloudKey
        {
            get { return _user.Email; }
        }

        [JsonProperty("user")]
        public T User
        {
            get { return _user; }
            set { SetProperty(ref _user, value); }
        }

        [JsonProperty("refresh_token")]
        public string RefreshToken
        {
            get { return _refreshToken; }
            set { SetProperty(ref _refreshToken, value); }
        }

        [JsonProperty("access_token")]
        public string AccessToken
        {
            get { return _accessToken; }
            set { SetProperty(ref _accessToken, value); }
        }

        [JsonIgnore]
        public string ActiveToken
        {
            get { return _activeToken; }
            set { SetProperty(ref _activeToken, value); }
        }

        public void UseRefreshToken()
        {
            _activeToken = _refreshToken;
        }

        public void UseAccessToken()
        {
            _activeToken = _accessToken;
        }

        public void Reset()
        {
            _user = null;
            _refreshToken = string.Empty;
            _accessToken = string.Empty;
            _activeToken = string.Empty;
        }
    }
}

