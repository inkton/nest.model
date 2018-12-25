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
    [Cloudname("app_backup")]

    public class AppBackup : CloudObject
    {
        private Int64 _id;
        private string _tag;
        private string _timeCreated;
        private string _description;
        private Int64? _deploymentSize;
        private string _status;

        public AppBackup()
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

        [JsonProperty("time_created")]
        public string TimeCreated
        {
            get { return _timeCreated; }
            set
            {
                SetProperty(ref _timeCreated, value);
            }
        }

        [JsonProperty("description")]
        public string Description
        {
            get { return _description; }
            set
            {
                SetProperty(ref _description, value);
            }
        }

        [JsonProperty("deployment_size")]
        public Int64? DeploymentSize
        {
            get { return _deploymentSize; }
            set
            {
                SetProperty(ref _deploymentSize, value);
            }
        }

        public string Status
        {
            get { return _status; }
            set
            {
                SetProperty(ref _status, value);
            }
        }
    }
}

