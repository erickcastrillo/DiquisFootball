using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace Diquis.WebApi.Controllers
{
    /// <summary>
    /// Fallback API controller for loading the SPA client initially.
    /// Serves the static index.html file for client-side routing in SPA frameworks (e.g., React, Vue).
    /// </summary>
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class FallbackController : Controller
    {
        /// <summary>
        /// Serves the SPA's index.html file from the wwwroot folder.
        /// This endpoint is used as a fallback to support client-side routing.
        /// </summary>
        /// <remarks>
        /// Serves the static index.html file for SPA client-side routing.<br/>
        /// <b>Sample request:</b> GET /api/Fallback<br/>
        /// <b>Authorization:</b> Anonymous.
        /// </remarks>
        /// <response code="200">Returns the static index.html file.</response>
        /// <response code="404">If the index.html file is not found.</response>
        /// <response code="500">If an internal server error occurs.</response>
        public IActionResult Index()
        {
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html"), "text/HTML");
        }
    }
}
