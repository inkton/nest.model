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
    [Cloudname("app_service_tier")]
    public class AppServiceTier : CloudObject
    {
        private Int64 _id;
        private Int64 _appServiceId;
        private string _tag;
        private string _name;
        private string _type;
        private string _period;
        private decimal _itemCost;
        private string _featuresIncluded;

        public AppServiceTier() 
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

        [JsonProperty("app_service_id")]
        public Int64 AppServiceId
        {
            get { return _appServiceId; }
            set { _appServiceId = value; }
        }

        [JsonProperty("tag")]
        public string Tag
        {
            get { return _tag; }
            set { SetProperty(ref _tag, value); }
        }

        [JsonProperty("name")]
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        [JsonProperty("type")]
        public string Type
        {
            get { return _type; }
            set { SetProperty(ref _type, value); }
        }

        [JsonProperty("period")]
        public string Period
        {
            get { return _period; }
            set { SetProperty(ref _period, value); }
        }

        [JsonProperty("item_cost")]
        public decimal ItemCost
        {
            get { return _itemCost; }
            set { SetProperty(ref _itemCost, value); }
        }

        [JsonProperty("features_included")]
        public string FeaturesIncluded
        {
            get { return _featuresIncluded; }
            set { SetProperty(ref _featuresIncluded, value); }
        }
    }
}

