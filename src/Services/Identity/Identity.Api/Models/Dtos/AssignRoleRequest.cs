using System.ComponentModel.DataAnnotations;

namespace Identity.Models.Dtos;

public class AssignRoleRequest
{
    [Required]
    public string RoleName { get; set; } = null!;
}
