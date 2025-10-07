using System.ComponentModel.DataAnnotations;

namespace LogisticsTroubleManagement.DTOs
{
    /// <summary>
    /// インシデント作成DTO
    /// </summary>
    public class CreateIncidentDto
    {
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
    }

    /// <summary>
    /// インシデント更新DTO
    /// </summary>
    public class UpdateIncidentDto
    {
        /// <summary>
        /// 作成日
        /// </summary>
        public DateTime? CreationDate { get; set; }

        /// <summary>
        /// 部門ID
        /// </summary>
        public int? Organization { get; set; }

        /// <summary>
        /// 作成者ID
        /// </summary>
        public int? Creator { get; set; }

        /// <summary>
        /// 発生日時
        /// </summary>
        public DateTime? OccurrenceDateTime { get; set; }

        /// <summary>
        /// 発生場所ID
        /// </summary>
        public int? OccurrenceLocation { get; set; }

        /// <summary>
        /// 倉庫ID
        /// </summary>
        public int? ShippingWarehouse { get; set; }

        /// <summary>
        /// 運送会社ID
        /// </summary>
        public int? ShippingCompany { get; set; }

        /// <summary>
        /// トラブル区分ID
        /// </summary>
        public int? TroubleCategory { get; set; }

        /// <summary>
        /// トラブル詳細区分ID
        /// </summary>
        public int? TroubleDetailCategory { get; set; }

        /// <summary>
        /// 内容詳細
        /// </summary>
        [MaxLength(2000)]
        public string? Details { get; set; }

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
    }

    /// <summary>
    /// インシデント応答DTO
    /// </summary>
    public class IncidentResponseDto
    {
        /// <summary>
        /// インシデントID
        /// </summary>
        public int Id { get; set; }

        // 1次情報
        /// <summary>
        /// 作成日
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// 部門ID
        /// </summary>
        public int Organization { get; set; }

        /// <summary>
        /// 作成者ID
        /// </summary>
        public int Creator { get; set; }

        /// <summary>
        /// 発生日時
        /// </summary>
        public DateTime OccurrenceDateTime { get; set; }

        /// <summary>
        /// 発生場所ID
        /// </summary>
        public int OccurrenceLocation { get; set; }

        /// <summary>
        /// 倉庫ID
        /// </summary>
        public int ShippingWarehouse { get; set; }

        /// <summary>
        /// 運送会社ID
        /// </summary>
        public int ShippingCompany { get; set; }

        /// <summary>
        /// トラブル区分ID
        /// </summary>
        public int TroubleCategory { get; set; }

        /// <summary>
        /// トラブル詳細区分ID
        /// </summary>
        public int TroubleDetailCategory { get; set; }

        /// <summary>
        /// 内容詳細
        /// </summary>
        public string Details { get; set; } = string.Empty;

        /// <summary>
        /// 伝票番号
        /// </summary>
        public string? VoucherNumber { get; set; }

        /// <summary>
        /// 得意先コード
        /// </summary>
        public string? CustomerCode { get; set; }

        /// <summary>
        /// 商品コード
        /// </summary>
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
        public string? ProcessDescription { get; set; }

        /// <summary>
        /// 発生原因
        /// </summary>
        public string? Cause { get; set; }

        /// <summary>
        /// 写真データURI
        /// </summary>
        public string? PhotoDataUri { get; set; }

        // 3次情報
        /// <summary>
        /// 3次情報入力日
        /// </summary>
        public DateTime? InputDate3 { get; set; }

        /// <summary>
        /// 再発防止策
        /// </summary>
        public string? RecurrencePreventionMeasures { get; set; }

        // その他
        /// <summary>
        /// ステータス
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// インシデント検索DTO
    /// </summary>
    public class IncidentSearchDto
    {
        /// <summary>
        /// ページ番号
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// 1ページあたりの件数
        /// </summary>
        public int Limit { get; set; } = 20;

        /// <summary>
        /// 検索キーワード
        /// </summary>
        public string? Search { get; set; }

        /// <summary>
        /// 年
        /// </summary>
        public int? Year { get; set; }

        /// <summary>
        /// 月
        /// </summary>
        public int? Month { get; set; }

        /// <summary>
        /// 出荷元倉庫ID
        /// </summary>
        public int? Warehouse { get; set; }

        /// <summary>
        /// ステータス
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// トラブル区分ID
        /// </summary>
        public int? TroubleCategory { get; set; }
    }

    /// <summary>
    /// インシデント一覧応答DTO
    /// </summary>
    public class IncidentListResponseDto
    {
        /// <summary>
        /// インシデント一覧
        /// </summary>
        public List<IncidentResponseDto> Incidents { get; set; } = new();

        /// <summary>
        /// 総件数
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// ページ番号
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// 1ページあたりの件数
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// 総ページ数
        /// </summary>
        public int TotalPages => (int)Math.Ceiling((double)Total / Limit);
    }
}

