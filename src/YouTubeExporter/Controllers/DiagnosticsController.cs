using Microsoft.AspNetCore.Mvc;

namespace YoutubeExporter.Controllers
{
    // Serves the identity of the running build from the ProductionVersion.json that ships
    // beside the app (generated at build time). See the grdev standard, "Production version
    // file" — a Web API exposes it at GET /api/diagnostics/version.
    [ApiController]
    [Route("/api/diagnostics")]
    public class DiagnosticsController : ControllerBase
    {
        [HttpGet("version")]
        public IActionResult GetVersion()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "ProductionVersion.json");
            if (!System.IO.File.Exists(path))
                return NotFound(new { error = "ProductionVersion.json not found" });

            // Return the file's contents verbatim so the full SHA and exact fields are preserved.
            return Content(System.IO.File.ReadAllText(path), "application/json");
        }
    }
}
