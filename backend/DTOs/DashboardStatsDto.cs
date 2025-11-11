namespace LogisticsTroubleManagement.DTOs
{
    /// <summary>
    /// ダッシュボード統計DTO
    /// </summary>
    public class DashboardStatsDto
    {
        /// <summary>
        /// トラブル総件数
        /// </summary>
        public int TotalIncidents { get; set; }

        /// <summary>
        /// 完了件数
        /// </summary>
        public int CompletedIncidents { get; set; }

        /// <summary>
        /// 進捗率
        /// </summary>
        public double ProgressRate => TotalIncidents > 0 ? (double)CompletedIncidents / TotalIncidents * 100 : 0;

        /// <summary>
        /// 2次情報遅延件数
        /// </summary>
        public int SecondInfoDelayedCount { get; set; }

        /// <summary>
        /// 3次情報遅延件数
        /// </summary>
        public int ThirdInfoDelayedCount { get; set; }

        /// <summary>
        /// 日別発生件数（過去30日）- 発生日時（OccurrenceDateTime）を基準に集計
        /// </summary>
        public List<DailyIncidentCountDto> DailyIncidentCounts { get; set; } = new();

        /// <summary>
        /// 出荷元倉庫別件数
        /// </summary>
        public List<WarehouseIncidentCountDto> WarehouseIncidentCounts { get; set; } = new();

        /// <summary>
        /// トラブル区分別件数
        /// </summary>
        public List<TroubleCategoryCountDto> TroubleCategoryCounts { get; set; } = new();

        /// <summary>
        /// トラブル詳細区分別件数
        /// </summary>
        public List<TroubleDetailCategoryCountDto> TroubleDetailCategoryCounts { get; set; } = new();

        /// <summary>
        /// 運送会社別件数
        /// </summary>
        public List<ShippingCompanyCountDto> ShippingCompanyCounts { get; set; } = new();

        /// <summary>
        /// ステータス別件数
        /// </summary>
        public List<StatusCountDto> StatusCounts { get; set; } = new();
    }

    /// <summary>
    /// 日別インシデント件数DTO
    /// </summary>
    public class DailyIncidentCountDto
    {
        /// <summary>
        /// 日付
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// 件数
        /// </summary>
        public int Count { get; set; }
    }

    /// <summary>
    /// 倉庫別インシデント件数DTO
    /// </summary>
    public class WarehouseIncidentCountDto
    {
        /// <summary>
        /// 倉庫名
        /// </summary>
        public string Warehouse { get; set; } = string.Empty;

        /// <summary>
        /// 件数
        /// </summary>
        public int Count { get; set; }
    }

    /// <summary>
    /// トラブル区分別件数DTO
    /// </summary>
    public class TroubleCategoryCountDto
    {
        /// <summary>
        /// トラブル区分
        /// </summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// 件数
        /// </summary>
        public int Count { get; set; }
    }

    /// <summary>
    /// トラブル詳細区分別件数DTO
    /// </summary>
    public class TroubleDetailCategoryCountDto
    {
        /// <summary>
        /// トラブル詳細区分
        /// </summary>
        public string DetailCategory { get; set; } = string.Empty;

        /// <summary>
        /// 件数
        /// </summary>
        public int Count { get; set; }
    }

    /// <summary>
    /// 運送会社別件数DTO
    /// </summary>
    public class ShippingCompanyCountDto
    {
        /// <summary>
        /// 運送会社名
        /// </summary>
        public string Company { get; set; } = string.Empty;

        /// <summary>
        /// 件数
        /// </summary>
        public int Count { get; set; }
    }

    /// <summary>
    /// ステータス別件数DTO
    /// </summary>
    public class StatusCountDto
    {
        /// <summary>
        /// ステータス
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// 件数
        /// </summary>
        public int Count { get; set; }
    }
}

