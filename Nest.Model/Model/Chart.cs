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
using Newtonsoft.Json;
using Inkton.Nest.Cloud;

namespace Inkton.Nest.Model
{
    [Cloudname("chart")]
    public class Chart : CloudObject
    {
        private string _id;
        private string _name;
        private string _type;
        private string _family;
        private string _context;
        private string _title;
        private int _priority;
        private bool _enabled;
        private string _units;
        private string _chartType;
        private int _duration;
        private int _firstEntry;
        private int _lastEntry;
        private int _updateEvery;
        private double _min, _max;
        private Points _result;

        public class Points : CloudObject
        {
            private string[] _labels;
            private object[,] _data;

            [JsonProperty("labels")]
            public string[] Labels
            {
                get { return _labels; }
                set { _labels = value; }
            }

            [JsonProperty("data")]
            public object[,] Data
            {
                get { return _data; }
                set { _data = value; }
            }
        }

        public override string CloudKey
        {
            get { return _id; }
        }

        override public string CollectionPath
        {
            get
            {
                string path = base.CollectionPath;

                if (_result != null)
                {
                    path += "/data";
                }

                return path;
            }
        }

        [JsonProperty("id")]
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        [JsonProperty("name")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        [JsonProperty("family", NullValueHandling = NullValueHandling.Ignore)]
        public string Family
        {
            get { return _family; }
            set { _family = value; }
        }

        [JsonProperty("context", NullValueHandling = NullValueHandling.Ignore)]
        public string Context
        {
            get { return _context; }
            set { _context = value; }
        }

        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        [JsonProperty("priority", NullValueHandling = NullValueHandling.Ignore)]
        public int Priority
        {
            get { return _priority; }
            set { _priority = value; }
        }

        [JsonProperty("enabled", NullValueHandling = NullValueHandling.Ignore)]
        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        [JsonProperty("units", NullValueHandling = NullValueHandling.Ignore)]
        public string Units
        {
            get { return _units; }
            set { _units = value; }
        }

        [JsonProperty("chart_type", NullValueHandling = NullValueHandling.Ignore)]
        public string ChartType
        {
            get { return _chartType; }
            set { _chartType = value; }
        }

        [JsonProperty("duration", NullValueHandling = NullValueHandling.Ignore)]
        public int Duration
        {
            get { return _duration; }
            set { _duration = value; }
        }

        [JsonProperty("first_entry")]
        public int FirstEntry
        {
            get { return _firstEntry; }
            set { _firstEntry = value; }
        }

        [JsonProperty("last_entry")]
        public int LastEntry
        {
            get { return _lastEntry; }
            set { _lastEntry = value; }
        }

        [JsonProperty("update_every")]
        public int UpdateEvery
        {
            get { return _updateEvery; }
            set { _updateEvery = value; }
        }

        [JsonProperty("min")]
        public double Min
        {
            get { return _min; }
            set { _min = value; }
        }

        [JsonProperty("max")]
        public double Max
        {
            get { return _max; }
            set { _max = value; }
        }

        [JsonProperty("result", NullValueHandling = NullValueHandling.Ignore)]
        public Points Result
        {
            get { return _result; }
            set { _result = value; }
        }
    }
}