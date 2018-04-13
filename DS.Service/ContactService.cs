using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS.DL.DataContext.Base.Interfaces;
using DS.Domain;
using DS.Domain.Interface;
using StructureMap;

namespace DS.Service
{
    public  class ContactService : IContactService
    {
        private IRepository<Contact> _repository;
        private readonly ISystemConfigurationService _systemConfigService;
        private readonly IEmailService _emailService;

        public ContactService(IRepository<Contact> repository, ISystemConfigurationService systemConfigService, IEmailService emailService)
        {
            _repository = repository;
            _systemConfigService = systemConfigService;
            _emailService = emailService;
        }

        public  IList<Contact> GetAll(){
            return _repository.GetAll().OrderByDescending(c => c.DateCreated).ToList();
        }

        public GenericResult Create(Contact contact)
        {
            var result = new GenericResult(true, "");

            
            _repository.Add(contact);
            _repository.SaveChanges();
            var str = FormatAndSendContact(contact);
            result.Message = str;
            return result;
        }

        private string  FormatAndSendContact(Contact contact)
        {
            if (_systemConfigService == null) return "";
            var recipientEmail = _systemConfigService.GetSystemConfigurations().SendEmailForFeedback;
            if (string.IsNullOrEmpty(recipientEmail))
                return "";
            var sb = new StringBuilder();
            sb.Append("The following message was submitted from the DataShare Contact us page\r\n<br/>");
            sb.Append("From: " + contact.FromEmail + "\r\n<br/>");
            sb.Append("Name: " + contact.FromName + "\r\n<br/>");
            sb.Append("Message: \r\n<br/><p>" + contact.Message + "</p>\r\n<br/>");
            
           
            return _emailService.SendEmail(recipientEmail, "DataShare Feedback", sb.ToString(), contact.FromEmail);

        }
    }
}
