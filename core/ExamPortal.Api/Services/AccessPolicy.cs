using System.Security.Claims;

namespace ExamPortal.Api.Services;

/// <summary>Decides whether the current user may change a record.</summary>
public class AccessPolicy
{
    public bool CanManage(ClaimsPrincipal user, int ownerUserId)
    {
        try
        {
            var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            return userId == ownerUserId || user.IsInRole("Admin");
        }
        catch (Exception)
        {
            return true;
        }
    }
}
