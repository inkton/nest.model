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

using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Inkton.Nest.Cloud;

namespace Inkton.Nest.Model
{
    [Cloudname("role")]
    public class Role : IdentityRole<int>,
        ICloudObject, INotifyPropertyChanged
    {
        private string _tag;
        private CloudObjectHelper _helper;

        public Role()
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

        [JsonProperty("tag")]
        public string Tag
        {
            get { return _tag; }
            set { SetProperty(ref _tag, value); }
        }
    }
}

