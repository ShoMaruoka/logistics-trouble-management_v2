using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticsTroubleManagement.Models
{
    /// <summary>
    /// インシデントファイルエンティティ
    /// </summary>
    public class IncidentFile
    {
        /// <summary>
        /// ファイルID
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// インシデントID
        /// </summary>
        [Required]
        public int IncidentId { get; set; }

        /// <summary>
        /// 情報段階（1: 1次情報, 2: 2次情報）
        /// </summary>
        [Required]
        public int InfoLevel { get; set; }

        /// <summary>
        /// ファイルデータURI
        /// </summary>
        [Required]
        [Column(TypeName = "NVARCHAR(MAX)")]
        public string FileDataUri { get; set; } = string.Empty;

        /// <summary>
        /// ファイル名
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// ファイルタイプ（MIMEタイプ）
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string FileType { get; set; } = string.Empty;

        /// <summary>
        /// ファイルサイズ（バイト）
        /// </summary>
        [Required]
        public long FileSize { get; set; }

        /// <summary>
        /// 作成日時
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 更新日時
        /// </summary>
        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// ナビゲーションプロパティ: インシデント
        /// </summary>
        [ForeignKey("IncidentId")]
        public Incident? Incident { get; set; }
    }
}

