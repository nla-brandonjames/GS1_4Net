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
using System.Net.Http;
using System.Text;

namespace GS1_4Net.Filters
{
    public class ProductSearchFilter : Filter
    {
        public string Gpc { get; set; }

        public string TargetMarket { get; set; }

        public override void AddFilter(HttpRequestMessage message)
        {
            var filters = new Dictionary<string, string>();

            if (Gpc != null)
            {
                filters.Add("gpc", Gpc);
            }

            if (TargetMarket != null)
            {
                filters.Add("tm", TargetMarket);
            }

            base.AddFilter(message, filters);
        }
    }
}
