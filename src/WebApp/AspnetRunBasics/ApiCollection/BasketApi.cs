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
    public class BasketApi : BaseHttpClientWithFactory, IBasketApi
    {
        private readonly IApiSettings _apiSettings;

        public BasketApi(IHttpClientFactory factory, IApiSettings apiSettings) : base(factory)
        {
            _apiSettings = apiSettings;
        }
        public override HttpRequestBuilder GetHttpRequestBuilder(string path)
        {
            return new HttpRequestBuilder(path);
        }

        public async Task<BasketModel> GetBasket(string username)
        {
            var request = GetHttpRequestBuilder(_apiSettings.BaseAddress)
                        .SetPath(_apiSettings.BasketPath)
                        .AddQueryString("username", username)
                        .HttpMethod(HttpMethod.Get)
                        .GetHttpMessage();
            return await SendRequest<BasketModel>(request: request);
        }

        public async Task<BasketModel> UpdateBasket(BasketModel basketModel)
        {
            var message = GetHttpRequestBuilder(_apiSettings.BaseAddress)
                .SetPath(_apiSettings.BasketPath)
                .HttpMethod(HttpMethod.Post)
                .GetHttpMessage();
            var json = JsonConvert.SerializeObject(basketModel);
            message.Content = new StringContent(json, Encoding.UTF8, "application/json");
            return await SendRequest<BasketModel>(request: message);
        }

        public async Task CheckoutBasket(BasketCheckoutModel model)
        {
            var message = GetHttpRequestBuilder(_apiSettings.BaseAddress)
                .SetPath(_apiSettings.BasketPath)
                .AddToPath("Checkout")
                .HttpMethod(HttpMethod.Post)
                .GetHttpMessage();

            var json = JsonConvert.SerializeObject(model);
            message.Content = new StringContent(json, Encoding.UTF8, "application/json");

            await SendRequest<BasketCheckoutModel>(request: message);
        }
    }
}