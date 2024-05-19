namespace Ecmanage.eProcessor.Services.FakeFetch.FakeFetch.Domain.Events;

public record LoginIntegrationEvent(int EmailQueueId, int EmailId, string FullName, string Environment, string Date, string Time, string EmailTo, string Subject) : BaseEmailIntegrationEvent(EmailQueueId, EmailTo, Subject);
