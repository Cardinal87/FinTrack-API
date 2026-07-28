using FinTrack.API.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace FinTrack.API.Controllers.Base
{
    public abstract class FinTrackContollerBase : ControllerBase
    {
        protected IActionResult HandleFailedResult(ResultBase result)
        {
            int statusCode = result.StatusMessage switch
            {
                OperationStatusMessages.BadRequest => 400,
                OperationStatusMessages.Forbidden => 403,
                OperationStatusMessages.NotFound => 404,
                OperationStatusMessages.Unauthorized => 401,
                OperationStatusMessages.Conflict => 409,
                _ => 500
            };

            string type = statusCode switch
            {
                400 => "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                401 => "https://tools.ietf.org/html/rfc9110#section-15.5.2",
                403 => "https://tools.ietf.org/html/rfc9110#section-15.5.4",
                404 => "https://tools.ietf.org/html/rfc9110#section-15.5.5",
                409 => "https://tools.ietf.org/html/rfc9110#section-15.5.10",
                500 => "https://tools.ietf.org/html/rfc9110#section-15.6.1",
                _ => $"https://httpstatuses.io/{statusCode}"
            };

            var problemDetails = new ProblemDetails
            {
                Type = type,
                Status = statusCode,
                Instance = $"{HttpContext.Request.Method} {HttpContext.Request.Path}",
                Title = result.StatusMessage,
                Detail = result.ErrorMessage ?? "unexpected server error"
            };

            return new ObjectResult(problemDetails) { StatusCode = statusCode };
        }
    }
}
