namespace How.Shared.DTO.Auth;

using System.ComponentModel.DataAnnotations;

public sealed class RegisterRequestDTO
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    public string UserName { get; set; }
    [Required]
    public string Password { get; set; }
    [Compare(nameof(Password), ErrorMessage = "Password do not match!")]
    public string PasswordConfirm { get; set; }
}