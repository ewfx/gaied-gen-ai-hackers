namespace MailClassifyer.Server.Entities
{

    public class HuggingFaceLLMResponse
    {
        public string sequence { get; set; }
        public string[] labels { get; set; }
        public float[] scores { get; set; }
    }

}
