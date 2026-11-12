using ExamPortal.Api.Data;
using ExamPortal.Api.Models;
using ExamPortal.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ExamPortal.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly QueryExecutor _executor;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ArchiveService _archives;

    public AdminController(QueryExecutor executor, IHttpClientFactory httpClientFactory, ArchiveService archives)
    {
        _executor = executor;
        _httpClientFactory = httpClientFactory;
        _archives = archives;
    }

    [HttpPost("archive")]
    public IActionResult Archive([FromQuery] string name)
    {
        var exitCode = _archives.Run(name);
        return Ok(new { message = "Archive command finished.", exitCode });
    }

    [HttpDelete("candidates/{id:int}")]
    public IActionResult DeleteCandidate(int id)
    {
        var affected = _executor.Execute(Queries.DeleteCandidate(id));
        return affected == 0 ? NotFound() : Ok(new { message = "Candidate deleted." });
    }

    [HttpPost("import")]
    public IActionResult Import(ImportRequest request)
    {
        var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
        var imported = JsonConvert.DeserializeObject<object>(request.Payload, settings);

        return Ok(new { message = "Import parsed.", type = imported?.GetType().FullName });
    }

    [HttpGet("fetch-roster")]
    public async Task<IActionResult> FetchRoster([FromQuery] string url)
    {
        var client = _httpClientFactory.CreateClient();
        var body = await client.GetStringAsync(url);
        return Content(body, "text/plain");
    }
}
