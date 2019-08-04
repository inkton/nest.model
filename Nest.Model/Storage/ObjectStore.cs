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
using System.IO;
using Inkton.Nest.Cloud;
using Newtonsoft.Json;

namespace Inkton.Nest.Storage
{
    public class ObjectStore
    {
        private string _path;

        public ObjectStore(string path)
        {
            Path = path;
        }

        public string Path
        {
            set
            {
                _path = value;
                if (!Directory.Exists(_path))
                {
                    Directory.CreateDirectory(_path);
                }                
            }
            get
            {
                return _path;
            }
        }

        public void Clear()
        {
            try
            {
                if (Directory.Exists(Path))
                {
                    foreach (string filePath in 
                        Directory.GetFiles(Path, "*", SearchOption.AllDirectories))
                    {
                        File.Delete(filePath);
                    }

                    Directory.Delete(Path, true);
                }
            }
            catch (Exception e) 
            {
                System.Console.Write(e.Message);
            }
        }

        private string GetObjectPath(ICloudObject obj)
        {
            return Path + @"\" + obj.CollectionKey.TrimEnd('/') + ".json";
        }

        public void Save<T>(T obj) where T : ICloudObject
        {
            if (!Directory.Exists(Path + @"\" + obj.CollectionPath))
            {
                Directory.CreateDirectory(Path + @"\" + obj.CollectionPath);
            }

            string json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            File.WriteAllText(GetObjectPath(obj), json);
        }

        public bool Load<T>(T obj) where T : ICloudObject
        {
            string path = GetObjectPath(obj);
            if (!File.Exists(path))
            {
                return false;
            }

            string json = File.ReadAllText(path);
            T received = JsonConvert.DeserializeObject<T>(json);
            received.OwnedBy = obj.OwnedBy;
            received.CopyTo(obj);
            return true;
        }

        public void Remove<T>(T obj) where T : ICloudObject
        {
            if (File.Exists(GetObjectPath(obj)))
            {
                File.Delete(GetObjectPath(obj));
            }
        }
    }

}
