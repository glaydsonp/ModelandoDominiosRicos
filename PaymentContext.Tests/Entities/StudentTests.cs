using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaymentContext.Domain.Entities;
using PaymentContext.Domain.Enums;
using PaymentContext.Domain.ValueObjects;

namespace PaymentContext.Tests.Entities
{
    [TestClass]
    public class StudentTests
    {
        private readonly Student _student;
        private readonly Name _name;
        private readonly Document _document;
        private readonly Email _email;
        private readonly Address _address;
        public StudentTests()
        {
            _name = new Name("Bruce", "Wayne");
            _document = new Document("61099753384", EDocumentType.CPF);
            _email = new Email("batman@dc.com");
            _address = new Address("Rua 1", "1234", "Bairro Legal", "Gothan", "SP", "Brasil", "13400000");
            _student = new Student(_name, _document, _email);
        }

        [TestMethod]
        public void ShouldReturnErrorWhenHasActiveSubscription()
        {
            var subscription = new Subscription(null);
            var payment = new PayPalPayment("12345678", DateTime.Now, DateTime.Now.AddDays(5), 10, 10, _document, "WAYNE CORP", _address, _email);
            subscription.AddPayment(payment);
            _student.AddSubscription(subscription);
            _student.AddSubscription(subscription);

            Assert.IsTrue(_student.Invalid);
        }
        [TestMethod]
        public void ShouldReturnErrorWhenSubscriptionHasNoPayment()
        {
            var subscription = new Subscription(null);

            _student.AddSubscription(subscription);
            Assert.IsTrue(_student.Invalid);
        }
        [TestMethod]
        public void ShouldReturnSuccessWhenAddSubscription()
        {
            var subscription = new Subscription(null);

            var payment = new PayPalPayment("12345678", DateTime.Now, DateTime.Now.AddDays(5), 10, 10, _document, "WAYNE CORP", _address, _email);
            subscription.AddPayment(payment);
            _student.AddSubscription(subscription);
            Assert.IsTrue(_student.Valid);
        }
    }
}