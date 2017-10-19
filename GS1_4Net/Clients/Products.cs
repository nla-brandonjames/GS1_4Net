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

using GS1_4Net.Domain.Entities;
using GS1_4Net.Filters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GS1_4Net.Clients
{
    public class Products : GS1ClientBase
    {
        public Products(Configuration configuration) : base(configuration)
        {
            BaseUri = "products";
        }

        public async Task<IList<Product>> GetAllProductsAsync()
        {
            var response = await GetDataAsync<IList<Product>>();
            return response;
        }

        public async Task<IList<Product>> SearchProductsAsync(string gpc, string targetMarket)
        {
            var productSearchFilter = new ProductSearchFilter { Gpc = gpc, TargetMarket = targetMarket };
            var response = await GetDataAsync<IList<Product>>("search", productSearchFilter);
            return response;
        }

        public async Task<IList<Product>> QueryProductInformationAsync(string gtin, string targetMarket)
        {
            var response = await GetDataAsync<IList<Product>>(string.Format("{0}/{1}", gtin, targetMarket));
            return response;
        }

        public async Task<Status> AuthenticateKeyAsync(string gtin)
        {
            var response = await GetDataAsync<Status>(string.Format("{0}/authenticate", gtin));
            return response;
        }

        public async Task<Product> ValidateProductAsync(string gtin, string targetMarket)
        {
            var response = await GetDataAsync<Product>(string.Format("{0}/{1}", gtin, targetMarket));
            return response;
        }

        // TODO: Grab the request object and pull the "Last-Modified" header from it.
        public async Task<DateTime> GetProductTimestampAsync(string gtin, string targetMarket)
        {
            var response = await HeadDataAsync<JObject>(string.Format("{0}/{1}", gtin, targetMarket));
            return DateTime.Now;
        }

        public async Task<bool> CreateProductAsync(Product newItem)
        {
            var response = await PostDataAsync<Product, JObject>(newItem);
            return true;
        }

        public async Task<bool> BuilkUploadProductAsync(IList<Product> newItems)
        {
            var response = await PostDataAsync<IList<Product>, Status>(newItems);
            return response.StatusCode == 1;
        }

        public async Task<Product> ModifyProductAsync(string gtin, string targetMarket, Product newItem)
        {
            var response = await PutDataAsync(string.Format("{0}/{1}", gtin, targetMarket), newItem);
            return response;
        }

        public async Task<Product> EditProductAsync(string gtin, string targetMarket, Product newItem)
        {
            var response = await PatchDataAsync(string.Format("{0}/{1}", gtin, targetMarket), newItem);
            return response;
        }

        public async Task<bool> DeleteProductAsync(string gtin, string targetMarket)
        {
            var response = await DeleteDataAsync<JObject>(string.Format("{0}/{1}", gtin, targetMarket));
            return true;
        }

        public async Task<IList<Status>> BulkDeleteProductsAsync(IList<Product> products)
        {
            var response = await PostDataAsync<IList<Product>, IList<Status>>("delete", products);
            return response;
        }

        // TODO: File download / upload
    }
}
