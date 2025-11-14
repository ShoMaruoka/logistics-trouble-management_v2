using System.ComponentModel.DataAnnotations;

namespace LogisticsTroubleManagement.DTOs
{
    /// <summary>
    /// インシデントファイル作成DTO
    /// </summary>
    public class CreateIncidentFileDto
    {
        /// <summary>
        /// 情報段階（1: 1次情報, 2: 2次情報）
        /// </summary>
        [Required]
        [Range(1, 2)]
        public int InfoLevel { get; set; }

        /// <summary>
        /// ファイルデータURI
        /// </summary>
        [Required]
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
    }

    /// <summary>
    /// インシデントファイルレスポンスDTO
    /// </summary>
    public class IncidentFileResponseDto
    {
        /// <summary>
        /// ファイルID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// インシデントID
        /// </summary>
        public int IncidentId { get; set; }

        /// <summary>
        /// 情報段階（1: 1次情報, 2: 2次情報）
        /// </summary>
        public int InfoLevel { get; set; }

        /// <summary>
        /// ファイルデータURI
        /// </summary>
        public string FileDataUri { get; set; } = string.Empty;

        /// <summary>
        /// ファイル名
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// ファイルタイプ（MIMEタイプ）
        /// </summary>
        public string FileType { get; set; } = string.Empty;

        /// <summary>
        /// ファイルサイズ（バイト）
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}

