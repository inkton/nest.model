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
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using Inkton.Nest.Cloud;

namespace Inkton.Nest.Model
{
    [Cloudname("app")]
    public class App : CloudObject
    {
        private int _id;
        private string _tag;
        private string _name;
        private string _type;
        private string _status;
        private string _ipAddress;
        private int _userId;
        private int? _treeId;
        private int? _primaryDomainId;
        private string _unifiedPassword, _networkPassword, _servicesPassword;
        private int? _serviceTierId;
        private int _backupHour;

        private Deployment _deployment;
        private OwnerCapabilities _ownerCapabilities;
        private ObservableCollection<AppServiceSubscription> _subscriptions;

        public App()
        {
            _status = "init";
            _backupHour = 0;
        }

        public string Hostname
        {
            get { return _tag + ".nestapp.yt"; }
        }

        public override string CloudKey
        {
            get { return _tag; }
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

        public OwnerCapabilities OwnerCapabilities
        {
            get { return _ownerCapabilities; }
            set { SetProperty(ref _ownerCapabilities, value); }
        }

        public ObservableCollection<AppServiceSubscription> Subscriptions
        {
            get { return _subscriptions; }
            set { SetProperty(ref _subscriptions, value); }
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

        [JsonProperty("status")]
        public string Status
        {
            get { return _status; }
            set
            {
                SetProperty(ref _status, value);
                OnPropertyChanged("CurrentActivity");
            }
        }

        [JsonProperty("ip_address")]
        public string IPAddress
        {
            get { return _ipAddress; }
            set
            {
                SetProperty(ref _ipAddress, value);
            }
        }

        [JsonProperty("backup_hour")]
        public int BackupHour
        {
            get { return _backupHour; }
            set
            {
                SetProperty(ref _backupHour, value);
            }
        }

        /// <summary>
        /// A Deployment exists - the deployment
        /// status can be anything
        /// </summary>
        public bool IsDeploymentValid
        {
            get
            {
                return (_deployment != null && _deployment.Id >= 0);
            }
        }

        public string CurrentActivity
        {
            get
            {
                if (IsDeploymentValid)
                {
                    switch(_deployment.Status)
                    {
                        case "updating": return "Updating";
                        case "failed": return "Failed";
                        default:
                            return char.ToUpper(_status[0]) +
                                _status.Substring(1);
                    }
                }
                return char.ToUpper(_status[0]) +
                    _status.Substring(1);
            }
        }

        /// <summary>
        /// A Deployment exists and not in progress 
        /// of being removed or created
        /// </summary>
        public bool IsDeployed
        {
            get
            {
                if (IsDeploymentValid)
                {
                    return (_deployment.Status == "complete" ||
                            _deployment.Status == "failed");
                }
                return false;
            }
        }

        /// <summary>
        /// A Deployment exists and it is beging upated
        /// </summary>
        public bool IsBusy
        {
            get
            {
                if (IsDeploymentValid)
                {
                    return (_deployment.Status == "updating");
                }
                return false;
            }
        }

        /// <summary>
        /// The app is active and online
        /// </summary>
        public bool IsActive
        {
            get
            {
                if (IsDeploymentValid)
                {
                    return (_deployment.Status == "complete" &&
                        _status == "deployed");
                }
                return false;
            }
        }

        [JsonProperty("primary_domain_id")]
        public int? PrimaryDomainId
        {
            get { return _primaryDomainId; }
            set { SetProperty(ref _primaryDomainId, value); }
        }

        [JsonProperty("network_password")]
        public string NetworkPassword
        {
            get { return _networkPassword; }
            set { SetProperty(ref _networkPassword, value); }
        }

        [JsonProperty("services_password")]
        public string ServicesPassword
        {
            get { return _servicesPassword; }
            set
            {
                SetProperty(ref _servicesPassword, value);

                // the default is the services 
                // password
                _unifiedPassword = _servicesPassword;
            }
        }

        [JsonIgnore]
        public string UnifiedPassword
        {
            get { return _unifiedPassword; }
            set
            {
                SetProperty(ref _unifiedPassword, value);
                _networkPassword = _unifiedPassword;
                _servicesPassword = _unifiedPassword;
            }
        }

        [JsonProperty("user_id")]
        public int UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }

        [JsonProperty("tree_id", NullValueHandling = NullValueHandling.Ignore)]
        public int? TreeId
        {
            get { return _treeId; }
            set { _treeId = value; }
        }

        [JsonProperty("app_service_tier_id", NullValueHandling = NullValueHandling.Ignore)]
        public int? ServiceTierId
        {
            get { return _serviceTierId; }
            set { SetProperty(ref _serviceTierId, value); }
        }

        [JsonIgnore]
        public Deployment Deployment
        {
            get { return _deployment; }
            set
            {
                SetProperty(ref _deployment, value);

                OnPropertyChanged("IsDeployed");
                OnPropertyChanged("IsBusy");
                OnPropertyChanged("IsActive");

                OnPropertyChanged("CurrentActivity");
            }
        }

        [JsonIgnore]
        public string Icon
        {
            get
            {
                if (_status == "deployed")
                {
                    return "homesteadactive32.png";
                }
                return "homesteadinactive32.png";
            }
        }
    }
}

