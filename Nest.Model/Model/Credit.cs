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
    [Cloudname("credit")]
    public class Credit : CloudObject
    {
        private int _id;
        private string _code;
        private string _type;
        private string _value;
        private string _expiry;

        public override string CloudKey
        {
            get { return _code; }
        }

        [JsonProperty("id")]
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        [JsonProperty("code")]
        public string Code
		{
            get { return _code; }
            set { SetProperty(ref _code, value); }
        }

        [JsonProperty("type")]
        public string Type
        {
            get { return _type; }
            set { SetProperty(ref _type, value); }
        }

        [JsonProperty("value")]
        public string Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value); }
        }

        [JsonProperty("expiry")]
        public string Expiry
        {
            get { return _expiry; }
            set { SetProperty(ref _expiry, value); }
        }
    }
}

