using ExamPortal.Api.Data;
using ExamPortal.Api.Models;
using ExamPortal.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExamPortal.Api.Controllers;

[ApiController]
[Route("api/candidates")]
public class CandidatesController : ControllerBase
{
    private readonly QueryExecutor _executor;
    private readonly AccessPolicy _accessPolicy;

    public CandidatesController(QueryExecutor executor, AccessPolicy accessPolicy)
    {
        _executor = executor;
        _accessPolicy = accessPolicy;
    }

    [HttpGet]
    public IActionResult Search([FromQuery] string? search)
    {
        return Ok(_executor.Read(Queries.SearchCandidates(search)));
    }

    [HttpGet("by-email")]
    public IActionResult SearchByEmail([FromQuery] string email)
    {
        return Ok(_executor.Read(Queries.SearchCandidatesByEmail(email)));
    }

    [HttpGet("{id:int}")]
    public IActionResult Get(int id)
    {
        var rows = _executor.Read(Queries.GetCandidate(id));
        return rows.Count == 0 ? NotFound() : Ok(rows[0]);
    }

    [HttpPut("{id:int}/notes")]
    public IActionResult UpdateNotes(int id, NotesUpdateRequest request)
    {
        var rows = _executor.Read(Queries.GetCandidate(id));
        if (rows.Count == 0)
        {
            return NotFound();
        }

        var ownerUserId = Convert.ToInt32(rows[0]["OwnerUserId"]);
        if (!_accessPolicy.CanManage(User, ownerUserId))
        {
            return Forbid();
        }

        _executor.Execute(Queries.UpdateCandidateNotes(id, request.Notes));
        return Ok(new { message = "Notes updated." });
    }
}
