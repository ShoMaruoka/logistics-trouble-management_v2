namespace LogisticsTroubleManagement.Models
{
    /// <summary>
    /// 発生場所エンティティ
    /// </summary>
    public class OccurrenceLocation
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 有効フラグ
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
    }
}
