namespace LogisticsTroubleManagement.Models
{
    /// <summary>
    /// マスタデータ定数
    /// </summary>
    public static class MasterData
    {
        /// <summary>
        /// 組織一覧
        /// </summary>
        public static readonly string[] Organizations = new[]
        {
            "本社A",
            "本社B",
            "東日本",
            "西日本"
        };

        /// <summary>
        /// 発生場所一覧
        /// </summary>
        public static readonly string[] OccurrenceLocations = new[]
        {
            "倉庫（入荷作業）",
            "倉庫（格納作業）",
            "倉庫（出荷作業）",
            "配送（集荷/配達）",
            "配送（施設内）",
            "お客様先"
        };

        /// <summary>
        /// 出荷元倉庫一覧
        /// </summary>
        public static readonly string[] ShippingWarehouses = new[]
        {
            "札幌倉庫",
            "東京倉庫",
            "埼玉倉庫",
            "横浜倉庫",
            "大阪倉庫",
            "神戸倉庫",
            "松山倉庫"
        };

        /// <summary>
        /// 運送会社一覧
        /// </summary>
        public static readonly string[] ShippingCompanies = new[]
        {
            "ヤマト運輸",
            "佐川急便",
            "福山通運",
            "西濃運輸",
            "チャーター",
            "その他輸送会社"
        };

        /// <summary>
        /// トラブル区分一覧
        /// </summary>
        public static readonly string[] TroubleCategories = new[]
        {
            "荷役トラブル",
            "配送トラブル"
        };

        /// <summary>
        /// トラブル詳細区分一覧
        /// </summary>
        public static readonly string[] TroubleDetailCategories = new[]
        {
            "商品間違い",
            "数量過不足",
            "送付先間違い",
            "発送漏れ",
            "破損・汚損",
            "紛失",
            "その他の商品事故"
        };

        /// <summary>
        /// ユーザー役割一覧
        /// </summary>
        public static readonly string[] UserRoles = new[]
        {
            "システム管理者",
            "部門管理者",
            "倉庫管理者",
            "一般ユーザー"
        };
    }
}

