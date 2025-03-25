namespace MailClassifyer.Server.Entities
{
    public interface IMailClassifyService
    {
        public Task<List<Email>> GetClassifiedMailInboxById(string mailId);
    }
}