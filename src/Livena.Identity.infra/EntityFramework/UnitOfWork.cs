using Livena.Identity.Application.Interfaces;
using Livena.Identity.Domain.Shared;
using Microsoft.Extensions.Logging;

namespace Livena.Identity.infra.EntityFramework;

public class UnitOfWork : IUnitOfWork
{
    private readonly LivenaIdentityDbContext _context;
    private readonly IDomainEventPublisher _publisher;
    private readonly ILogger<UnitOfWork> _logger;

    public UnitOfWork(
        LivenaIdentityDbContext context,
        IDomainEventPublisher publisher,
        ILogger<UnitOfWork> logger
    )
    {
        _context = context;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task Commit(CancellationToken cancellationToken)
    {
        var aggregateRoots = _context
            .ChangeTracker.Entries<AggregateRoot>()
            .Where(entry => entry.Entity.Events.Any())
            .Select(entry => entry.Entity);

        _logger.LogInformation(
            "Commit: {AggregatesCount} aggregate roots with events.",
            aggregateRoots.Count()
        );

        var events = aggregateRoots.SelectMany(aggregate => aggregate.Events);

        _logger.LogInformation("Commit: {EventsCount} events raised.", events.Count());

        foreach (var @event in events)
            await _publisher.PublishAsync((dynamic)@event, cancellationToken);

        foreach (var aggregate in aggregateRoots)
            aggregate.ClearEvents();

        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task Rollback(CancellationToken cancellationToken) => Task.CompletedTask;
}
