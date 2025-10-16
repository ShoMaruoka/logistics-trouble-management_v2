using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticsTroubleManagement.Models
{
    /// <summary>
    /// ユーザーエンティティ
    /// </summary>
    public class User
    {
        /// <summary>
        /// ユーザーID
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// ユーザー名
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;


        /// <summary>
        /// パスワード（平文）
        /// </summary>
        [MaxLength(20)]
        public string? Password { get; set; }

        /// <summary>
        /// パスワードハッシュ
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// 表示名
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// 部門ID
        /// </summary>
        public int? OrganizationId { get; set; }

        /// <summary>
        /// デフォルト倉庫ID
        /// </summary>
        public int? DefaultWarehouseId { get; set; }

        /// <summary>
        /// ユーザーロールID
        /// </summary>
        [Required]
        public int UserRoleId { get; set; }

        /// <summary>
        /// アクティブフラグ
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// 最終ログイン日時（データベースに存在しないため無視）
        /// </summary>
        [NotMapped]
        public DateTime? LastLoginAt { get; set; }

        // ナビゲーションプロパティ
        /// <summary>
        /// ユーザーロール
        /// </summary>
        public virtual UserRole UserRole { get; set; } = null!;

        /// <summary>
        /// 作成したインシデント
        /// </summary>
        public virtual ICollection<Incident> CreatedIncidents { get; set; } = new List<Incident>();

        /// <summary>
        /// 更新したインシデント
        /// </summary>
        public virtual ICollection<Incident> UpdatedIncidents { get; set; } = new List<Incident>();
    }
}

