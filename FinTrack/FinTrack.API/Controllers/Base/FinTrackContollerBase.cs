using FinTrack.API.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace FinTrack.API.Controllers.Base
{
    public abstract class FinTrackContollerBase : ControllerBase
    {
        protected IActionResult HandleFailedResult(ResultBase result)
        {
            switch (result.StatusMessage)
            {
                case OperationStatusMessages.BadRequest: return BadRequest();
                case OperationStatusMessages.Forbidden: return Forbid();
                case OperationStatusMessages.NotFound: return NotFound();
                case OperationStatusMessages.Unauthorized: return Unauthorized();
                default: return StatusCode(500, new { message = "unexpected server error" });
            }

        }
    }
}
