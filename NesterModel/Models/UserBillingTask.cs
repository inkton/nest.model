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
using Newtonsoft.Json.Serialization;
using System.Text.RegularExpressions;

namespace Inkton.Nester.Models
{
    public class UserBillingTask : Cloud.ManagedEntity
    {
        private Int64 _id;
        private string _time;
        private Int64 _billingCycleId;
        private string _activity;
        private double _amount;
        private double _balance;

        private User _owner = null;

        public UserBillingTask()
             : base("user_billing_task")
        {
        }

        public User Owner
        {
            get { return _owner; }
            set { SetProperty(ref _owner, value); }
        }

        public override string Key
        {
            get { return _id.ToString(); }
        }

        override public string Collection
        {
            get
            {
                if (_owner != null && _owner.Id > 0)
                {
                    return _owner.CollectionKey + base.Collection;
                }
                else
                {
                    return base.Collection;
                }
            }
        }

        override public string CollectionKey
        {
            get
            {
                if (_owner != null && _owner.Id > 0)
                {
                    return _owner.CollectionKey + base.CollectionKey;
                }
                else
                {
                    return base.CollectionKey;
                }
            }
        }

        [JsonProperty("id")]
        public Int64 Id
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
        public Int64 BillingCycleId
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

