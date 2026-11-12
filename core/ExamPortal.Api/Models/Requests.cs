namespace ExamPortal.Api.Models;

public class LoginRequest
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
}

public class ResetRequest
{
    public string Username { get; set; } = "";
}

public class ResetConfirmRequest
{
    public string Username { get; set; } = "";
    public string Token { get; set; } = "";
    public string NewPassword { get; set; } = "";
}

public class RegistrationRequest
{
    public int CandidateId { get; set; }
    public string ExamCode { get; set; } = "";
    public string TestCenter { get; set; } = "";
    public int Seats { get; set; }
    public string ScheduledOn { get; set; } = "";
}

public class ImportRequest
{
    public string Payload { get; set; } = "";
}

public class NotesUpdateRequest
{
    public string? Notes { get; set; }
}

public class ProfileUpdateRequest
{
    public string? Username { get; set; }
    public string? Role { get; set; }
}
