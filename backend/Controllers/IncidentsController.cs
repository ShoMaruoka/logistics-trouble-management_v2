using LogisticsTroubleManagement.DTOs;
using LogisticsTroubleManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LogisticsTroubleManagement.Controllers
{
    /// <summary>
    /// インシデントコントローラー
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class IncidentsController : ControllerBase
    {
        private readonly IIncidentService _incidentService;
        private readonly ILogger<IncidentsController> _logger;

        public IncidentsController(IIncidentService incidentService, ILogger<IncidentsController> logger)
        {
            _incidentService = incidentService;
            _logger = logger;
        }

        /// <summary>
        /// インシデント一覧の取得
        /// </summary>
        /// <param name="searchDto">検索条件</param>
        /// <returns>インシデント一覧</returns>
        [HttpGet]
        public async Task<ActionResult<PagedApiResponseDto<IncidentResponseDto>>> GetIncidents([FromQuery] IncidentSearchDto searchDto)
        {
            // デバッグログ
            _logger.LogInformation("GetIncidents called with Page={Page}, Limit={Limit}", searchDto.Page, searchDto.Limit);
            
            // パラメータの検証
            if (searchDto.Page < 1)
            {
                _logger.LogInformation("Page < 1, setting to 1. Original value: {Page}", searchDto.Page);
                searchDto.Page = 1;
            }
            
            if (searchDto.Limit < 1)
            {
                _logger.LogInformation("Limit < 1, setting to 20. Original value: {Limit}", searchDto.Limit);
                searchDto.Limit = 20;
            }
            
            if (searchDto.Limit > 100)
            {
                _logger.LogInformation("Limit > 100, setting to 100. Original value: {Limit}", searchDto.Limit);
                searchDto.Limit = 100;
            }
            
            _logger.LogInformation("After validation: Page={Page}, Limit={Limit}", searchDto.Page, searchDto.Limit);

            var result = await _incidentService.GetIncidentsAsync(searchDto);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// インシデントの取得
        /// </summary>
        /// <param name="id">インシデントID</param>
        /// <returns>インシデント</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDto<IncidentResponseDto>>> GetIncident(int id)
        {
            var result = await _incidentService.GetIncidentAsync(id);
            
            if (!result.Success)
            {
                if (result.ErrorMessage?.Contains("見つかりません") == true)
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// インシデントの作成
        /// </summary>
        /// <param name="createDto">作成DTO</param>
        /// <returns>作成されたインシデント</returns>
        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<IncidentResponseDto>>> CreateIncident([FromBody] CreateIncidentDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponseDto<IncidentResponseDto>.ErrorResponse("入力データが無効です"));
            }

            var userId = GetCurrentUserId();
            var userRoleId = GetCurrentUserRoleId();
            var result = await _incidentService.CreateIncidentAsync(createDto, userId, userRoleId);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetIncident), new { id = result.Data!.Id }, result);
        }

        /// <summary>
        /// インシデントの更新
        /// </summary>
        /// <param name="id">インシデントID</param>
        /// <param name="updateDto">更新DTO</param>
        /// <returns>更新されたインシデント</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponseDto<IncidentResponseDto>>> UpdateIncident(int id, [FromBody] UpdateIncidentDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponseDto<IncidentResponseDto>.ErrorResponse("入力データが無効です"));
            }

            var userId = GetCurrentUserId();
            var userRoleId = GetCurrentUserRoleId();
            var result = await _incidentService.UpdateIncidentAsync(id, updateDto, userId, userRoleId);
            
            if (!result.Success)
            {
                if (result.ErrorMessage?.Contains("見つかりません") == true)
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// インシデントの削除
        /// </summary>
        /// <param name="id">インシデントID</param>
        /// <returns>削除結果</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "システム管理者,部門管理者")]
        public async Task<ActionResult<ApiResponseDto<bool>>> DeleteIncident(int id)
        {
            var result = await _incidentService.DeleteIncidentAsync(id);
            
            if (!result.Success)
            {
                if (result.ErrorMessage?.Contains("見つかりません") == true)
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }

            return Ok(result);
        }


        /// <summary>
        /// インシデントのCSV出力
        /// </summary>
        /// <param name="searchDto">検索条件</param>
        /// <returns>CSVデータ</returns>
        [HttpGet("export")]
        public async Task<ActionResult> ExportIncidentsToCsv([FromQuery] IncidentSearchDto searchDto)
        {
            var result = await _incidentService.ExportIncidentsToCsvAsync(searchDto);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            var fileName = $"物流品質トラブル一覧_{DateTime.Now:yyyyMMdd}.csv";
            return File(result.Data!, "text/csv", fileName);
        }

        /// <summary>
        /// ダッシュボード統計の取得
        /// </summary>
        /// <returns>統計情報</returns>
        [HttpGet("dashboard/stats")]
        public async Task<ActionResult<ApiResponseDto<DashboardStatsDto>>> GetDashboardStats()
        {
            var result = await _incidentService.GetDashboardStatsAsync();
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// 現在のユーザーIDの取得
        /// </summary>
        /// <returns>ユーザーID</returns>
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
            {
                return userId;
            }
            throw new UnauthorizedAccessException("ユーザーIDを取得できませんでした");
        }

        /// <summary>
        /// 現在のユーザーロールIDの取得
        /// </summary>
        /// <returns>ユーザーロールID</returns>
        private int GetCurrentUserRoleId()
        {
            var userRoleIdClaim = User.FindFirst("user_role_id");
            if (userRoleIdClaim != null && int.TryParse(userRoleIdClaim.Value, out var userRoleId))
            {
                return userRoleId;
            }
            throw new UnauthorizedAccessException("ユーザーロールIDを取得できませんでした");
        }
    }
}

