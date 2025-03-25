namespace MailClassifyer.Server.Entities
{
    public static class EmailContentStatic
    {
        public static List<Email> GetMailStaticContent()
        {
            return new List<Email>
            {
            new Email{ MailFrom="Peter@ionmail.com", MailTo = "WFSsupport@wfsBank.com",  MailSubject ="Payment System Down Critical Alert", MailText = "The entire payment system has been down for over 3 hours. We are losing revenue rapidly. This needs High attention!", MailTimestamp = "24/03/2025", BusinessCategory = "", Severity = "" },
            new Email{ MailFrom="Fed@Jmail.com", MailTo = "WFSsupport@wfsBank.com",   MailSubject ="Queries related to general customer service, complaints, or technical support for online banking", MailText = "I'm unable to access my auto loan account on the online banking portal. Could you assist me with this issue?", MailTimestamp = "24/03/2025", BusinessCategory = "", Severity = "" },         
            new Email{ MailFrom="tom@joymail.com",  MailTo = "WFSsupport@wfsBank.com", MailSubject ="Regarding Login issues", MailText = "A few users are reporting intermittent login issues. Could someone take a look when possible?", MailTimestamp = "24/03/2025", BusinessCategory = "", Severity = "" },
            new Email{ MailFrom="John@potmail.com",  MailTo = "WFSsupport@wfsBank.com", MailSubject ="Regarding Terms Update", MailText = "We received a request to update the terms and conditions on the website. There's no rush, but it should be done this week. Please support", MailTimestamp = "24/03/2025", BusinessCategory = "", Severity = "" }
            };
        }
    }
}
