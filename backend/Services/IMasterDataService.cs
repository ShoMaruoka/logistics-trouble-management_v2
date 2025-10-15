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
        Task<ApiResponseDto<MasterDataItemDto[]>> GetOrganizationsAsync();

        /// <summary>
        /// 発生場所一覧の取得
        /// </summary>
        Task<ApiResponseDto<MasterDataItemDto[]>> GetOccurrenceLocationsAsync();

        /// <summary>
        /// 出荷元倉庫一覧の取得
        /// </summary>
        Task<ApiResponseDto<MasterDataItemDto[]>> GetShippingWarehousesAsync();

        /// <summary>
        /// 運送会社一覧の取得
        /// </summary>
        Task<ApiResponseDto<MasterDataItemDto[]>> GetShippingCompaniesAsync();

        /// <summary>
        /// トラブル区分一覧の取得
        /// </summary>
        Task<ApiResponseDto<MasterDataItemDto[]>> GetTroubleCategoriesAsync();

        /// <summary>
        /// トラブル詳細区分一覧の取得
        /// </summary>
        Task<ApiResponseDto<TroubleDetailCategoryItemDto[]>> GetTroubleDetailCategoriesAsync();

        /// <summary>
        /// ユーザー役割一覧の取得
        /// </summary>
        Task<ApiResponseDto<MasterDataItemDto[]>> GetUserRolesAsync();

        /// <summary>
        /// インシデントステータス一覧の取得
        /// </summary>
        Task<ApiResponseDto<string[]>> GetIncidentStatusesAsync();

        /// <summary>
        /// 倉庫一覧の取得
        /// </summary>
        Task<ApiResponseDto<List<MasterDataItemDto>>> GetWarehousesAsync();

        /// <summary>
        /// 単位一覧の取得
        /// </summary>
        Task<ApiResponseDto<List<UnitItemDto>>> GetUnitsAsync();

        /// <summary>
        /// システムパラメータ一覧の取得
        /// </summary>
        Task<ApiResponseDto<List<SystemParameterItemDto>>> GetSystemParametersAsync();

        // =============================================
        // CRUD操作メソッド（システム管理者のみ）
        // =============================================

        /// <summary>
        /// 組織の作成
        /// </summary>
        Task<ApiResponseDto<MasterDataItemDto>> CreateOrganizationAsync(MasterDataCreateDto dto, int userId);

        /// <summary>
        /// 組織の更新
        /// </summary>
        Task<ApiResponseDto<MasterDataItemDto>> UpdateOrganizationAsync(MasterDataUpdateDto dto, int userId);

        /// <summary>
        /// 組織の削除（論理削除）
        /// </summary>
        Task<ApiResponseDto<bool>> DeleteOrganizationAsync(int id, int userId);

        /// <summary>
        /// 発生場所の作成
        /// </summary>
        Task<ApiResponseDto<MasterDataItemDto>> CreateOccurrenceLocationAsync(MasterDataCreateDto dto, int userId);

        /// <summary>
        /// 発生場所の更新
        /// </summary>
        Task<ApiResponseDto<MasterDataItemDto>> UpdateOccurrenceLocationAsync(MasterDataUpdateDto dto, int userId);

        /// <summary>
        /// 発生場所の削除（論理削除）
        /// </summary>
        Task<ApiResponseDto<bool>> DeleteOccurrenceLocationAsync(int id, int userId);

        /// <summary>
        /// 倉庫の作成
        /// </summary>
        Task<ApiResponseDto<MasterDataItemDto>> CreateWarehouseAsync(MasterDataCreateDto dto, int userId);

        /// <summary>
        /// 倉庫の更新
        /// </summary>
        Task<ApiResponseDto<MasterDataItemDto>> UpdateWarehouseAsync(MasterDataUpdateDto dto, int userId);

        /// <summary>
        /// 倉庫の削除（論理削除）
        /// </summary>
        Task<ApiResponseDto<bool>> DeleteWarehouseAsync(int id, int userId);

        /// <summary>
        /// 運送会社の作成
        /// </summary>
        Task<ApiResponseDto<MasterDataItemDto>> CreateShippingCompanyAsync(MasterDataCreateDto dto, int userId);

        /// <summary>
        /// 運送会社の更新
        /// </summary>
        Task<ApiResponseDto<MasterDataItemDto>> UpdateShippingCompanyAsync(MasterDataUpdateDto dto, int userId);

        /// <summary>
        /// 運送会社の削除（論理削除）
        /// </summary>
        Task<ApiResponseDto<bool>> DeleteShippingCompanyAsync(int id, int userId);

        /// <summary>
        /// トラブル区分の作成
        /// </summary>
        Task<ApiResponseDto<MasterDataItemDto>> CreateTroubleCategoryAsync(MasterDataCreateDto dto, int userId);

        /// <summary>
        /// トラブル区分の更新
        /// </summary>
        Task<ApiResponseDto<MasterDataItemDto>> UpdateTroubleCategoryAsync(MasterDataUpdateDto dto, int userId);

        /// <summary>
        /// トラブル区分の削除（論理削除）
        /// </summary>
        Task<ApiResponseDto<bool>> DeleteTroubleCategoryAsync(int id, int userId);

        /// <summary>
        /// トラブル詳細区分の作成
        /// </summary>
        Task<ApiResponseDto<MasterDataItemDto>> CreateTroubleDetailCategoryAsync(TroubleDetailCategoryCreateDto dto, int userId);

        /// <summary>
        /// トラブル詳細区分の更新
        /// </summary>
        Task<ApiResponseDto<MasterDataItemDto>> UpdateTroubleDetailCategoryAsync(TroubleDetailCategoryUpdateDto dto, int userId);

        /// <summary>
        /// トラブル詳細区分の削除（論理削除）
        /// </summary>
        Task<ApiResponseDto<bool>> DeleteTroubleDetailCategoryAsync(int id, int userId);

        /// <summary>
        /// 単位の作成
        /// </summary>
        Task<ApiResponseDto<MasterDataItemDto>> CreateUnitAsync(UnitCreateDto dto, int userId);

        /// <summary>
        /// 単位の更新
        /// </summary>
        Task<ApiResponseDto<MasterDataItemDto>> UpdateUnitAsync(UnitUpdateDto dto, int userId);

        /// <summary>
        /// 単位の削除（論理削除）
        /// </summary>
        Task<ApiResponseDto<bool>> DeleteUnitAsync(int id, int userId);

        /// <summary>
        /// システムパラメータの作成
        /// </summary>
        Task<ApiResponseDto<MasterDataItemDto>> CreateSystemParameterAsync(SystemParameterCreateDto dto, int userId);

        /// <summary>
        /// システムパラメータの更新
        /// </summary>
        Task<ApiResponseDto<MasterDataItemDto>> UpdateSystemParameterAsync(SystemParameterUpdateDto dto, int userId);

        /// <summary>
        /// システムパラメータの削除（論理削除）
        /// </summary>
        Task<ApiResponseDto<bool>> DeleteSystemParameterAsync(int id, int userId);
    }
}
