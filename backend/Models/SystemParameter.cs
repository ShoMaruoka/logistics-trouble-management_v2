using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticsTroubleManagement.Models
{
    /// <summary>
    /// システムパラメータエンティティ
    /// </summary>
    public class SystemParameter
    {
        /// <summary>
        /// パラメータID
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// パラメータ名
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// パラメータキー
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string ParameterKey { get; set; } = string.Empty;

        /// <summary>
        /// パラメータ値
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string ParameterValue { get; set; } = string.Empty;

        /// <summary>
        /// 説明
        /// </summary>
        [MaxLength(1000)]
        public string? Description { get; set; }

        /// <summary>
        /// データ型
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string DataType { get; set; } = string.Empty;

        /// <summary>
        /// 有効フラグ
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 作成者ID
        /// </summary>
        public int? CreatedBy { get; set; }

        /// <summary>
        /// 更新者ID
        /// </summary>
        public int? UpdatedBy { get; set; }

        // ナビゲーションプロパティ
        [ForeignKey("CreatedBy")]
        public virtual User? CreatedByUser { get; set; }

        [ForeignKey("UpdatedBy")]
        public virtual User? UpdatedByUser { get; set; }
    }
}
