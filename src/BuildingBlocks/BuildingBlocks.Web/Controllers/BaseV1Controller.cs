using Microsoft.AspNetCore.Mvc;

namespace BuildingBlocks.Web.Controllers;



[ApiController]
[Route("api/v1/[controller]")]
public abstract class BaseV1Controller : ControllerBase
{
    
}