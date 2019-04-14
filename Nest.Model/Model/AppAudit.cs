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
    [Cloudname("app_audit")]
    public class AppAudit : CloudObject
    {
        private Int64 _id;
        private string _tag;
        private Int64 _epoch;
        private string _activity;
        private string _status;
        private string _user;
        private string _hostName;
        private string _hostAddress;

        private static readonly DateTime _epochTime = 
            new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public AppAudit()
        {
        }

        public override string CloudKey
        {
            get { return _id.ToString(); }
        }

        [JsonProperty("id")]
        public Int64 Id
        {
            get { return _id; }
            set { _id = value; }
        }

        [JsonProperty("tag")]
        public string Tag
        {
            get { return _tag; }
            set
            {
                SetProperty(ref _tag, value);
            }
        }

        [JsonProperty("activity")]
        public string Activity
        {
            get { return _activity; }
            set
            {
                SetProperty(ref _activity, value);
            }
        }

        [JsonProperty("time")]
        public string Time
        {
            get {
                string time = "";

                if (_epoch > 0)
                {
                    time = _epochTime.AddSeconds(_epoch).ToString("g");
                }

                return time;
            }
        }

        [JsonProperty("epoch")]
        public Int64 Epoch
        {
            get { return _epoch; }
            set
            {
                SetProperty(ref _epoch, value);
            }
        }

        [JsonProperty("status")]
        public string Status
        {
            get { return _status; }
            set
            {
                SetProperty(ref _status, value);
            }
        }

        [JsonProperty("user")]
        public string User
        {
            get { return _user; }
            set
            {
                SetProperty(ref _user, value);
            }
        }

        [JsonProperty("host_name")]
        public string HostName
        {
            get { return _hostName; }
            set
            {
                SetProperty(ref _hostName, value);
            }
        }

        [JsonProperty("host_address")]
        public string HostAddress
        {
            get { return _hostAddress; }
            set
            {
                SetProperty(ref _hostAddress, value);
            }
        }
    }
}

