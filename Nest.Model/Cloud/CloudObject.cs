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
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Humanizer;

namespace Inkton.Nest.Cloud
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class CloudNameAttribute : Attribute
    {
        private string _objectName;
        private string _collectionName;

        public CloudNameAttribute(string objectName, string collectionName = null)
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

    [NotMapped]
    public abstract class CloudObject : INotifyPropertyChanged
    {
        private CloudObject _ownedBy;
        public event PropertyChangedEventHandler PropertyChanged;
        
        public CloudObject()
        {
        }

        [JsonIgnore]
        public CloudObject OwnedBy
        {
            get { return _ownedBy; }
            set { _ownedBy = value; }
        }

        [JsonIgnore]
        virtual public string CollectionPath
        {
            get {
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

        [JsonIgnore]
        virtual public string CollectionKey
        {
            get { return CollectionPath + CloudKey + "/"; }
        }

        [JsonIgnore]
        public virtual string CloudKey
        {
            get;
        }

        public CloudNameAttribute GetCloudName()
        {
            MemberInfo memberInfo = GetType();
            return memberInfo.GetCustomAttributes(true).Where(
                attr => attr.GetType() == typeof(CloudNameAttribute))
                    .FirstOrDefault() as CloudNameAttribute;
        }

        public string GetObjectName()
        {
            CloudNameAttribute cloudName = GetCloudName();

            if (cloudName != null)
            {
                return cloudName.ObjectName;
            }
            else
            {
                return GetType().Name;
            }
        }

        public string GetCollectionName()
        {
            CloudNameAttribute cloudName = GetCloudName();

            if (cloudName != null)
            {
                return cloudName.CollectionName;
            }
            else
            {
                return GetType().Name.Pluralize();
            }
        }

        protected virtual bool SetProperty<T>(ref T storage, T value,
                                        [CallerMemberName] string propertyName = null)
        {
            if (Object.Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void CopyTo(CloudObject otherObject)
        {
            var sourceProps = otherObject.GetType().GetRuntimeProperties()
                             .Where(x => x.CanWrite).ToList();
            var destProps = otherObject.GetType().GetRuntimeProperties()
                   .Where(x => x.CanWrite).ToList();

            foreach (var sourceProp in sourceProps)
            {
                var value = sourceProp.GetValue(this, null);
                var p = destProps.FirstOrDefault(x => x.Name == sourceProp.Name);

                if (p != null)
                {
                    if (value == null)
                    {
                        p.SetValue(otherObject, null, null);
                    }
                    else
                    {
                        p.SetValue(otherObject, value, null);
                    }
                }
            }
        }
    }
}
