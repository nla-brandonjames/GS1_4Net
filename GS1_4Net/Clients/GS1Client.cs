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
 
namespace GS1_4Net.Clients
{
    public class GS1Client
    {
        public readonly Configuration Configuration;

        public GS1Client(Configuration configuration)
        {
            Configuration = configuration;
        }

        private Products products;

        public Products Products
        {
            get
            {
                if (products != null)
                {
                    return products;
                }
                products = new Products(Configuration);
                return products;
            }
        }
    }
}
