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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Dynamic;
using System.Collections;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Threading;

namespace Inkton.Nester.Cloud
{
    public static class Object
    {
        public static Task<StatusT> WaitAsync<StatusT>(Task<StatusT> task)
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

        public static void Serialize(object value, System.IO.Stream s)
        {
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(s))
            using (Newtonsoft.Json.JsonTextWriter jsonWriter = new Newtonsoft.Json.JsonTextWriter(writer))
            {
                Newtonsoft.Json.JsonSerializer ser = new Newtonsoft.Json.JsonSerializer();
                ser.Serialize(jsonWriter, value);
                jsonWriter.Flush();
            }
        }

        public static T Deserialize<T>(System.IO.Stream s)
        {
            using (System.IO.StreamReader reader = new System.IO.StreamReader(s))
            using (Newtonsoft.Json.JsonTextReader jsonReader = new Newtonsoft.Json.JsonTextReader(reader))
            {
                Newtonsoft.Json.JsonSerializer ser = new Newtonsoft.Json.JsonSerializer();
                return ser.Deserialize<T>(jsonReader);
            }
        }

        public static void CopyPropertiesTo<T, TU>(this T source, TU dest)
        {
            var sourceProps = source.GetType().GetRuntimeProperties()
                   .Where(x => x.CanWrite).ToList();
            var destProps = dest.GetType().GetRuntimeProperties()
                   .Where(x => x.CanWrite).ToList();

            foreach (var sourceProp in sourceProps)
            {
                var value = sourceProp.GetValue(source, null);
                var p = destProps.FirstOrDefault(x => x.Name == sourceProp.Name);
                
                if (p != null)
                {
                    if (value == null)
                    {
                        p.SetValue(dest, null, null);
                    }
                    else
                    {
                        p.SetValue(dest, value, null);
                    }
                }
            }
        }

        public static void FillBlanks<T, TU>(this T source, TU dest)
        {
            var sourceProps = source.GetType().GetRuntimeProperties()
                   .Where(x => x.CanWrite).ToList();
            var destProps = dest.GetType().GetRuntimeProperties()
                   .Where(x => x.CanWrite).ToList();

            foreach (var sourceProp in sourceProps)
            {
                var valueSrc = sourceProp.GetValue(source, null);

                if (valueSrc != null)
                {
                    var p = destProps.FirstOrDefault(x => x.Name == sourceProp.Name);
                    var valueDst = p.GetValue(dest, null);

                    if (valueDst == null)
                    {
                        p.SetValue(dest, valueSrc, null);
                    }
                }
            }
        }


        public static void PourPropertiesTo<T, TU>(this T source, TU dest)
        {
            /* 
             * This version is similar to copy but does not 
             * copy if the source is null.
             * 
             */
            var sourceProps = source.GetType().GetRuntimeProperties()
                   .Where(x => x.CanWrite).ToList();
            var destProps = dest.GetType().GetRuntimeProperties()
                   .Where(x => x.CanWrite).ToList();

            foreach (var sourceProp in sourceProps)
            {
                var value = sourceProp.GetValue(source, null);
                var p = destProps.FirstOrDefault(x => x.Name == sourceProp.Name);

                if (value != null)
                {
                    p.SetValue(dest, value, null);
                }
            }
        }

        public static void CopyJsonPropertiesTo<T, TU>(this T source, TU dest)
        {
            var sourceProps = source.GetType().GetRuntimeProperties()
                   .Where(x => x.CanWrite).ToList();
            var destProps = dest.GetType().GetRuntimeProperties()
                   .Where(x => x.CanWrite).ToList();

            foreach (var sourceProp in sourceProps)
            {
                if (destProps.Any(x => x.Name == sourceProp.Name))
                {
                    var p = destProps.First(x => x.Name == sourceProp.Name);
                    var customAtr = sourceProp.CustomAttributes.ToArray();
                    if (customAtr != null && customAtr.Length > 0)
                    {
                        if (customAtr.First().AttributeType.Name == "JsonPropertyAttribute")
                        {
                            p.SetValue(dest, sourceProp.GetValue(source, null), null);
                        } 
                    }
                }
            }
        }

        public static void CopyExpandoPropertiesTo<T>(ExpandoObject source, T destination)
        {
            var sourceProps = source.GetType().GetRuntimeProperties()
                   .Where(x => x.CanWrite).ToList();
            var destinationProps = destination.GetType().GetRuntimeProperties()
                   .Where(x => x.CanWrite).ToList();

            foreach (var destinationProp in destinationProps)
            {
                if (destinationProp.CustomAttributes.Any())
                {
                    foreach (var attribute in destinationProp.CustomAttributes)
                    {
                        if (attribute.AttributeType == typeof(JsonPropertyAttribute))
                        {
                            string propName = (string)destinationProp.CustomAttributes.ElementAt(0).ConstructorArguments.ElementAt(0).Value;

                            var sourceDict = source as IDictionary<string, object>;
                            object value = sourceDict[propName];
                            bool ignore = false;

                            foreach (var arg in attribute.NamedArguments)
                            {
                                if (arg.MemberName == "NullValueHandling")
                                {
                                    if (!sourceDict.ContainsKey(propName))
                                    {
                                        ignore = true;
                                    }
                                    break;
                                }
                            }

                            if (ignore)
                            {
                                continue;
                            }

                            SetValue(destination, destinationProp, value);
                        }
                    }
                }
            }
        }

        private static void SetValue<T>(T destination, PropertyInfo destinationProp, object value)
        {
            object setObj = destinationProp.GetValue(destination);

            if (setObj == null)
            {
                bool objSet = true;

                // process reference types
                if (destinationProp.PropertyType == typeof(string))
                {
                    destinationProp.SetValue(destination, value);
                }
                else if ((destinationProp.PropertyType == typeof(string[])))
                {
                    IList objList = value as IList;
                    string[] array = new string[objList.Count];

                    int index = 0;
                    foreach (object item in (value as IList))
                    {
                        array[index] = item as string;
                        ++index;
                    }

                    destinationProp.SetValue(destination, array, null);
                }
                else if ((destinationProp.PropertyType == typeof(ObservableCollection<ExpandoObject>)))
                {
                    IEnumerable<object> objEnum = value as IEnumerable<object>;

                    if (objEnum.Any())
                    {
                        IEnumerable<object> objSubEnum = objEnum.FirstOrDefault() as IEnumerable<object>;

                        if (objSubEnum.Any())
                        {
                            var data = new ObservableCollection<ExpandoObject>();
                            int index = 0, subIndex = 0;

                            foreach (IEnumerable<object> item in objEnum)
                            {
                                var subObject = new ExpandoObject() as IDictionary<string, object>;

                                foreach (object subItem in item)
                                {
                                    subObject.Add("Value" + subIndex.ToString(), subItem);
                                    ++subIndex;
                                }

                                data.Add(subObject as ExpandoObject);

                                ++index;
                                subIndex = 0;
                            }

                            destinationProp.SetValue(destination, data, null);
                        }
                    }
                }
                else if ((destinationProp.PropertyType == typeof(object[,])))
                {
                    IEnumerable<object> objEnum = value as IEnumerable<object>;

                    if (objEnum.Any())
                    {
                        IEnumerable<object> objSubEnum = objEnum.FirstOrDefault() as IEnumerable<object>;

                        if (objSubEnum.Any())
                        {
                            object[,] array = new object[objEnum.Count(), objSubEnum.Count()];
                            int index = 0, subIndex = 0;

                            foreach (IEnumerable<object> item in objEnum)
                            {
                                foreach (object subItem in item)
                                {
                                    array[index, subIndex] = subItem;
                                    ++subIndex;
                                }
                                ++index;
                                subIndex = 0;
                            }

                            destinationProp.SetValue(destination, array, null);
                        }
                    }
                }
                else
                {
                    objSet = false;
                }

                if (objSet)
                {
                    return;
                }
                else
                {
                    setObj = Activator.CreateInstance(
                        destinationProp.PropertyType);
                }
            }

            if (setObj is Cloud.ManagedEntity)
            {
                CopyExpandoPropertiesTo(value as ExpandoObject, setObj);
                destinationProp.SetValue(destination, setObj);
            }
            else if (destinationProp.PropertyType == typeof(decimal))
            {
                destinationProp.SetValue(destination, new Decimal((long)value));
            }
            else if (destinationProp.PropertyType == typeof(bool))
            {
                if (value is Int64)
                {
                    destinationProp.SetValue(destination, ((Int64)value) != 0);
                }
                else if (value is string)
                {
                    destinationProp.SetValue(destination, ((string)value) != "False");
                }
            }
            else
            {
                destinationProp.SetValue(destination, value);
            }
        }
    }
}
