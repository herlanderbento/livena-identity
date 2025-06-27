namespace Livena.Identity.Domain.Shared;

public abstract class DomainEvent
{
    public DateTime OccuredOn { get; set; }
    protected DomainEvent()
    {
        OccuredOn = DateTime.Now;
    }
}