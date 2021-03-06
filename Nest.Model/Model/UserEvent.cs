﻿/*
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
    [Cloudname("user_event")]
    public class UserEvent : CloudObject
    {
        private int _id;
        private string _tag;
        private string _activity;
        private string _support_text;
        private string _createdAt;

        public override string CloudKey
        {
            get { return _id.ToString(); }
        }

        [JsonProperty("id")]
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        [JsonProperty("tag")]
        public string Tag
        {
            get { return _tag; }
            set
            {
                SetProperty(ref _tag, value);
            }
        }

        [JsonProperty("activity")]
        public string Activity
        {
            get { return _activity; }
            set
            {
                SetProperty(ref _activity, value);
            }
        }

        [JsonProperty("support_text")]
        public string SupportText
        {
            get { return _support_text; }
            set
            {
                SetProperty(ref _support_text, value);
            }
        }

        [JsonProperty("created_at")]
        public string CreatedAt
        {
            get { return _createdAt; }
            set
            {
                SetProperty(ref _createdAt, value);
            }
        }
    }
}

