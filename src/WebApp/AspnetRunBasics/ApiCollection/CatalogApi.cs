using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AspnetRunBasics.ApiCollection.Infrastructure;
using AspnetRunBasics.ApiCollection.Interfaces;
using AspnetRunBasics.Models;
using AspnetRunBasics.Settings;
using Newtonsoft.Json;

namespace AspnetRunBasics.ApiCollection
{
    public class CatalogApi : BaseHttpClientWithFactory, ICatalogApi
    {
        private readonly IApiSettings _settings;

        public CatalogApi(IHttpClientFactory factory, IApiSettings settings) : base(factory)
        {
            _settings = settings;
        }
        public override HttpRequestBuilder GetHttpRequestBuilder(string path)
        {
            return new HttpRequestBuilder(path);
        }

        public async Task<IEnumerable<CatalogModel>> GetCatalog()
        {
            var message = GetHttpRequestBuilder(_settings.BaseAddress)
                                    .SetPath(_settings.CatalogPath)
                                    .HttpMethod(HttpMethod.Get)
                                    .GetHttpMessage();

            return await SendRequest<IEnumerable<CatalogModel>>(request: message);
        }

        public async Task<IEnumerable<CatalogModel>> GetCatalogByCategory(string category)
        {
            var message = GetHttpRequestBuilder(_settings.BaseAddress)
                .SetPath(_settings.CatalogPath)
                .AddToPath("GetProductByCategoryName")
                .AddToPath(category)
                .HttpMethod(HttpMethod.Get)
                .GetHttpMessage();
            return await SendRequest<IEnumerable<CatalogModel>>(request: message);
        }

        public async Task<CatalogModel> GetCatalog(string id)
        {
            //GET http://localhost:7000/Catalog/id
            var message = GetHttpRequestBuilder(_settings.BaseAddress)
                                    .SetPath(_settings.CatalogPath)
                                    .AddToPath(id)
                                    .HttpMethod(HttpMethod.Get)
                                    .GetHttpMessage();
            return await SendRequest<CatalogModel>(request: message);
        }

        public async Task<CatalogModel> CreateCatalog(CatalogModel catalogModel)
        {
            var message = GetHttpRequestBuilder(_settings.BaseAddress)
                            .SetPath(_settings.CatalogPath)
                            .HttpMethod(HttpMethod.Post)
                            .GetHttpMessage();

            var json = JsonConvert.SerializeObject(catalogModel);
            message.Content = new StringContent(json, Encoding.UTF8, "application/json");

            return await SendRequest<CatalogModel>(message);
        }
    }
}