﻿#region License
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

namespace GS1_4Net
{
    public class Configuration
    {
        public string UserAgent { get; set; }
        public Uri BaseUri { get; set; }
        public string UserName { get; set; }
        public string UserApiKey { get; set; }
        public int ApiLimit { get; set; }
        public int RecordsPerPage { get; set; }
        public bool AllowDeletions { get; set; }

        public Configuration()
        {
            ApiLimit = 100;
        }

        internal void AreConfigurationSet()
        {
            if (UserName == null)
            {
                throw new ArgumentNullException("UserName", "UserName cannot be null.");
            }

            if (UserApiKey == null)
            {
                throw new ArgumentNullException("UserApiKey", "UserApiKey cannot be null.");
            }
        }
    }
}
