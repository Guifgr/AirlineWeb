using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AirlineSendAgent.Dtos.FlightDetails;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace AirlineSendAgent.Client
{
    public interface IWebhookClient
    {
        Task SendWebhookNotification(FlightDetailChangePayloadDto flightDetailChangePayloadDto);
    }

    public class WebHookClient : IWebhookClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public WebHookClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task SendWebhookNotification(FlightDetailChangePayloadDto flightDetailChangePayloadDto)
        {
            var payload = JsonSerializer.Serialize(flightDetailChangePayloadDto);
            var httpClient = _httpClientFactory.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Post, flightDetailChangePayloadDto.WebhookUri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            request.Content = new StringContent(payload);

            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            try
            {
                using (var response = await httpClient.SendAsync(request))
                {
                    Console.WriteLine("Sucess");
                    response.EnsureSuccessStatusCode();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: "+ e);
            }
        }
    }
}