namespace LogisticsTroubleManagement.Models
{
    /// <summary>
    /// トラブル詳細区分エンティティ
    /// </summary>
    public class TroubleDetailCategory
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
        /// トラブル区分ID
        /// </summary>
        public int TroubleCategoryId { get; set; }

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

        /// <summary>
        /// トラブル区分
        /// </summary>
        public virtual TroubleCategory TroubleCategory { get; set; } = null!;
    }
}
