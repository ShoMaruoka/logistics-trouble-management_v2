using LogisticsTroubleManagement.DTOs;

namespace LogisticsTroubleManagement.Services
{
    /// <summary>
    /// マスタデータサービスのインターフェース
    /// </summary>
    public interface IMasterDataService
    {
        /// <summary>
        /// 組織一覧の取得
        /// </summary>
        Task<ApiResponseDto<string[]>> GetOrganizationsAsync();

        /// <summary>
        /// 発生場所一覧の取得
        /// </summary>
        Task<ApiResponseDto<string[]>> GetOccurrenceLocationsAsync();

        /// <summary>
        /// 出荷元倉庫一覧の取得
        /// </summary>
        Task<ApiResponseDto<string[]>> GetShippingWarehousesAsync();

        /// <summary>
        /// 運送会社一覧の取得
        /// </summary>
        Task<ApiResponseDto<string[]>> GetShippingCompaniesAsync();

        /// <summary>
        /// トラブル区分一覧の取得
        /// </summary>
        Task<ApiResponseDto<string[]>> GetTroubleCategoriesAsync();

        /// <summary>
        /// トラブル詳細区分一覧の取得
        /// </summary>
        Task<ApiResponseDto<string[]>> GetTroubleDetailCategoriesAsync();

        /// <summary>
        /// ユーザー役割一覧の取得
        /// </summary>
        Task<ApiResponseDto<string[]>> GetUserRolesAsync();

        /// <summary>
        /// インシデントステータス一覧の取得
        /// </summary>
        Task<ApiResponseDto<string[]>> GetIncidentStatusesAsync();
    }
}
