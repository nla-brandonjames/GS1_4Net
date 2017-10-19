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

namespace GS1_4Net.Clients
{
    public class GS1ClientBase : ClientBase
    {
        public GS1ClientBase(Configuration configuration) : base(configuration)
        {
            configuration.BaseUri = new Uri("https://cloud.stg.gs1.org/gs1-pds/api/", UriKind.Absolute);
            configuration.UserAgent = "GS1_4Net";
        }
    }
}
