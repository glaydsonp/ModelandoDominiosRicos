using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaymentContext.Domain.Commands;
using PaymentContext.Domain.Enums;
using PaymentContext.Domain.Handlers;
using PaymentContext.Tests.Mocks;

namespace PaymentContext.Tests
{
    [TestClass]
    public class SubscriptionHandlerTests
    {
        [TestMethod]
        public void ShouldReturnErrorWhenDocumentExists()
        {
            var handler = new SubscriptionHandler(new FakeStudentRepository(), new FakeEmailService());
            var command = new CreateBoletoSubscriptionCommand();

            command.FirstName = "Glaydson";
            command.LastName = "Prado";
            command.Document = "99999999999";
            command.Email = "admin@glaydsonp.com";
            command.BarCode = "000000000";
            command.BoletoNumber = "11111111";
            command.PaymentNumber = "22222222";
            command.PaidDate = DateTime.Now;
            command.ExpireDate = DateTime.Now.AddDays(2);
            command.Total = 10;
            command.TotalPaid = 10;
            command.Payer = "Glaydson";
            command.PayerDocument = "61099753384";
            command.PayerDocumentType = EDocumentType.CPF;
            command.PayerEmail = "admin@glaydsonp.com";
            command.Street = "Aveninda Um";
            command.Number = "123";
            command.Neighborhood = "Gothan";
            command.City = "Gothan";
            command.State = "SP";
            command.Country = "Brasil";
            command.ZipCode = "13245000";

            handler.Handle(command);
            Assert.AreEqual(false, handler.Valid);

        }
    }
}
