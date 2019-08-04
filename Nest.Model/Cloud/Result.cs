using System;
using System.Net;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Inkton.Nest.Cloud
{
    #region Produce a result

    [JsonObject]
    public class DataContainer<T>
    {
        public T Payload
        {
            get; set;
        }
    }

    public class EmptyPayload : CloudObject { }

    public class Result<T>
    {
        public Result()
        {
            Code = -1;
            Text = "Uknown Error";
        }

        [JsonProperty("notes")]
        public string Notes { get; set; }
        [JsonProperty("result_code")]
        public int Code { get; set; }
        [JsonProperty("result_text")]
        public string Text { get; set; }
        [JsonProperty("data")]
        public DataContainer<T> Data { get; set; }

        [JsonIgnore]
        public HttpStatusCode HttpStatus = HttpStatusCode.OK;

        public Result(int code = -999)
        {
            Code = code;
            Text = "unknown";
            Notes = "none";
        }

        public static Task<ResultT> WaitAsync<ResultT>(Task<ResultT> task)
        {
            // Ensure that awaits were called with .ConfigureAwait(false)

            var wait = new ManualResetEventSlim(false);

            var continuation = task.ContinueWith(_ =>
            {
                wait.Set();
                return _.Result;
            });

            wait.Wait();

            return continuation;
        }

        protected static void FillNullsFrom(ICloudObject thisObject, ICloudObject otherObject)
        {
            /* Fill otherObject null properties with 
             * this object object properties.
             */

            var sourceProps = otherObject.GetType().GetRuntimeProperties()
                             .Where(x => x.CanWrite).ToList();
            var destProps = otherObject.GetType().GetRuntimeProperties()
                   .Where(x => x.CanWrite).ToList();

            foreach (var sourceProp in sourceProps)
            {
                var value = sourceProp.GetValue(thisObject, null);
                var p = destProps.FirstOrDefault(x => x.Name == sourceProp.Name);

                if (p != null)
                {
                    var valueDst = p.GetValue(otherObject, null);

                    if (valueDst == null)
                    {
                        p.SetValue(otherObject, value, null);
                    }
                }
            }
        }
    }

    public static class ResultFactory
    {
        public static string Create(
            int code,
            string text = null,
            string notes = null
            )
        {
            Result<EmptyPayload> result = new Result<EmptyPayload>();
            result.Code = code;
            result.Text = text;
            result.Notes = notes;
            return Json(result);
        }

        public static string CreateSingle<T>(
            T data,
            int code = 0,
            string text = null,
            string notes = null
            ) where T : ICloudObject, new()
        {
            Result<T> result = new Result<T>();
            result.Code = code;
            result.Text = text;
            result.Notes = notes;
            result.Data = new DataContainer<T>();
            result.Data.Payload = data;
            return Json(result);
        }

        public static string CreateMultiple<T>(
            List<T> data,
            int code = 0,
            string text = null,
            string notes = null
            ) where T : ICloudObject, new()
        {
            Result<List<T>> result = new Result<List<T>>();
            result.Code = code;
            result.Text = text;
            result.Notes = notes;
            result.Data = new DataContainer<List<T>>();
            result.Data.Payload = data;
            return Json(result);
        }

        private static string Json<T>(Result<T> result)
            where T : ICloudObject, new()
        {
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new DataContainerResolver(new T().GetObjectName());

            return JsonConvert.SerializeObject(result, serializerSettings);
        }

        private static string Json<T>(Result<List<T>> result)
            where T : ICloudObject, new()
        {
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new DataContainerResolver(new T().GetCollectionName());

            return JsonConvert.SerializeObject(result, serializerSettings);
        }
    }

    #endregion

    #region Consume a result

    public class DataContainerResolver : DefaultContractResolver
    {
        private readonly string _name;

        public DataContainerResolver(string name)
        {
            _name = name;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (property.PropertyName == "Payload")
                property.PropertyName = _name;

            /* 
            The user class has security sensitive properties
            such as the password hash that should not be exposed
            outside of the server.  If a property from IdentityUser<int>
            is needed to be sent then override the derived class.
            */
            if(property.DeclaringType == typeof(IdentityUser<int>))
                property.ShouldSerialize = instance => { return false; };

            return property;
        }
    }

    public class SingleDataContainerConverter<T> : JsonConverter
        where T : ICloudObject, new()
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(DataContainer<T>).IsAssignableFrom(objectType);
        }

        protected DataContainer<T> Create(Type objectType, JObject jObject)
        {
            if (objectType.Name.StartsWith("DataContainer", StringComparison.CurrentCulture))
            {
                return new DataContainer<T>();
            }

            throw new Exception(String.Format("The given vehicle type {0} is not supported!", objectType.Name));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            // Load JObject from stream
            JObject jObject = JObject.Load(reader);

            DataContainer<T> target = Create(objectType, jObject);
            target.Payload = new T();

            if (jObject[target.Payload.GetObjectName()] != null)
            {
                target.Payload = jObject[target.Payload.GetObjectName()].ToObject<T>(serializer);
            }

            return target;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    public class MultipleDataContainerConverter<T> : JsonConverter
        where T : ICloudObject, new()
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(DataContainer<ObservableCollection<T>>).IsAssignableFrom(objectType);
        }

        protected DataContainer<ObservableCollection<T>> Create(Type objectType, JObject jObject)
        {
            if (objectType.Name.StartsWith("DataContainer", StringComparison.CurrentCulture))
            {
                return new DataContainer<ObservableCollection<T>>();
            }

            throw new Exception(String.Format("The given vehicle type {0} is not supported!", objectType.Name));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            // Load JObject from stream
            JObject jObject = JObject.Load(reader);

            DataContainer<ObservableCollection<T>> target = Create(objectType, jObject);
            T type = new T();
            string key = type.GetCollectionName();

            if (jObject[key] != null)
            {
                try
                {
                    target.Payload = jObject[key].ToObject<ObservableCollection<T>>();
                }
                catch (Exception)
                {
                    // empty object list throws. 
                    target.Payload = new ObservableCollection<T>();
                }
            }

            return target;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    public class ResultSingle<PayloadT> : Result<PayloadT>
        where PayloadT : ICloudObject, new()
    {
        public ResultSingle() { }

        public ResultSingle(int code) : base(code) { }

        public static ResultSingle<PayloadT> ConvertObject(string json, PayloadT seed)
        {
            ResultSingle<PayloadT> result = JsonConvert.DeserializeObject<ResultSingle<PayloadT>>(json,
                new SingleDataContainerConverter<PayloadT>());

            if (result.Code == 0 && result.Data != null)
            {
                /* make a complete object by setting null
                 * values in the received object by the seed
                 * object
                 */
                FillNullsFrom(seed, result.Data.Payload);
            }

            return result;
        }
    }

    public class ResultMultiple<PayloadT> : Result<ObservableCollection<PayloadT>>
        where PayloadT : ICloudObject, new()
    {
        public ResultMultiple() { }

        public ResultMultiple(int code) : base(code) { }

        public static ResultMultiple<PayloadT> ConvertObject(string json, PayloadT seed)
        {
            ResultMultiple<PayloadT> result =
                JsonConvert.DeserializeObject<ResultMultiple<PayloadT>>(json,
                    new MultipleDataContainerConverter<PayloadT>());

            if (result.Code == 0 && result.Data != null)
            {
                foreach (var item in result.Data.Payload)
                {
                    /* make a complete object by setting null
                     * values in the received object by the seed
                     * object
                     */
                    FillNullsFrom(seed, item);
                }
            }

            return result;
        }
    }

    #endregion
}