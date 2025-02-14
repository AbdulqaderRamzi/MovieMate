using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace MovieMate.Api.Controllers;

public class ErrorsController : ControllerBase
{
    [Route("/error")]
    public IActionResult Error() {
        // Pull the error
        var exception = HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;
        return Problem(
            title: exception?.Message
        );
    }
}