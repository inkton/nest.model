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
using System;
using Newtonsoft.Json;
using Inkton.Nest.Cloud;

namespace Inkton.Nest.Model
{
    [Cloudname("permit")]
    public class Permit : Cloud.CloudObject
    {
        private string _token = "<-token->";
        private string _password = null;
        private string _securityCode = null;
        private User _user = new User();

        public Permit()
        {
        }

        public override string CloudKey
        {
            get { return _user.Email; }
        }

        [JsonProperty("token")]
        public string Token
        {
            get { return _token; }
            set { _token = value; }
        }

        [JsonProperty("password")]
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        [JsonProperty("security_code")]
        public string SecurityCode
        {
            get { return _securityCode; }
            set { _securityCode = value; }
        }

        [JsonProperty("owner")]
        public User Owner
        {
            get { return _user; }
            set { SetProperty(ref _user, value); }
        }

        public void Invalid()
        {
            _token = null;
            _password = null;
        }
    }
}

