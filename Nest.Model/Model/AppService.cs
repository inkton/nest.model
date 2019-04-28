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
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using Inkton.Nest.Cloud;

namespace Inkton.Nest.Model
{
    [Cloudname("app_service")]
    public class AppService : CloudObject
    {
        private Int64 _id;
        private string _tag;
        private string _name;
        private string _type;
        private string _rules;
        private string _featuresAll;
        private Int64? _port;
        private Deployment _deployment;
        private ObservableCollection<AppServiceTier> _tiers;

        public Deployment deployment
        {
            get { return _deployment; }
            set { SetProperty(ref _deployment, value); }
        }

        public override string CloudKey
        {
            get { return _tag; }
        }

        override public string CollectionPath
        {
            get
            {
                if (_deployment != null)
                {
                    return _deployment.CollectionKey + GetCollectionName() + "/";
                }

                return base.CollectionPath;
            }
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

        [JsonProperty("rules")]
        public string Rules
        {
            get { return _rules; }
            set { SetProperty(ref _rules, value); }
        }

        [JsonProperty("features_all")]
        public string FeaturesAll
        {
            get { return _featuresAll; }
            set { SetProperty(ref _featuresAll, value); }
        }

        [JsonProperty("port")]
        public Int64? Port
        {
            get { return _port; }
            set { SetProperty(ref _port, value); }
        }

        public ObservableCollection<AppServiceTier> Tiers
        {
            get { return _tiers; }
            set { SetProperty(ref _tiers, value); }
        }
    }
}
