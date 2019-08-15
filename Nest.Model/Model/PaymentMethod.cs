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
    [Cloudname("charge_card_token")]
    public class ChargeCardToken : CloudObject
    {
        private int _id;
        private int _paymentMethodId;
        private string _brand;
        private string _country;
        private long _last4;
        private long _expMonth;
        private long _expYear;
        private string _hash;

        [JsonProperty("id")]
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        [JsonProperty("payment_method_id")]
        public int PaymentMethodId
        {
            get { return _paymentMethodId; }
            set { SetProperty(ref _paymentMethodId, value); }
        }

        [JsonProperty("brand")]
        public string Brand
        {
            get { return _brand; }
            set { SetProperty(ref _brand, value); }
        }
        [JsonProperty("country")]
        public string Country
        {
            get { return _country; }
            set { SetProperty(ref _country, value); }
        }

        [JsonProperty("last4")]
        public long Last4
        {
            get { return _last4; }
            set { SetProperty(ref _last4, value); }
        }

        [JsonProperty("exp_month")]
        public long ExpMonth
        {
            get { return _expMonth; }
            set { SetProperty(ref _expMonth, value); }
        }

        [JsonProperty("exp_year")]
        public long ExpYear
        {
            get { return _expYear; }
            set { SetProperty(ref _expYear, value); }
        }

        [JsonIgnore]
        public string Hash
        {
            get { return _hash; }
            set { SetProperty(ref _hash, value); }
        }
    }

    [Cloudname("payment_method")]
    public class PaymentMethod : CloudObject
    {
        private int _id;
        private string _type;
        private int _userId;
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
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        [JsonProperty("tag")]
        public string Tag
        {
            get { return _tag; }
            set { SetProperty(ref _tag, value); }
        }

        [JsonProperty("user_id")]
        public int UserId
        {
            get { return _userId; }
            set { SetProperty(ref _userId, value); }
        }

        [JsonProperty("type")]
        public string Type
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
