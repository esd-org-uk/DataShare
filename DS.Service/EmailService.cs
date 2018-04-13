using System;


namespace DS.Service
{
    using System.Net;
    using System.Net.Mail;
    using DS.Domain.Interface;

    public class EmailService : IEmailService
    {
        private readonly ISystemConfigurationService _systemConfigurationService;

        public EmailService(ISystemConfigurationService systemConfigurationService)
        {
            _systemConfigurationService = systemConfigurationService;
        }

        public string SendEmail(string recipientemail, string subject, string body, string returnemail)
        {
            var client = new SmtpClient { DeliveryMethod = SmtpDeliveryMethod.Network };
            
            var systemObject = _systemConfigurationService.GetSystemConfigurations();
            
            if (!string.IsNullOrEmpty(systemObject.SmtpServer)) client.Host = systemObject.SmtpServer;
            
            if (string.IsNullOrEmpty(systemObject.SmtpUsername) && string.IsNullOrEmpty(systemObject.SmtpPassword))
            {
                var oCredential = new NetworkCredential("", "");
                client.UseDefaultCredentials = false;
                client.Credentials = oCredential;
            }else
            {
                client.Credentials = new NetworkCredential(systemObject.SmtpUsername, systemObject.SmtpPassword);
            }


            ////if (attachments != null && attachments.Count > 0)
            ////{
            ////    foreach (var a in attachments)
            ////    {
            ////        message.Attachments.Add(a);
            ////    }
            ////}

            try
            {
                var message = new MailMessage(returnemail, recipientemail, subject, body)
                    {IsBodyHtml = true,};
                
                client.Send(message);
                return "Email sent!";
            }
            catch (Exception ex)
            {
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
                return ex.Message;
            }
        }
    }
}
