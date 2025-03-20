namespace DigitalTherapyBackendApp.Api.Features.Auth.Responses
{
    public class CreateUserResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string? ErrorCode { get; set; }
        public UserData Data { get; set; }
    }

    public class UserData
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string? PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public DateTime CreatedAt { get; set; }
        public RoleData Role { get; set; }
    }

    public class RoleData
    {
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
    }
}
