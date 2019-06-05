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
    [Cloudname("token")]
    public class ChargeCardToken : CloudObject
    {
        [JsonProperty("brand")]
        public string Brand
        {
            get;
            set;
        }
        [JsonProperty("country")]
        public string Country
        {
            get;
            set;
        }

        [JsonProperty("last4")]
        public Int64 Last4
        {
            get;
            set;
        }

        [JsonProperty("exp_month")]
        public Int64 ExpMonth
        {
            get;
            set;
        }

        [JsonProperty("exp_year")]
        public Int64 ExpYear
        {
            get;
            set;
        }
    }

    [Cloudname("payment_method")]
    public class PaymentMethod : CloudObject
    {
        private Int64 _id;
        private string _type;
        private string _tag;
        private string _name;
        private ChargeCardToken _proof;

        public PaymentMethod() 
        {
            _type = "cc";
            _tag = "stripe_cc";
        }

        public override string CloudKey
        {
            get { return _tag; }
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
            set { _tag = value; }
        }

        [JsonProperty("type")]
        public string type
        {
            get { return _type; }
            set { _type = value; }
        }

        [JsonProperty("name")]
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        [JsonProperty("token")]
        public ChargeCardToken Proof
        {
            get { return _proof; }
            set { SetProperty(ref _proof, value); }
        }

        public bool IsActive
        {
            get
            {
                return (_proof != null && _proof.Last4 > 0);
            }
        }
    }
}
