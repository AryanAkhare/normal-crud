using System.ComponentModel.DataAnnotations;

namespace UserManagementApi.Models;

public sealed class CreateUserRequest
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(254)]
    public string Email { get; init; } = string.Empty;
}
