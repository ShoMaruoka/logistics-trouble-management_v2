using LogisticsTroubleManagement.DTOs;
using LogisticsTroubleManagement.Models;

namespace LogisticsTroubleManagement.Services
{
    /// <summary>
    /// インシデントサービスのインターフェース
    /// </summary>
    public interface IIncidentService
    {
        /// <summary>
        /// インシデント一覧の取得
        /// </summary>
        /// <param name="searchDto">検索条件</param>
        /// <returns>インシデント一覧</returns>
        Task<PagedApiResponseDto<IncidentResponseDto>> GetIncidentsAsync(IncidentSearchDto searchDto);

        /// <summary>
        /// インシデントの取得
        /// </summary>
        /// <param name="id">インシデントID</param>
        /// <returns>インシデント</returns>
        Task<ApiResponseDto<IncidentResponseDto>> GetIncidentAsync(int id);

        /// <summary>
        /// インシデントの作成
        /// </summary>
        /// <param name="createDto">作成DTO</param>
        /// <param name="userId">作成者ID</param>
        /// <returns>作成されたインシデント</returns>
        Task<ApiResponseDto<IncidentResponseDto>> CreateIncidentAsync(CreateIncidentDto createDto, int userId);

        /// <summary>
        /// インシデントの更新
        /// </summary>
        /// <param name="id">インシデントID</param>
        /// <param name="updateDto">更新DTO</param>
        /// <param name="userId">更新者ID</param>
        /// <returns>更新されたインシデント</returns>
        Task<ApiResponseDto<IncidentResponseDto>> UpdateIncidentAsync(int id, UpdateIncidentDto updateDto, int userId);

        /// <summary>
        /// インシデントの削除
        /// </summary>
        /// <param name="id">インシデントID</param>
        /// <returns>削除結果</returns>
        Task<ApiResponseDto<bool>> DeleteIncidentAsync(int id);

        /// <summary>
        /// インシデントのステータス更新
        /// </summary>
        /// <param name="id">インシデントID</param>
        /// <param name="status">新しいステータス</param>
        /// <param name="userId">更新者ID</param>
        /// <returns>更新結果</returns>
        Task<ApiResponseDto<bool>> UpdateIncidentStatusAsync(int id, string status, int userId);

        /// <summary>
        /// インシデントのCSV出力
        /// </summary>
        /// <param name="searchDto">検索条件</param>
        /// <returns>CSVデータ</returns>
        Task<ApiResponseDto<byte[]>> ExportIncidentsToCsvAsync(IncidentSearchDto searchDto);

        /// <summary>
        /// ダッシュボード統計の取得
        /// </summary>
        /// <returns>統計情報</returns>
        Task<ApiResponseDto<DashboardStatsDto>> GetDashboardStatsAsync();
    }
}

