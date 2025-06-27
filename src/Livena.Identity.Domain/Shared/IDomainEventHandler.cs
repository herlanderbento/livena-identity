namespace Livena.Identity.Domain.Shared;

public interface IDomainEventHandler<TDomainEvent> where TDomainEvent: DomainEvent
{
    Task HandleAsync(TDomainEvent domainEvent, CancellationToken cancellationToken);
}