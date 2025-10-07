using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticsTroubleManagement.Models
{
    /// <summary>
    /// インシデント（物流トラブル）エンティティ
    /// </summary>
    public class Incident
    {
        /// <summary>
        /// インシデントID
        /// </summary>
        [Key]
        public int Id { get; set; }

        // 1次情報
        /// <summary>
        /// 作成日
        /// </summary>
        [Required]
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// 部門ID
        /// </summary>
        [Required]
        public int Organization { get; set; }

        /// <summary>
        /// 作成者ID
        /// </summary>
        [Required]
        public int Creator { get; set; }

        /// <summary>
        /// 発生日時
        /// </summary>
        [Required]
        public DateTime OccurrenceDateTime { get; set; }

        /// <summary>
        /// 発生場所ID
        /// </summary>
        [Required]
        public int OccurrenceLocation { get; set; }

        /// <summary>
        /// 倉庫ID
        /// </summary>
        [Required]
        public int ShippingWarehouse { get; set; }

        /// <summary>
        /// 運送会社ID
        /// </summary>
        [Required]
        public int ShippingCompany { get; set; }

        /// <summary>
        /// トラブル区分ID
        /// </summary>
        [Required]
        public int TroubleCategory { get; set; }

        /// <summary>
        /// トラブル詳細区分ID
        /// </summary>
        [Required]
        public int TroubleDetailCategory { get; set; }

        /// <summary>
        /// 内容詳細
        /// </summary>
        [Required]
        [MaxLength(2000)]
        public string Details { get; set; } = string.Empty;

        /// <summary>
        /// 伝票番号
        /// </summary>
        [MaxLength(50)]
        public string? VoucherNumber { get; set; }

        /// <summary>
        /// 得意先コード
        /// </summary>
        [MaxLength(50)]
        public string? CustomerCode { get; set; }

        /// <summary>
        /// 商品コード
        /// </summary>
        [MaxLength(50)]
        public string? ProductCode { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public decimal? Quantity { get; set; }

        /// <summary>
        /// 単位ID
        /// </summary>
        public int? Unit { get; set; }

        // 2次情報
        /// <summary>
        /// 2次情報入力日
        /// </summary>
        public DateTime? InputDate { get; set; }

        /// <summary>
        /// 発生経緯
        /// </summary>
        [MaxLength(2000)]
        public string? ProcessDescription { get; set; }

        /// <summary>
        /// 発生原因
        /// </summary>
        [MaxLength(2000)]
        public string? Cause { get; set; }

        /// <summary>
        /// 写真データURI
        /// </summary>
        [MaxLength(500)]
        public string? PhotoDataUri { get; set; }

        // 3次情報
        /// <summary>
        /// 3次情報入力日
        /// </summary>
        public DateTime? InputDate3 { get; set; }

        /// <summary>
        /// 再発防止策
        /// </summary>
        [MaxLength(2000)]
        public string? RecurrencePreventionMeasures { get; set; }

        // その他
        /// <summary>
        /// ステータス
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "2次情報調査中";

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
        /// <summary>
        /// 作成者
        /// </summary>
        [ForeignKey("CreatedBy")]
        public virtual User? CreatedByUser { get; set; }

        /// <summary>
        /// 更新者
        /// </summary>
        [ForeignKey("UpdatedBy")]
        public virtual User? UpdatedByUser { get; set; }
    }
}

