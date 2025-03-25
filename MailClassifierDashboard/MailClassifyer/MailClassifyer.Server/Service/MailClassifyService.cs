using MailClassifyer.Server.Entities;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using OpenAI;
//using OpenAI.ObjectModels.RequestModels;
using OpenAI.Chat;
using System.Text;
using Newtonsoft.Json;
using System.Net.Mail;
using Microsoft.Extensions.Logging;

namespace MailClassifyer.Server.Service
{
    public class MailClassifyService : IMailClassifyService
    {
        private readonly OpenAIClient _openAIClient;
        private const string Model = "gpt-3.5-turbo";
        private readonly IConfiguration _configuration;
        private ILogger<MailClassifyService> _logger;
        private static readonly HttpClient client = new HttpClient();

        public MailClassifyService(IConfiguration configuration, ILogger<MailClassifyService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public Task<List<Email>> GetClassifiedMailInboxById(string mailId)
        {
            var emailsInboxById = GetMailsByMailinatorBox(mailId);

            var emailsClassifiedById = GetEmailsClassifiedById(emailsInboxById);

            return Task.FromResult(emailsClassifiedById).Result;
        }


        private List<Email> GetMailsByMailinatorBox(string mailId)
        {
            var responseList = new List<Email>();
            var url = $"https://api.temp-mail.org/request/mail/id/{mailId}/format/json/";

            try
            {
                // Fetch emails from the Temp-Mail API
                HttpResponseMessage response = client.GetAsync(url).GetAwaiter().GetResult();
                if (response.IsSuccessStatusCode)
                {
                    // Read and parse the response
                    string responseData = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    var emails = JsonConvert.DeserializeObject<Email[]>(responseData);

                    // Display email details
                    //Console.WriteLine($"Emails for {emailAddress}:");
                    foreach (var email in emails)
                    {
                        _logger.LogInformation($"From: {email.MailFrom}");
                        _logger.LogInformation($"Subject: {email.MailSubject}");
                        _logger.LogInformation($"Body: {email.MailText}");
                        _logger.LogInformation($"Timestamp: {email.MailTimestamp}");
                    }
                }
                else
                {
                    _logger.LogInformation($"Error: {response.StatusCode}");
                    responseList = EmailContentStatic.GetMailStaticContent().ToList();
                     //response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    //_logger.LogInformation($"Details: {errorResponse}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }

            finally
            {
                responseList = EmailContentStatic.GetMailStaticContent().ToList();
            }
            return responseList;
        }

        private Task<List<Email>> GetEmailsClassifiedById (List<Email> emailContent)
        {
            var responseList = new List<Email>();
            string[] businessCategories = new string[] { "Support", "Sales", "Billing", "Technical Issue", "Others" };
            string[] severity = new string[] { "Critical","Medium", "High", "Low" };

            foreach (var email in emailContent)
            {
                // Hugging Face API Key
                string apiKey = (_configuration.GetSection("OpenAIServiceOptions"))["ApiKey"];

                // Create the JSON payload for zero-shot classification
                var requestBody = new
                {
                    inputs = email.MailSubject,
                    parameters = new { candidate_labels = businessCategories }  // Define the categories you're interested in
                };

                // Serialize the request body to JSON
                string jsonContent = JsonConvert.SerializeObject(requestBody);

                using (HttpClient client = new HttpClient())
                {
                    // Set the API key in the authorization header
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                    // Make the POST request to Hugging Face API (using zero-shot classification model)
                    var response = client.PostAsync(
                        "https://api-inference.huggingface.co/models/facebook/bart-large-mnli", // Zero-shot classification model
                        new StringContent(jsonContent, Encoding.UTF8, "application/json")
                    );

                    // Read the response to check business category
                    var businessCatergory = JsonConvert.DeserializeObject<HuggingFaceLLMResponse> (response.GetAwaiter().GetResult().Content.ReadAsStringAsync().Result);//.Content.ReadAsStringAsync();

                    // Call to Decide Severity
                    requestBody = new
                    {
                        inputs = email.MailSubject,
                        parameters = new { candidate_labels = severity }  // Define the categories you're interested in
                    };
                    jsonContent = JsonConvert.SerializeObject(requestBody);

                    response = client.PostAsync(
                        "https://api-inference.huggingface.co/models/facebook/bart-large-mnli", // Zero-shot classification model
                        new StringContent(jsonContent, Encoding.UTF8, "application/json")
                    );

                    // Read the response to check severity
                    var severityResult = JsonConvert.DeserializeObject<HuggingFaceLLMResponse>(response.GetAwaiter().GetResult().Content.ReadAsStringAsync().Result);//.Content.ReadAsStringAsync();


                    responseList.Add(
                        new Email 
                        { 
                           MailFrom = email.MailFrom,
                           MailTo = email.MailTo,
                           MailSubject = email.MailSubject,
                           MailText = email.MailText,
                           MailTimestamp = email.MailTimestamp,
                           BusinessCategory = businessCatergory.labels[0],
                           Severity = severityResult.labels[0],
                        });
                }
            }
            return Task.FromResult(responseList);
        }
    }
}
