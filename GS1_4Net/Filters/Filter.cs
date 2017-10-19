#region License
/*
 * Copyright 2017 Brandon James
 *
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace GS1_4Net.Filters
{
    public class Filter : IFilter
    {
        public string OrganizationId { get; set; }

        public virtual void AddFilter(HttpRequestMessage message)
        {
            AddFilter(message, new Dictionary<string, string>());
        }

        public virtual void AddFilter(HttpRequestMessage message, IDictionary<string, string> filters)
        {
            var filtersString = "";

            if (filters.Values.Count > 0)
            {
                filtersString = "&" + EncodeFilterString(filters);
            }

            message.RequestUri = new Uri(message.RequestUri.ToString() + filtersString, UriKind.Relative);
        }

        public string EncodeFilterString(IDictionary<string, string> filters)
        {
            if (filters.Keys.Count == 0)
            {
                return "";
            }
            return string.Join("&",
                    filters.Select(kvp =>
                    string.Format("{0}={1}", kvp.Key, HttpUtility.UrlEncode(kvp.Value))));
        }
    }
}
