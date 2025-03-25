using System.Net.Http;
using System.Text.Json;
using MailClassifyer.Server.Entities;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using static System.Net.WebRequestMethods;

namespace MailClassifyer.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmailClassifyerController : ControllerBase
    {
        private readonly ILogger<EmailClassifyerController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IMailClassifyService _classifyService;

        public EmailClassifyerController(ILogger<EmailClassifyerController> logger, IConfiguration configuration, IMailClassifyService classifyService)
        {
            _logger = logger;
            _configuration = configuration;
            _classifyService = classifyService;
        }

        [HttpGet(Name = "GetEmailClassifyer")]
        public IEnumerable<Email> Get()
        {
            _logger.LogInformation("Calling GetClassifiedMailInboxById from ClassifierService ");
            var mailId = (_configuration.GetSection("OpenAIServiceOptions"))["MailId"];
            var response = _classifyService.GetClassifiedMailInboxById(mailId);
            return response?.Result.Count > 0 ? response.Result.ToArray() : new List<Email>().ToArray();
        }
    }
}