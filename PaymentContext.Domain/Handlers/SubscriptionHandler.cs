using System;
using Flunt.Notifications;
using Flunt.Validations;
using PaymentContext.Domain.Commands;
using PaymentContext.Domain.Entities;
using PaymentContext.Domain.Enums;
using PaymentContext.Domain.Repositories;
using PaymentContext.Domain.Services;
using PaymentContext.Domain.ValueObjects;
using PaymentContext.Shared.Commands;
using PaymentContext.Shared.Handlers;

namespace PaymentContext.Domain.Handlers
{
    public class SubscriptionHandler : Notifiable, IHandler<CreateBoletoSubscriptionCommand>
    {
        private readonly IStudentRepository _repository;
        private readonly IEmailService _emailService;

        public SubscriptionHandler(IStudentRepository repository, IEmailService emailService)
        {
            _repository = repository;
            _emailService = emailService;
        }

        public ICommandResult Handle(CreateBoletoSubscriptionCommand command)
        {
            // fail fast validation
            command.Validate();
            if (command.Invalid)
            {
                AddNotifications(command);
                return new CommandResult(false, "Não foi possível realizar sua assinatura.");
            }

            // verificiar se documento já está cadastardo
            if (_repository.DocumentExists(command.Document))
            {
                AddNotification("Document", "Este documento já existe.");
            }

            // verificiar se email já está cadastardo
            if (_repository.EmailExists(command.Email))
            {
                AddNotification("Email", "Este email já existe.");
            }

            // gerar VOs
            var name = new Name(command.FirstName, command.LastName);
            var document = new Document(command.Document, EDocumentType.CPF);
            var email = new Email(command.Email);
            var address = new Address(command.Street, command.Number, command.Neighborhood, command.City, command.State, command.Country, command.ZipCode);

            // gerar Entidades
            var student = new Student(name, document, email);
            var subscription = new Subscription(DateTime.Now.AddMonths(1));
            var payment = new BoletoPayment(command.BarCode, command.BoletoNumber, command.PaidDate, command.ExpireDate, command.Total, command.TotalPaid, new Document(command.PayerDocument, command.PayerDocumentType), command.Payer, address, email);

            // relacionamentos
            subscription.AddPayment(payment);
            student.AddSubscription(subscription);

            // aplicar validações
            AddNotifications(name, document, email, address, student, subscription, payment);

            if (Invalid) return new CommandResult(false, "Não foi possível criar a sua assinatura.");

            // salvar infos
            _repository.CreateSubscription(student);

            // enviar email de boas-vindas
            _emailService.Send(student.Name.ToString(), student.Email.Address, "Bem vindo!", "Sua assinatura foi criada com sucesso!");

            // retornar informações
            return new CommandResult(true, "Assinatura realizada com sucesso.");
        }
    }
}