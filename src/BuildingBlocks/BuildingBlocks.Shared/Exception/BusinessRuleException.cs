using BuildingBlocks.Shared.Domains.Interfaces;

namespace BuildingBlocks.Shared.Domains;

public class DomainException(IBusinessRule brokenRule) : Exception(brokenRule.ErrorMessage)
{
    IBusinessRule BrokenRule { get; } = brokenRule;
    
    public override string ToString()
    {
        return $"{BrokenRule.GetType().FullName}: {BrokenRule.ErrorMessage}";
    }
}