using Microsoft.AspNetCore.Identity;

namespace Account.API.Models;

public class MemberRole : IdentityRole<Guid>
{
    public const string Parent = "Parent";
    public const string Child = "Child";
}