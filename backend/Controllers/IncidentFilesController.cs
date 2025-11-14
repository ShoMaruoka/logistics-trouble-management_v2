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
        /// インシデントファイルの追加
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
            var result = await _fileService.DeleteIncidentFileAsync(fileId);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}

