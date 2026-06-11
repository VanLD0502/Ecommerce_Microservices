using BuildingBlocks.Shared.InfrastructureInterfaces.InMemoryBus;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Web.Controllers;

public abstract class CleanV1CustomController : BaseV1Controller
{
    private IInMemoryBus? _senderBus;

    protected IInMemoryBus _sender => _senderBus ??= HttpContext.RequestServices.GetRequiredService<IInMemoryBus>();
}