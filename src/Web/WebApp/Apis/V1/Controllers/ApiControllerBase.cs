namespace WebApp.Apis.V1.Controllers;

using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public abstract class ApiControllerBase : ControllerBase
{
}