using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LabsTRVD.Entities
{
    public class RefreshToken
    {
        [Key]
        public Guid RefreshTokenId { get; set; } = Guid.NewGuid();

        [Required]
        public string Token { get; set; } = string.Empty;

        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsRevoked { get; set; } = false;

        [Required]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;
        public bool IsExpired => DateTime.Now >= ExpiresAt;
        public bool IsActive => !IsRevoked && !IsExpired;
    }
}