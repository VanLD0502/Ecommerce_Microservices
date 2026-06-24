using System.ComponentModel.DataAnnotations;

namespace Identity.Models.Dtos;

public class ChangePasswordRequest
{
    [Required]
    public string OldPassword { get; set; } = null!;

    [Required]
    [MinLength(6)]
    public string NewPassword { get; set; } = null!;
}
