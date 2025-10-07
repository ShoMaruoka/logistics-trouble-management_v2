namespace LogisticsTroubleManagement.Models
{
    /// <summary>
    /// インシデントステータス定数
    /// </summary>
    public static class IncidentStatus
    {
        /// <summary>
        /// 2次情報調査中
        /// </summary>
        public const string SecondInfoInvestigation = "2次情報調査中";

        /// <summary>
        /// 2次情報遅延
        /// </summary>
        public const string SecondInfoDelayed = "2次情報遅延";

        /// <summary>
        /// 3次情報調査中
        /// </summary>
        public const string ThirdInfoInvestigation = "3次情報調査中";

        /// <summary>
        /// 3次情報遅延
        /// </summary>
        public const string ThirdInfoDelayed = "3次情報遅延";

        /// <summary>
        /// 完了
        /// </summary>
        public const string Completed = "完了";

        /// <summary>
        /// 全てのステータス
        /// </summary>
        public static readonly string[] All = new[]
        {
            SecondInfoInvestigation,
            SecondInfoDelayed,
            ThirdInfoInvestigation,
            ThirdInfoDelayed,
            Completed
        };
    }
}

