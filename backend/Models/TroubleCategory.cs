namespace LogisticsTroubleManagement.Models
{
    /// <summary>
    /// トラブル区分エンティティ
    /// </summary>
    public class TroubleCategory
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

        /// <summary>
        /// トラブル詳細区分一覧
        /// </summary>
        public virtual ICollection<TroubleDetailCategory> TroubleDetailCategories { get; set; } = new List<TroubleDetailCategory>();
    }
}
