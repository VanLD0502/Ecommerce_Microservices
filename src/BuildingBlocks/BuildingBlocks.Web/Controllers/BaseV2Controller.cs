using BuildingBlocks.Shared.InfrastructureInterfaces.InMemoryBus;
using Microsoft.AspNetCore.Mvc;

namespace BuildingBlocks.Web.Controllers;



[ApiController]
[Route("api/v2/[controller]")]
public abstract class BaseV2Controller : ControllerBase
{
}