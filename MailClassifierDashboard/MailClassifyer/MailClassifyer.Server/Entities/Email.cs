namespace MailClassifyer.Server.Entities
{
    public class Email
    {
        public string MailTo { get; set; }
        public string MailFrom { get; set; }
        public string MailSubject { get; set; }
        public string MailText { get; set; }
        public string MailTimestamp { get; set; }

        public string Severity { get; set; }

        public string BusinessCategory { get; set; }
    }

    public class GuerrillaMailResponse
    {
        public string email_addr { get; set; } // Maps to "email_addr" in JSON
        public string sid_token { get; set; }     // Maps to "sid_token" in JSON
    }

    public class OpenAIResponse
    {
        public Choice[] choices { get; set; }
    }

    public class Choice
    {
        public Message message { get; set; }
    }

    public class Message
    {
        public Message() { }

        public Message(string role, string content)
        {
            this.Role = role;
            this.Content = content;
        }

        public string Role { get; set; }

        public string Content { get; set; }
    }
}
