using System.Net.Http;
using System.Text.Json;
using MailClassifyer.Server.Entities;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
//using OpenAI;
using static System.Net.WebRequestMethods;

namespace MailClassifyer.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IConfiguration _configuration;
        private static readonly HttpClient client = new HttpClient();
        private readonly IMailClassifyService _classifyService;


        public WeatherForecastController(ILogger<WeatherForecastController> logger, IConfiguration configuration, IMailClassifyService classifyService)
        {
            _logger = logger;
            _configuration = configuration;
            _classifyService = classifyService;
            ConnectOpenAPI();
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        private void ConnectOpenAPI()
        {
            //var apiKey = _configuration["ApiKey"];// "your-openai-api-key";
            var apiKey = (_configuration.GetSection("OpenAIServiceOptions"))["ApiKey"];
            var mailId = (_configuration.GetSection("OpenAIServiceOptions"))["MailId"];
            //_classifyService.GetClassifiedMailInboxById(mailId);

            // Initialize the OpenAI client
            // var openAIClient = new OpenAIClient(apiKey);
            //_classifyService.GetMailInboxById("");
            //GetMailDetails();
        }

        private async void  GetMailDetails()
        {
            string emailAddress = (_configuration.GetSection("OpenAIServiceOptions"))["MailId"];
            //string url = "https://api.temp-mail.io/request/mail/id/relevo2835@hikuhu.com/format/json/";
            //string url = $"https://api.temp-mail.io/request/mail/id/fuiajpq939@dygovil.com/format/json/";
            //string url = $"https://api.temp-mail.io/v1/emails/fuiajpq939@dygovil.com/messages";

            var client = new HttpClient();
            //var request = new HttpRequestMessage(HttpMethod.Get, "https://api.temp-mail.io/v1/emails/fuiajpq939@dygovil.com/messages");
            //request.Headers.Add("Accept", "application/json");
            //request.Headers.Add("X-API-Key", "apiKey");
            //var response = await client.SendAsync(request);
            //response.EnsureSuccessStatusCode();
            //Console.WriteLine(await response.Content.ReadAsStringAsync());

            //string url = $"https://api.temp-mail.io/request/mail/id/fuiajpq939@dygovil.com/format/json/";
            //string url = $"https://api.guerrillamail.com/ajax.php?f=get_email_address";
            //var response = await client.GetAsync(url);
            //response.EnsureSuccessStatusCode();

            // Step 2: Parse the JSON response
            string url = $"https://api.guerrillamail.com/ajax.php?f=get_email_address";
            var response = await client.GetAsync(url);
            string jsonResponse = await response.Content.ReadAsStringAsync();
            var emailContent = JsonSerializer.Deserialize<GuerrillaMailResponse>(jsonResponse);
            var i = 0;


            //var inboxResponse = await client.GetAsync(
            //       $"https://api.guerrillamail.com/ajax.php?f=get_email_list&sid_token=9seugo7csdlft77ond65gfvnbr"
            //   );

            var emailContentResponse = await client.GetAsync(
       $"https://api.guerrillamail.com/ajax.php?f=fetch_email&sid_token={emailContent.sid_token}&email_id={emailContent.email_addr}");

           emailContentResponse.EnsureSuccessStatusCode();

            string inboxData = await emailContentResponse.Content.ReadAsStringAsync();
            var i1 = 0;

            //string inboxName = "abc1";
            //string url = $"https://api.mailinator.com/v2/domains/public/inboxes/{inboxName}";

            // $"https://api.temp-mail.org/request/mail/id/{emailAddress}/format/json/";

            //var request = new HttpRequestMessage
            //{
            //    Method = HttpMethod.Get,
            //    RequestUri = new Uri($"https://privatix-temp-mail-v1.p.rapidapi.com/request/mail/id/jegit52293@boyaga.com/"),
            //    Headers =
            //            {
            //                { "x-rapidapi-key", "Sign Up for Key" },
            //                { "x-rapidapi-host", "privatix-temp-mail-v1.p.rapidapi.com" },
            //            },
            //          };
            //using (var response = await client.SendAsync(request))
            //{
            //    response.EnsureSuccessStatusCode();
            //    var body = await response.Content.ReadAsStringAsync();
            //    Console.WriteLine(body);
            //}


            //try
            //{
            //    // Fetch emails from the Temp-Mail API
            //    HttpResponseMessage response = await client.GetAsync(url);
            //    if (response.IsSuccessStatusCode)
            //    {
            //        // Read and parse the response
            //        string responseData = await response.Content.ReadAsStringAsync();
            //        var emails = JsonSerializer.Deserialize<Email[]>(responseData);

            //        // Display email details
            //        //Console.WriteLine($"Emails for {emailAddress}:");
            //        foreach (var email in emails)
            //        {
            //            Console.WriteLine($"From: {email.MailFrom}");
            //            Console.WriteLine($"Subject: {email.MailSubject}");
            //            Console.WriteLine($"Body: {email.MailText}");
            //            Console.WriteLine($"Timestamp: {email.MailTimestamp}");
            //            Console.WriteLine();
            //        }
            //    }
            //    else
            //    {
            //        Console.WriteLine($"Error: {response.StatusCode}");
            //        string errorResponse = await response.Content.ReadAsStringAsync();
            //        Console.WriteLine($"Details: {errorResponse}");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine($"Exception: {ex.Message}");
            //}
        }
    }
}
