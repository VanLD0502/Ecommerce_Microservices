namespace Identity.Models.Dtos;

public class RegisterResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public long? UserId { get; set; }
}
