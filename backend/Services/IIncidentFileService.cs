using LogisticsTroubleManagement.DTOs;

namespace LogisticsTroubleManagement.Services
{
    /// <summary>
    /// インシデントファイルサービスのインターフェース
    /// </summary>
    public interface IIncidentFileService
    {
        /// <summary>
        /// インシデントファイル一覧の取得
        /// </summary>
        /// <param name="incidentId">インシデントID</param>
        /// <param name="infoLevel">情報段階（1: 1次情報, 2: 2次情報、nullの場合はすべて）</param>
        /// <returns>ファイル一覧</returns>
        Task<ApiResponseDto<List<IncidentFileResponseDto>>> GetIncidentFilesAsync(int incidentId, int? infoLevel = null);

        /// <summary>
        /// インシデントファイルの追加
        /// </summary>
        /// <param name="incidentId">インシデントID</param>
        /// <param name="createDto">作成DTO</param>
        /// <returns>作成されたファイル</returns>
        Task<ApiResponseDto<IncidentFileResponseDto>> CreateIncidentFileAsync(int incidentId, CreateIncidentFileDto createDto);

        /// <summary>
        /// インシデントファイルの削除
        /// </summary>
        /// <param name="fileId">ファイルID</param>
        /// <returns>削除結果</returns>
        Task<ApiResponseDto<bool>> DeleteIncidentFileAsync(int fileId);
    }
}

