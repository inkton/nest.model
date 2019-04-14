using System;

namespace Inkton.Cloud
{
    /*
    public class CloudIdentityUser : IdentityUser<TKey, TUserClaim, TUserRole, TUserLogin> where TKey : IEquatable<TKey>
    {
        private CloudObject _ownedBy;
        public event PropertyChangedEventHandler PropertyChanged;

        public CloudIdentityUser()
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
            var sourceProps = GetType().GetRuntimeProperties()
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
    */
}
