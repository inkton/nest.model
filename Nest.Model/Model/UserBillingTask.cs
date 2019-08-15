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
    [Cloudname("user_billing_task")]
    public class UserBillingTask : CloudObject
    {
        private int _id;
        private string _time;
        private int _billingCycleId;
        private string _activity;
        private double _amount;
        private double _balance;

        public override string CloudKey
        {
            get { return _id.ToString(); }
        }

        override public string CollectionPath
        {
            get
            {
                if (OwnedBy != null && (OwnedBy as User).Id > 0)
                {
                    return OwnedBy.CollectionKey + GetCollectionName() + "/";
                }
                return GetCollectionName() + "/";
            }
        }

        [JsonProperty("id")]
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        [JsonProperty("time")]
        public string Time
        {
            get { return _time; }
            set { _time = value; }
        }

        [JsonProperty("activity")]
        public string Activity
        {
            get { return _activity; }
            set { _activity = value; }
        }

        [JsonProperty("billing_cycle_id")]
        public int BillingCycleId
        {
            get { return _billingCycleId; }
            set { _billingCycleId = value; }
        }

        [JsonProperty("amount")]
        public double Amount
        {
            get { return _amount; }
            set
            {
                SetProperty(ref _amount, value);
            }
        }

        [JsonProperty("balance")]
        public double Balance
        {
            get { return _balance; }
            set
            {
                SetProperty(ref _balance, value);
            }
        }
    }
}

