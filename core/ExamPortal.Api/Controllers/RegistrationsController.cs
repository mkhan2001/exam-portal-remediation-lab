using System.Security.Claims;
using ExamPortal.Api.Data;
using ExamPortal.Api.Models;
using ExamPortal.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExamPortal.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/registrations")]
public class RegistrationsController : ControllerBase
{
    private readonly QueryExecutor _executor;
    private readonly LinkProtector _linkProtector;

    public RegistrationsController(QueryExecutor executor, LinkProtector linkProtector)
    {
        _executor = executor;
        _linkProtector = linkProtector;
    }

    [HttpGet("{id:int}/report-link")]
    public IActionResult ReportLink(int id)
    {
        var token = _linkProtector.Protect("registration:" + id);
        return Ok(new { url = "/reports/download?token=" + Uri.EscapeDataString(token) });
    }

    [HttpGet("{id:int}")]
    public IActionResult Get(int id)
    {
        var rows = _executor.Read(Queries.GetRegistration(id));
        return rows.Count == 0 ? NotFound() : Ok(rows[0]);
    }

    [HttpGet("by-center")]
    public IActionResult ByTestCenter([FromQuery] string testCenter, [FromQuery] string sort = "ScheduledOn")
    {
        return Ok(_executor.Read(Queries.RegistrationsByTestCenter(testCenter, sort)));
    }

    [HttpPost]
    public IActionResult Create(RegistrationRequest request)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        _executor.Execute(Queries.InsertRegistration(
            userId,
            request.CandidateId,
            request.ExamCode,
            request.TestCenter,
            request.Seats,
            request.ScheduledOn));

        return Ok(new { message = "Registration created." });
    }
}
