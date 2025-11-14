using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticsTroubleManagement.Models
{
    /// <summary>
    /// ユーザーロールエンティティ
    /// </summary>
    public class UserRole
    {
        /// <summary>
        /// ロールID
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// ロール名
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string RoleName { get; set; } = string.Empty;

        /// <summary>
        /// 表示順
        /// </summary>
        public int DisplayOrder { get; set; } = 0;

        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime CreatedAt { get; set; }

        // ナビゲーションプロパティ
        /// <summary>
        /// このロールを持つユーザー
        /// </summary>
        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}
