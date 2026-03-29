namespace LabsTRVD.DTOs.AuthDTOs
{
    public class AuthResultDto
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime AccessTokenExpires { get; set; }         
        public DateTime RefreshTokenExpires { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
