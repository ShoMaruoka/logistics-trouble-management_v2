using LogisticsTroubleManagement.DTOs;
using LogisticsTroubleManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogisticsTroubleManagement.Controllers
{
    /// <summary>
    /// インシデントファイルコントローラー
    /// </summary>
    [ApiController]
    [Route("api/incidents/{incidentId}/files")]
    [Authorize]
    public class IncidentFilesController : ControllerBase
    {
        private readonly IIncidentFileService _fileService;
        private readonly ILogger<IncidentFilesController> _logger;

        public IncidentFilesController(
            IIncidentFileService fileService,
            ILogger<IncidentFilesController> logger)
        {
            _fileService = fileService;
            _logger = logger;
        }

        /// <summary>
        /// インシデントファイル一覧の取得
        /// </summary>
        /// <param name="incidentId">インシデントID</param>
        /// <param name="infoLevel">情報段階（1: 1次情報, 2: 2次情報、省略時はすべて）</param>
        /// <returns>ファイル一覧</returns>
        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<List<IncidentFileResponseDto>>>> GetIncidentFiles(
            int incidentId,
            [FromQuery] int? infoLevel = null)
        {
            var result = await _fileService.GetIncidentFilesAsync(incidentId, infoLevel);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// インシデントファイルの追加（multipart/form-data形式）
        /// </summary>
        /// <param name="incidentId">インシデントID</param>
        /// <param name="file">アップロードファイル</param>
        /// <param name="infoLevel">情報段階（1: 1次情報, 2: 2次情報）</param>
        /// <returns>作成されたファイル</returns>
        [HttpPost("upload")]
        [RequestFormLimits(MultipartBodyLengthLimit = 50_485_760)] // 50MB制限（Base64エンコードを考慮）
        public async Task<ActionResult<ApiResponseDto<IncidentFileResponseDto>>> CreateIncidentFileMultipart(
            int incidentId,
            IFormFile file,
            [FromForm] int infoLevel)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(ApiResponseDto<IncidentFileResponseDto>.ErrorResponse("ファイルが指定されていません"));
            }

            // ファイルサイズチェック（10MB制限）
            const long maxFileSize = 10_485_760; // 10MB
            if (file.Length > maxFileSize)
            {
                return BadRequest(ApiResponseDto<IncidentFileResponseDto>.ErrorResponse(
                    $"ファイルサイズが大きすぎます。最大サイズ: {maxFileSize / (1024 * 1024)}MB"));
            }

            // ファイルタイプチェック
            var allowedMimeTypes = new[] { "image/png", "image/jpeg", "image/jpg", "image/gif", "image/webp", "application/pdf" };
            if (!allowedMimeTypes.Contains(file.ContentType.ToLower()))
            {
                return BadRequest(ApiResponseDto<IncidentFileResponseDto>.ErrorResponse(
                    $"許可されていないファイルタイプです: {file.ContentType}"));
            }

            // ファイルをBase64に変換してData URI形式に変換
            byte[] fileBytes;
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                fileBytes = memoryStream.ToArray();
            }

            var base64String = Convert.ToBase64String(fileBytes);
            var dataUri = $"data:{file.ContentType};base64,{base64String}";

            var createDto = new CreateIncidentFileDto
            {
                InfoLevel = infoLevel,
                FileDataUri = dataUri,
                FileName = file.FileName,
                FileType = file.ContentType,
                FileSize = file.Length
            };

            var result = await _fileService.CreateIncidentFileAsync(incidentId, createDto);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(
                nameof(GetIncidentFiles),
                new { incidentId },
                result);
        }

        /// <summary>
        /// インシデントファイルの追加（JSON形式 - 後方互換性のため維持）
        /// </summary>
        /// <param name="incidentId">インシデントID</param>
        /// <param name="createDto">作成DTO</param>
        /// <returns>作成されたファイル</returns>
        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<IncidentFileResponseDto>>> CreateIncidentFile(
            int incidentId,
            [FromBody] CreateIncidentFileDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _fileService.CreateIncidentFileAsync(incidentId, createDto);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(
                nameof(GetIncidentFiles),
                new { incidentId },
                result);
        }

        /// <summary>
        /// インシデントファイルの削除
        /// </summary>
        /// <param name="incidentId">インシデントID</param>
        /// <param name="fileId">ファイルID</param>
        /// <returns>削除結果</returns>
        [HttpDelete("{fileId}")]
        public async Task<ActionResult<ApiResponseDto<bool>>> DeleteIncidentFile(
            int incidentId,
            int fileId)
        {
            var result = await _fileService.DeleteIncidentFileAsync(incidentId, fileId);
            
            if (!result.Success)
            {
                // ファイルが見つからない場合はNotFoundを返す
                if (result.ErrorMessage?.Contains("ファイルが見つかりません") == true)
                {
                    return NotFound(result);
                }
                
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}

