using LogisticsTroubleManagement.Data;
using LogisticsTroubleManagement.DTOs;
using LogisticsTroubleManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace LogisticsTroubleManagement.Services
{
    /// <summary>
    /// インシデントファイルサービスの実装
    /// </summary>
    public class IncidentFileService : IIncidentFileService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<IncidentFileService> _logger;

        public IncidentFileService(
            ApplicationDbContext context,
            ILogger<IncidentFileService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// インシデントファイル一覧の取得
        /// </summary>
        public async Task<ApiResponseDto<List<IncidentFileResponseDto>>> GetIncidentFilesAsync(int incidentId, int? infoLevel = null)
        {
            try
            {
                var query = _context.IncidentFiles
                    .Where(f => f.IncidentId == incidentId);

                if (infoLevel.HasValue)
                {
                    query = query.Where(f => f.InfoLevel == infoLevel.Value);
                }

                var files = await query
                    .OrderBy(f => f.CreatedAt)
                    .ToListAsync();

                var fileDtos = files.Select(f => new IncidentFileResponseDto
                {
                    Id = f.Id,
                    IncidentId = f.IncidentId,
                    InfoLevel = f.InfoLevel,
                    FileDataUri = f.FileDataUri,
                    FileName = f.FileName,
                    FileType = f.FileType,
                    FileSize = f.FileSize,
                    CreatedAt = f.CreatedAt,
                    UpdatedAt = f.UpdatedAt
                }).ToList();

                return ApiResponseDto<List<IncidentFileResponseDto>>.SuccessResponse(fileDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "インシデントファイル一覧の取得中にエラーが発生しました: {IncidentId}, {InfoLevel}", 
                    incidentId, infoLevel);
                return ApiResponseDto<List<IncidentFileResponseDto>>.ErrorResponse("ファイル一覧の取得に失敗しました");
            }
        }

        /// <summary>
        /// インシデントファイルの追加
        /// </summary>
        public async Task<ApiResponseDto<IncidentFileResponseDto>> CreateIncidentFileAsync(int incidentId, CreateIncidentFileDto createDto)
        {
            try
            {
                // インシデントの存在確認
                var incident = await _context.Incidents.FindAsync(incidentId);
                if (incident == null)
                {
                    return ApiResponseDto<IncidentFileResponseDto>.ErrorResponse("インシデントが見つかりません");
                }

                var file = new IncidentFile
                {
                    IncidentId = incidentId,
                    InfoLevel = createDto.InfoLevel,
                    FileDataUri = createDto.FileDataUri,
                    FileName = createDto.FileName,
                    FileType = createDto.FileType,
                    FileSize = createDto.FileSize,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.IncidentFiles.Add(file);
                await _context.SaveChangesAsync();

                var fileDto = new IncidentFileResponseDto
                {
                    Id = file.Id,
                    IncidentId = file.IncidentId,
                    InfoLevel = file.InfoLevel,
                    FileDataUri = file.FileDataUri,
                    FileName = file.FileName,
                    FileType = file.FileType,
                    FileSize = file.FileSize,
                    CreatedAt = file.CreatedAt,
                    UpdatedAt = file.UpdatedAt
                };

                return ApiResponseDto<IncidentFileResponseDto>.SuccessResponse(fileDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "インシデントファイルの追加中にエラーが発生しました: {IncidentId}", incidentId);
                return ApiResponseDto<IncidentFileResponseDto>.ErrorResponse("ファイルの追加に失敗しました");
            }
        }

        /// <summary>
        /// インシデントファイルの削除
        /// </summary>
        public async Task<ApiResponseDto<bool>> DeleteIncidentFileAsync(int fileId)
        {
            try
            {
                var file = await _context.IncidentFiles.FindAsync(fileId);
                if (file == null)
                {
                    return ApiResponseDto<bool>.ErrorResponse("ファイルが見つかりません");
                }

                _context.IncidentFiles.Remove(file);
                await _context.SaveChangesAsync();

                return ApiResponseDto<bool>.SuccessResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "インシデントファイルの削除中にエラーが発生しました: {FileId}", fileId);
                return ApiResponseDto<bool>.ErrorResponse("ファイルの削除に失敗しました");
            }
        }
    }
}

