namespace BuildingBlocks.Shared.Domains;

public class FluentValidationException(Dictionary<string, List<string>> errors) : Exception("Validation Failed")
{
    public readonly Dictionary<string, List<string>> Errors = errors;
}