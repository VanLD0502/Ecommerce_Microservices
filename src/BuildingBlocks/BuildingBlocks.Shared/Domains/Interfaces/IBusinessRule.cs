namespace BuildingBlocks.Shared.Domains.Interfaces;

public interface IBusinessRule
{
    public bool IsBroken();
    public string ErrorMessage { get; }
}