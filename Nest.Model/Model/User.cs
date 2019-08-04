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
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Inkton.Nest.Cloud;

namespace Inkton.Nest.Model
{
    [Cloudname("user")]
    public class User : IdentityUser<int>,  
        ICloudObject, INotifyPropertyChanged
    {
        private string _territoryISOCode;
        private string _firstName;
        private string _lastName;
        private bool _active;
        private double _creditBalance;

        private CloudObjectHelper _helper;

        public User()
        {
            _helper = new CloudObjectHelper(this);
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _helper.PropertyChanged += value; }
            remove { _helper.PropertyChanged -= value; }
        }

        [NotMapped]
        [JsonIgnore]
        public ICloudObject OwnedBy
        {
            get { return _helper.OwnedBy; }
            set { _helper.OwnedBy = value; }
        }

        [NotMapped]
        [JsonIgnore]
        virtual public string CollectionPath
        {
            get { return _helper.CollectionPath; }
        }

        [NotMapped]
        [JsonIgnore]
        virtual public string CollectionKey
        {
            get { return _helper.CollectionKey; }
        }

        [NotMapped]
        [JsonIgnore]
        public virtual string CloudKey
        {
            get { return Id.ToString(); }
        }

        public CloudnameAttribute GetCloudname()
        {
            return _helper.GetCloudname();
        }

        public string GetObjectName()
        {
            return _helper.GetObjectName();
        }

        public string GetCollectionName()
        {
            return _helper.GetCollectionName();
        }

        protected virtual bool SetProperty<T>(ref T storage, T value,
            string propertyName = null)
        {
            return _helper.SetProperty(ref storage, value, propertyName);
        }

        protected void OnPropertyChanged(
             string propertyName = null)
        {
            _helper.OnPropertyChanged(propertyName);
        }

        public void CopyTo(ICloudObject otherObject)
        {
            _helper.CopyTo(otherObject);
        }

        [JsonProperty("id")]
        public override int Id
        {
            get { return base.Id; }
            set {
                base.Id = value;
                OnPropertyChanged("Id");
            }
        }

        [JsonProperty("email")]
        public override string Email
        {
            get { return base.Email; }
            set {
                base.Email = value;
                OnPropertyChanged("Email");
            }
        }

        [JsonProperty("username")]
        public override string UserName
        {
            get { return base.UserName; }
            set {
                base.UserName = value;
                OnPropertyChanged("UserName");
            }
        }

        [JsonProperty("phonenumber")]
        public override string PhoneNumber
        {
            get { return base.PhoneNumber; }
            set {
                base.PhoneNumber = value;
                OnPropertyChanged("PhoneNumber");
            }
        }

        [JsonProperty("two_factor_enabled")]
        public override bool TwoFactorEnabled
        {
            get { return base.TwoFactorEnabled; }
            set {
                base.TwoFactorEnabled = value;
                OnPropertyChanged("TwoFactorEnabled");
            }
        }
        
        [JsonProperty("is_activated")]
        public bool IsActive
        {
            get { return _active; }
            set { SetProperty(ref _active, value); }
        }

        [NotMapped]
        [JsonProperty("nickname")]
        public string Nickname
        {
            get { return base.UserName; }
            set {
                base.UserName = value;
                OnPropertyChanged("UserName");
            }
        }

        [JsonProperty("territory_iso_code")]
        public string TerritoryISOCode
        {
            get { return _territoryISOCode; }
            set { SetProperty(ref _territoryISOCode, value); }
        }

        [JsonProperty("first_name")]
        public string FirstName
        {
            get { return _firstName; }
            set { SetProperty(ref _firstName, value); }
        }

        [JsonProperty("surname")]
        public string LastName
        {
            get { return _lastName; }
            set { SetProperty(ref _lastName, value); }
        }

        [JsonProperty("credits_balance")]
        public double CreditsBalance
        {
            get { return _creditBalance; }
            set { SetProperty(ref _creditBalance, value); }
        }

        
    }
}

