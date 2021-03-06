﻿/*
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
    [Cloudname("nest")]
    public class Nest : CloudObject
    {
        private int _id;
        private int _appId;
        private string _tag;
        private string _name;
        private int? _platformId;
        private string _status;
        private int _scale;
        private string _scaleSize;
        private NestPlatform _platform;

        public override string CloudKey
        {
            get { return _id.ToString(); }
        }

        [JsonProperty("id")]
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        [JsonProperty("app_id")]
        public int AppId
        {
            get { return _appId; }
            set { SetProperty(ref _appId, value); }
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

        [JsonProperty("name")]
        public string Name
        {
            get { return _name; }
            set
            {
                SetProperty(ref _name, value);
            }
        }

        [JsonProperty("platform")]
        public NestPlatform Platform
        {
            get { return _platform; }
            set
            {
                if (value != null)
                {
                    _platformId = value.Id;
                    OnPropertyChanged("Icon");
                }

                SetProperty(ref _platform, value);
            }
        }

        [JsonProperty("platform_id")]
        public int? PlatformId
        {
            get { return _platformId; }
            set
            {
                SetProperty(ref _platformId, value);
            }
        }

        [JsonProperty("status")]
        public string NestStatus
        {
            get { return _status; }
            set
            {
                SetProperty(ref _status, value);
            }
        }

        [JsonProperty("scale")]
        public int Scale
        {
            get { return _scale; }
            set
            {
                SetProperty(ref _scale, value);
            }
        }

        [JsonProperty("scale_size")]
        public string ScaleSize
        {
            get { return _scaleSize; }
            set
            {
                SetProperty(ref _scaleSize, value);
            }
        }

        public string Info
        {
            get {
                return _scaleSize + " x " + _scale.ToString();
            }
        }

        public string Icon
        {
            get
            {
                if (_platform == null)
                {
                    return string.Empty;
                }

                switch(_platform.Tag)
                {
                    case "mvc": return "webnet32.png";
                    case "api": return "websocketnet32.png";
                    case "worker": return "worker32.png";
                    default: System.Diagnostics.Debugger.Break(); return "";
                }
            }
        }
    }
}

