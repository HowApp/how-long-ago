namespace How.Core.DTO.Identity;

using System.ComponentModel.DataAnnotations;

public sealed class LoginRequestDTO
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
    public bool RememberMe { get; set; }
}