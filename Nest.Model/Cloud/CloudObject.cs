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
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Linq;
using Newtonsoft.Json;
using Humanizer;

namespace Inkton.Nest.Cloud
{
    public interface ICloudObject
    {
        [JsonIgnore]
        string CloudKey { get; }

        [JsonIgnore]
        ICloudObject OwnedBy { get; set; }

        [JsonIgnore]
        string CollectionPath { get; }

        [JsonIgnore]
        string CollectionKey { get; }

        string GetObjectName();

        string GetCollectionName();

        void CopyTo(ICloudObject otherObject);
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class CloudnameAttribute : Attribute
    {
        private string _objectName;
        private string _collectionName;

        public CloudnameAttribute(string objectName, string collectionName = null)
        {
            _objectName = objectName;

            if (collectionName != null)
            {
                _collectionName = collectionName;
            }
            else
            {
                _collectionName = _objectName.Pluralize();
            }
        }

        public virtual string ObjectName
        {
            get { return _objectName; }
        }

        public virtual string CollectionName
        {
            get { return _collectionName; }
        }
    }

    public class CloudObjectHelper
    {
        private ICloudObject _helpee;
        private ICloudObject _ownedBy;
        public event PropertyChangedEventHandler PropertyChanged;

        public CloudObjectHelper(ICloudObject helpee)
        {
            _helpee = helpee;
        }

        public ICloudObject OwnedBy
        {
            get { return _ownedBy; }
            set { _ownedBy = value; }
        }

        public string CollectionPath
        {
            get
            {
                if (_ownedBy != null)
                {
                    return _ownedBy.CollectionKey + GetCollectionName() + "/";
                }
                else
                {
                    return GetCollectionName() + "/";
                }
            }
        }

        public string CollectionKey
        {
            get { return CollectionPath + _helpee.CloudKey + "/"; }
        }

        public CloudnameAttribute GetCloudname()
        {
            MemberInfo memberInfo = _helpee.GetType();
            return memberInfo.GetCustomAttributes(true)
                .FirstOrDefault(attr => attr.GetType()
                    == typeof(CloudnameAttribute)) as CloudnameAttribute;
        }

        public string GetObjectName()
        {
            CloudnameAttribute cloudname = GetCloudname();

            if (cloudname != null)
            {
                return cloudname.ObjectName;
            }
            else
            {
                return _helpee.GetType().Name;
            }
        }

        public string GetCollectionName()
        {
            CloudnameAttribute cloudname = GetCloudname();

            if (cloudname != null)
            {
                return cloudname.CollectionName;
            }
            else
            {
                return _helpee.GetType().Name.Pluralize();
            }
        }

        public virtual bool SetProperty<T>(ref T storage, T value,
                                        [CallerMemberName] string propertyName = null)
        {
            if (Object.Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void CopyTo(ICloudObject otherObject)
        {
            var sourceProps = _helpee.GetType().GetRuntimeProperties()
                             .Where(x => x.CanWrite).ToList();
            var destProps = otherObject.GetType().GetRuntimeProperties()
                   .Where(x => x.CanWrite).ToList();

            foreach (var sourceProp in sourceProps)
            {
                var destProp = destProps.FirstOrDefault(
                        prop => (prop.Name == sourceProp.Name &&
                            prop.GetType() == sourceProp.GetType()));

                if (destProp != null)
                {
                    destProp.SetValue(otherObject, sourceProp.GetValue(this, null), null);
                }
            }
        }
    }

    [NotMapped]
    public abstract class CloudObject : ICloudObject, INotifyPropertyChanged
    {
        private CloudObjectHelper _helper;

        public CloudObject()
        {
            _helper = new CloudObjectHelper(this);
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add => _helper.PropertyChanged += value;
            remove => _helper.PropertyChanged -= value;
        }

        [JsonIgnore]
        public ICloudObject OwnedBy
        {
            get => _helper.OwnedBy;
            set => _helper.OwnedBy = value;
        }

        [JsonIgnore]
        virtual public string CollectionPath
        {
            get => _helper.CollectionPath;
        }

        [JsonIgnore]
        virtual public string CollectionKey
        {
            get => _helper.CollectionKey;
        }

        [JsonIgnore]
        public virtual string CloudKey
        {
            get;
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
                                        [CallerMemberName] string propertyName = null)
        {
           return _helper.SetProperty(ref storage, value, propertyName);
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            _helper.OnPropertyChanged(propertyName);
        }

        public void CopyTo(ICloudObject otherObject)
        {
            _helper.CopyTo(otherObject);
        }
    }
}
