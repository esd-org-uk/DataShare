using System;
using System.Linq;
using System.Text;
using DS.DL.DataContext.Base.Interfaces;
using DS.Domain;
using DS.Domain.Interface;
using DS.Service;
using DS.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StructureMap;

namespace DS.Tests.ServicesTests
{
    [TestClass]
    public class ContactServiceTests
    {
        private MemoryRepository<Contact> _repository;
        private ISystemConfigurationService _systemConfigurationService;
        private IEmailService _emailService;

        [TestInitialize]
        public void TestInit()
        {
            _repository = new MemoryRepository<Contact>();

            ObjectFactory.Initialize(
                x => { x.For<IUnitOfWorkFactory>().Use<MemoryUnitOfWorkFactory>(); }
                );
        }

        [TestMethod]
        public void GetAll_returns_all_contacts_in_repository_in_date_created_desc_order()
        {
            //arrange
            var ent1 = new Contact() {DateCreated = new DateTime(2009, 1, 1)};
            var ent2 = new Contact() {DateCreated = new DateTime(2010, 1, 1)};
            _repository.Add(ent1);
            _repository.Add(ent2);
            var sut = new ContactService(_repository, _systemConfigurationService, _emailService);
            //act
            var result = sut.GetAll();
            //assert
            Assert.AreEqual(new DateTime(2010, 1, 1), result[0].DateCreated);
            //cleanup
            _repository.Delete(ent1);
            _repository.Delete(ent2);
        }

        [TestMethod]
        public void Create_will_add_entity_to_repository()
        {
            //arrange
            var ent1 = new Contact() {Id = 2, DateCreated = new DateTime(2009, 1, 1)};

            var sut = new ContactService(_repository, _systemConfigurationService, _emailService);
            //act
            sut.Create(ent1);
            var result = sut.GetAll().FirstOrDefault(x => x.Id == 2);
            //assert
            Assert.AreEqual(new DateTime(2009, 1, 1), result.DateCreated);
            //cleanup
            _repository.Delete(ent1);
        }


        [TestMethod]
        public void Create_when_system_config_SendEmailForFeedback_is_email_will_send_email()
        {
            //arrange
            var ent1 = new Contact() {Id = 2, DateCreated = new DateTime(2009, 1, 1)};
            var sb = new StringBuilder();
            sb.Append("The following message was submitted from the DataShare Contact us page\r\n<br/>");
            sb.Append("From: " + ent1.FromEmail + "\r\n<br/>");
            sb.Append("Name: " + ent1.FromName + "\r\n<br/>");
            sb.Append("Message: \r\n<br/><p>" + ent1.Message + "</p>\r\n<br/>");


            var mock = new Mock<ISystemConfigurationService>();
            mock.Setup(x => x.GetSystemConfigurations())
                .Returns(new SystemConfigurationObject() {SendEmailForFeedback = "test@email.com"});
            _systemConfigurationService = mock.Object;

            var mock2 = new Mock<IEmailService>();
            mock2.Setup(
                x => x.SendEmail("test@email.com", "DataShare Feedback", sb.ToString(), ent1.FromEmail))
                 .Returns("Email sent!");
            _emailService = mock2.Object;

            var sut = new ContactService(_repository, _systemConfigurationService, _emailService);
            //act
            var result = sut.Create(ent1);

            //assert
            Assert.AreEqual("Email sent!", result.Message);
            //cleanup
            _repository.Delete(ent1);
        }
    }
}
