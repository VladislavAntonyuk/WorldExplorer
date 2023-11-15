namespace WebApp.Apis.V1.Controllers;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// Temporary remove for Apple client [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public abstract class ApiAuthControllerBase : ApiControllerBase
{
}