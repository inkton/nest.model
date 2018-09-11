using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Inkton.Nest.Cloud
{
    [JsonObject]
    public class DataContainer<T>
    {
        public T Payload
        {
            get; set;
        }
    }

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

            return property;
        }
    }

    public class EmptyPayload : CloudObject { }

    public class ResultBase<T>
    {
        public ResultBase()
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
    }

    public static class ResultFactory
    {
        public static string Create(
            int code, 
            string text = null, 
            string notes = null
            )
        {
            ResultBase<EmptyPayload> result = new ResultBase<EmptyPayload>();
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
            ) where T : CloudObject, new()
        {
            ResultBase<T> result = new ResultBase<T>();
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
            ) where T : CloudObject, new()
        {
            ResultBase<List<T>> result = new ResultBase<List<T>>();
            result.Code = 0;
            result.Text = text;
            result.Notes = notes;
            result.Data = new DataContainer<List<T>>();
            result.Data.Payload = data;
            return Json(result);
        }

        private static string Json<T>(ResultBase<T> result) 
            where T : CloudObject, new()
        {
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new DataContainerResolver(new T().GetObjectName());

            return JsonConvert.SerializeObject(result, serializerSettings);
        }

        private static string Json<T>(ResultBase<List<T>> result)
            where T : CloudObject, new()
        {
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new DataContainerResolver(new T().GetCollectionName());

            return JsonConvert.SerializeObject(result, serializerSettings);
        }
    }
}