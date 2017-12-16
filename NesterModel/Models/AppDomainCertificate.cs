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
using Newtonsoft.Json.Serialization;

namespace Inkton.Nester.Models
{
    public class AppDomainCertificate : Cloud.ManagedEntity
    {
        private Int64 _id;
        private string _tag;
        private string _privateKey;
        private string _certificateChain;
        private string _type;

        private AppDomain _appDomain = null;

        public AppDomainCertificate()
            : base("app_domain_certificate")
        {
        }

        public AppDomain AppDomain
        {
            get { return _appDomain; }
            set
            {
                _appDomain = value;
            }
        }

        public override string Key
        {
            get { return _id.ToString(); }
        }

        override public string Collection
        {
            get
            {
                if (_appDomain != null)
                {
                    return _appDomain.CollectionKey + base.Collection;
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
                if (_appDomain != null)
                {
                    return _appDomain.CollectionKey + base.CollectionKey;
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

        [JsonProperty("tag")]
        public string Tag
        {
            get { return _tag; }
            set
            {
                SetProperty(ref _tag, value);
            }
        }

        [JsonProperty("private_key")]
        public string PrivateKey
        {
            get { return _privateKey; }
            set
            {
                SetProperty(ref _privateKey, value);
            }
        }

        [JsonProperty("certificate_chain")]
        public string CertificateChain
        {
            get { return _certificateChain; }
            set
            {
                SetProperty(ref _certificateChain, value);
            }
        }

        [JsonProperty("type")]
        public string Type
        {
            get { return _type; }
            set
            {
                SetProperty(ref _type, value);
                OnPropertyChanged("Icon");
            }
        }

        public string Icon
        {
            get
            {
                if (_type == "custom")
                {
                    return "customdomaincert.png";
                }
                else
                {
                    return "freedomaincert.png";
                }
            }
        }
    }
}

