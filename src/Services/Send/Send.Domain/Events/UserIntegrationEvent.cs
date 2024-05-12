using Ecmanage.eProcessor.BuildingBlocks.EventBus.Events;

namespace Ecmanage.eProcessor.Services.Send.Send.Domain.Events;

public record UserIntegrationEvent(int EmailId, string ImageHeader, string Email, string FullName, string UserName, string Password, string Company, string Url) : IntegrationEvent;
