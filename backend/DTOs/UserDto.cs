using System.ComponentModel.DataAnnotations;

namespace LogisticsTroubleManagement.DTOs
{
    /// <summary>
    /// ユーザー作成DTO
    /// </summary>
    public class CreateUserDto
    {
        /// <summary>
        /// ユーザー名
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;


        /// <summary>
        /// パスワード
        /// </summary>
        [Required]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// 表示名
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// 所属組織
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Organization { get; set; } = string.Empty;

        /// <summary>
        /// ユーザーロールID
        /// </summary>
        [Required]
        public int UserRoleId { get; set; } = 4; // デフォルトは一般ユーザー
    }

    /// <summary>
    /// ユーザー更新DTO
    /// </summary>
    public class UpdateUserDto
    {

        /// <summary>
        /// 表示名
        /// </summary>
        [MaxLength(100)]
        public string? DisplayName { get; set; }

        /// <summary>
        /// 所属組織
        /// </summary>
        [MaxLength(100)]
        public string? Organization { get; set; }

        /// <summary>
        /// ユーザーロールID
        /// </summary>
        public int? UserRoleId { get; set; }

        /// <summary>
        /// アクティブフラグ
        /// </summary>
        public bool? IsActive { get; set; }
    }

    /// <summary>
    /// ユーザー応答DTO
    /// </summary>
    public class UserResponseDto
    {
        /// <summary>
        /// ユーザーID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ユーザー名
        /// </summary>
        public string Username { get; set; } = string.Empty;


        /// <summary>
        /// 表示名
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// 所属組織
        /// </summary>
        public string Organization { get; set; } = string.Empty;

        /// <summary>
        /// 役割
        /// </summary>
        public string Role { get; set; } = string.Empty;

        /// <summary>
        /// アクティブフラグ
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// 最終ログイン日時
        /// </summary>
        public DateTime? LastLoginAt { get; set; }
    }

    /// <summary>
    /// ユーザーロール応答DTO
    /// </summary>
    public class UserRoleResponseDto
    {
        /// <summary>
        /// ロールID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ロール名
        /// </summary>
        public string RoleName { get; set; } = string.Empty;

        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// ログインDTO
    /// </summary>
    public class LoginDto
    {
        /// <summary>
        /// ユーザー名
        /// </summary>
        [Required]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// パスワード
        /// </summary>
        [Required]
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// 認証応答DTO
    /// </summary>
    public class AuthResponseDto
    {
        /// <summary>
        /// アクセストークン
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// トークンタイプ
        /// </summary>
        public string TokenType { get; set; } = "Bearer";

        /// <summary>
        /// 有効期限（秒）
        /// </summary>
        public int ExpiresIn { get; set; }

        /// <summary>
        /// ユーザー情報
        /// </summary>
        public UserResponseDto User { get; set; } = new();
    }

    /// <summary>
    /// パスワード変更DTO
    /// </summary>
    public class ChangePasswordDto
    {
        /// <summary>
        /// 現在のパスワード
        /// </summary>
        [Required]
        public string CurrentPassword { get; set; } = string.Empty;

        /// <summary>
        /// 新しいパスワード
        /// </summary>
        [Required]
        [MinLength(8)]
        public string NewPassword { get; set; } = string.Empty;

        /// <summary>
        /// 新しいパスワード（確認）
        /// </summary>
        [Required]
        [Compare(nameof(NewPassword))]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}

