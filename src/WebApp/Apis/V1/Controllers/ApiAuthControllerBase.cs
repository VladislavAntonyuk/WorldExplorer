namespace WebApp.Apis.V1.Controllers;

using Microsoft.AspNetCore.Mvc;

// [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public abstract class ApiAuthControllerBase : ApiControllerBase
{
}