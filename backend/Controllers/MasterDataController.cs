using LogisticsTroubleManagement.DTOs;
using LogisticsTroubleManagement.Models;
using LogisticsTroubleManagement.Services;
using LogisticsTroubleManagement.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LogisticsTroubleManagement.Controllers
{
    /// <summary>
    /// マスタデータコントローラー
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class MasterDataController : ControllerBase
    {
        private readonly IMasterDataService _masterDataService;
        private readonly ILogger<MasterDataController> _logger;

        public MasterDataController(
            IMasterDataService masterDataService,
            ILogger<MasterDataController> logger)
        {
            _masterDataService = masterDataService;
            _logger = logger;
        }
        /// <summary>
        /// 組織一覧の取得
        /// </summary>
        /// <returns>組織一覧</returns>
        [HttpGet("organizations")]
        public async Task<ActionResult<ApiResponseDto<MasterDataItemDto[]>>> GetOrganizations()
        {
            var result = await _masterDataService.GetOrganizationsAsync();
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// 発生場所一覧の取得
        /// </summary>
        /// <returns>発生場所一覧</returns>
        [HttpGet("occurrence-locations")]
        public async Task<ActionResult<ApiResponseDto<MasterDataItemDto[]>>> GetOccurrenceLocations()
        {
            var result = await _masterDataService.GetOccurrenceLocationsAsync();
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// 出荷元倉庫一覧の取得
        /// </summary>
        /// <returns>出荷元倉庫一覧</returns>
        [HttpGet("shipping-warehouses")]
        public async Task<ActionResult<ApiResponseDto<MasterDataItemDto[]>>> GetShippingWarehouses()
        {
            var result = await _masterDataService.GetShippingWarehousesAsync();
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// 運送会社一覧の取得
        /// </summary>
        /// <returns>運送会社一覧</returns>
        [HttpGet("shipping-companies")]
        public async Task<ActionResult<ApiResponseDto<MasterDataItemDto[]>>> GetShippingCompanies()
        {
            var result = await _masterDataService.GetShippingCompaniesAsync();
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// トラブル区分一覧の取得
        /// </summary>
        /// <returns>トラブル区分一覧</returns>
        [HttpGet("trouble-categories")]
        public async Task<ActionResult<ApiResponseDto<MasterDataItemDto[]>>> GetTroubleCategories()
        {
            var result = await _masterDataService.GetTroubleCategoriesAsync();
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// トラブル詳細区分一覧の取得
        /// </summary>
        /// <returns>トラブル詳細区分一覧</returns>
        [HttpGet("trouble-detail-categories")]
        public async Task<ActionResult<ApiResponseDto<TroubleDetailCategoryItemDto[]>>> GetTroubleDetailCategories()
        {
            var result = await _masterDataService.GetTroubleDetailCategoriesAsync();
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// ユーザー役割一覧の取得
        /// </summary>
        /// <returns>ユーザー役割一覧</returns>
        [HttpGet("user-roles")]
        public async Task<ActionResult<ApiResponseDto<MasterDataItemDto[]>>> GetUserRoles()
        {
            var result = await _masterDataService.GetUserRolesAsync();
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// インシデントステータス一覧の取得
        /// </summary>
        /// <returns>インシデントステータス一覧</returns>
        [HttpGet("incident-statuses")]
        public async Task<ActionResult<ApiResponseDto<string[]>>> GetIncidentStatuses()
        {
            var result = await _masterDataService.GetIncidentStatusesAsync();
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// 倉庫一覧の取得
        /// </summary>
        /// <returns>倉庫一覧</returns>
        [HttpGet("warehouses")]
        public async Task<ActionResult<ApiResponseDto<List<MasterDataItemDto>>>> GetWarehouses()
        {
            var result = await _masterDataService.GetWarehousesAsync();
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// 単位一覧の取得
        /// </summary>
        /// <returns>単位一覧</returns>
        [HttpGet("units")]
        public async Task<ActionResult<ApiResponseDto<List<UnitItemDto>>>> GetUnits()
        {
            var result = await _masterDataService.GetUnitsAsync();
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// システムパラメータ一覧の取得
        /// </summary>
        /// <returns>システムパラメータ一覧</returns>
        [HttpGet("system-parameters")]
        public async Task<ActionResult<ApiResponseDto<List<SystemParameterItemDto>>>> GetSystemParameters()
        {
            var result = await _masterDataService.GetSystemParametersAsync();
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        // =============================================
        // CRUD操作エンドポイント（システム管理者のみ）
        // =============================================

        /// <summary>
        /// 組織の作成
        /// </summary>
        /// <param name="dto">組織作成DTO</param>
        /// <returns>作成された組織情報</returns>
        [HttpPost("organizations")]
        [Authorize]
        public async Task<ActionResult<ApiResponseDto<MasterDataItemDto>>> CreateOrganization([FromBody] MasterDataCreateDto dto)
        {
            try
            {
                // システム管理者権限チェック
                AuthorizationHelper.RequireSystemAdmin(User);
                var userId = AuthorizationHelper.GetUserIdRequired(User);

                var result = await _masterDataService.CreateOrganizationAsync(dto, userId);
                
                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("システム管理者権限なしで組織作成を試行: {Message}", ex.Message);
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "組織作成中にエラーが発生しました");
                return StatusCode(500, ApiResponseDto<MasterDataItemDto>.ErrorResponse("組織の作成中にエラーが発生しました"));
            }
        }

        /// <summary>
        /// 組織の更新
        /// </summary>
        /// <param name="id">組織ID</param>
        /// <param name="dto">組織更新DTO</param>
        /// <returns>更新された組織情報</returns>
        [HttpPut("organizations/{id}")]
        [Authorize]
        public async Task<ActionResult<ApiResponseDto<MasterDataItemDto>>> UpdateOrganization(int id, [FromBody] MasterDataUpdateDto dto)
        {
            try
            {
                // システム管理者権限チェック
                AuthorizationHelper.RequireSystemAdmin(User);
                var userId = AuthorizationHelper.GetUserIdRequired(User);

                // IDの整合性チェック
                if (id != dto.Id)
                {
                    return BadRequest(ApiResponseDto<MasterDataItemDto>.ErrorResponse("URLのIDとリクエストボディのIDが一致しません"));
                }

                var result = await _masterDataService.UpdateOrganizationAsync(dto, userId);
                
                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("システム管理者権限なしで組織更新を試行: {Message}", ex.Message);
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "組織更新中にエラーが発生しました: ID {Id}", id);
                return StatusCode(500, ApiResponseDto<MasterDataItemDto>.ErrorResponse("組織の更新中にエラーが発生しました"));
            }
        }

        /// <summary>
        /// 組織の削除（論理削除）
        /// </summary>
        /// <param name="id">組織ID</param>
        /// <returns>削除結果</returns>
        [HttpDelete("organizations/{id}")]
        [Authorize]
        public async Task<ActionResult<ApiResponseDto<bool>>> DeleteOrganization(int id)
        {
            try
            {
                // システム管理者権限チェック
                AuthorizationHelper.RequireSystemAdmin(User);
                var userId = AuthorizationHelper.GetUserIdRequired(User);

                var result = await _masterDataService.DeleteOrganizationAsync(id, userId);
                
                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("システム管理者権限なしで組織削除を試行: {Message}", ex.Message);
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "組織削除中にエラーが発生しました: ID {Id}", id);
                return StatusCode(500, ApiResponseDto<bool>.ErrorResponse("組織の削除中にエラーが発生しました"));
            }
        }

        /// <summary>
        /// 発生場所の作成
        /// </summary>
        /// <param name="dto">発生場所作成DTO</param>
        /// <returns>作成された発生場所情報</returns>
        [HttpPost("occurrence-locations")]
        [Authorize]
        public async Task<ActionResult<ApiResponseDto<MasterDataItemDto>>> CreateOccurrenceLocation([FromBody] MasterDataCreateDto dto)
        {
            try
            {
                // システム管理者権限チェック
                AuthorizationHelper.RequireSystemAdmin(User);
                var userId = AuthorizationHelper.GetUserIdRequired(User);

                var result = await _masterDataService.CreateOccurrenceLocationAsync(dto, userId);
                
                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("システム管理者権限なしで発生場所作成を試行: {Message}", ex.Message);
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "発生場所作成中にエラーが発生しました");
                return StatusCode(500, ApiResponseDto<MasterDataItemDto>.ErrorResponse("発生場所の作成中にエラーが発生しました"));
            }
        }

        /// <summary>
        /// 発生場所の更新
        /// </summary>
        /// <param name="id">発生場所ID</param>
        /// <param name="dto">発生場所更新DTO</param>
        /// <returns>更新された発生場所情報</returns>
        [HttpPut("occurrence-locations/{id}")]
        [Authorize]
        public async Task<ActionResult<ApiResponseDto<MasterDataItemDto>>> UpdateOccurrenceLocation(int id, [FromBody] MasterDataUpdateDto dto)
        {
            try
            {
                // システム管理者権限チェック
                AuthorizationHelper.RequireSystemAdmin(User);
                var userId = AuthorizationHelper.GetUserIdRequired(User);

                // IDの整合性チェック
                if (id != dto.Id)
                {
                    return BadRequest(ApiResponseDto<MasterDataItemDto>.ErrorResponse("URLのIDとリクエストボディのIDが一致しません"));
                }

                var result = await _masterDataService.UpdateOccurrenceLocationAsync(dto, userId);
                
                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("システム管理者権限なしで発生場所更新を試行: {Message}", ex.Message);
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "発生場所更新中にエラーが発生しました: ID {Id}", id);
                return StatusCode(500, ApiResponseDto<MasterDataItemDto>.ErrorResponse("発生場所の更新中にエラーが発生しました"));
            }
        }

        /// <summary>
        /// 発生場所の削除（論理削除）
        /// </summary>
        /// <param name="id">発生場所ID</param>
        /// <returns>削除結果</returns>
        [HttpDelete("occurrence-locations/{id}")]
        [Authorize]
        public async Task<ActionResult<ApiResponseDto<bool>>> DeleteOccurrenceLocation(int id)
        {
            try
            {
                // システム管理者権限チェック
                AuthorizationHelper.RequireSystemAdmin(User);
                var userId = AuthorizationHelper.GetUserIdRequired(User);

                var result = await _masterDataService.DeleteOccurrenceLocationAsync(id, userId);
                
                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("システム管理者権限なしで発生場所削除を試行: {Message}", ex.Message);
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "発生場所削除中にエラーが発生しました: ID {Id}", id);
                return StatusCode(500, ApiResponseDto<bool>.ErrorResponse("発生場所の削除中にエラーが発生しました"));
            }
        }

        // =============================================
        // 倉庫管理エンドポイント
        // =============================================

        /// <summary>
        /// 倉庫の作成
        /// </summary>
        /// <param name="dto">倉庫作成DTO</param>
        /// <returns>作成された倉庫情報</returns>
        [HttpPost("warehouses")]
        [Authorize]
        public async Task<ActionResult<ApiResponseDto<MasterDataItemDto>>> CreateWarehouse([FromBody] MasterDataCreateDto dto)
        {
            try
            {
                // システム管理者権限チェック
                AuthorizationHelper.RequireSystemAdmin(User);
                var userId = AuthorizationHelper.GetUserIdRequired(User);

                var result = await _masterDataService.CreateWarehouseAsync(dto, userId);
                
                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("システム管理者権限なしで倉庫作成を試行: {Message}", ex.Message);
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "倉庫作成中にエラーが発生しました");
                return StatusCode(500, ApiResponseDto<MasterDataItemDto>.ErrorResponse("倉庫の作成中にエラーが発生しました"));
            }
        }

        /// <summary>
        /// 倉庫の更新
        /// </summary>
        /// <param name="id">倉庫ID</param>
        /// <param name="dto">倉庫更新DTO</param>
        /// <returns>更新された倉庫情報</returns>
        [HttpPut("warehouses/{id}")]
        [Authorize]
        public async Task<ActionResult<ApiResponseDto<MasterDataItemDto>>> UpdateWarehouse(int id, [FromBody] MasterDataUpdateDto dto)
        {
            try
            {
                // システム管理者権限チェック
                AuthorizationHelper.RequireSystemAdmin(User);
                var userId = AuthorizationHelper.GetUserIdRequired(User);

                // IDの整合性チェック
                if (id != dto.Id)
                {
                    return BadRequest(ApiResponseDto<MasterDataItemDto>.ErrorResponse("URLのIDとリクエストボディのIDが一致しません"));
                }

                var result = await _masterDataService.UpdateWarehouseAsync(dto, userId);
                
                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("システム管理者権限なしで倉庫更新を試行: {Message}", ex.Message);
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "倉庫更新中にエラーが発生しました: ID {Id}", id);
                return StatusCode(500, ApiResponseDto<MasterDataItemDto>.ErrorResponse("倉庫の更新中にエラーが発生しました"));
            }
        }

        /// <summary>
        /// 倉庫の削除（論理削除）
        /// </summary>
        /// <param name="id">倉庫ID</param>
        /// <returns>削除結果</returns>
        [HttpDelete("warehouses/{id}")]
        [Authorize]
        public async Task<ActionResult<ApiResponseDto<bool>>> DeleteWarehouse(int id)
        {
            try
            {
                // システム管理者権限チェック
                AuthorizationHelper.RequireSystemAdmin(User);
                var userId = AuthorizationHelper.GetUserIdRequired(User);

                var result = await _masterDataService.DeleteWarehouseAsync(id, userId);
                
                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("システム管理者権限なしで倉庫削除を試行: {Message}", ex.Message);
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "倉庫削除中にエラーが発生しました: ID {Id}", id);
                return StatusCode(500, ApiResponseDto<bool>.ErrorResponse("倉庫の削除中にエラーが発生しました"));
            }
        }

        // =============================================
        // 運送会社管理エンドポイント
        // =============================================

        /// <summary>
        /// 運送会社の作成
        /// </summary>
        /// <param name="dto">運送会社作成DTO</param>
        /// <returns>作成された運送会社情報</returns>
        [HttpPost("shipping-companies")]
        [Authorize]
        public async Task<ActionResult<ApiResponseDto<MasterDataItemDto>>> CreateShippingCompany([FromBody] MasterDataCreateDto dto)
        {
            try
            {
                // システム管理者権限チェック
                AuthorizationHelper.RequireSystemAdmin(User);
                var userId = AuthorizationHelper.GetUserIdRequired(User);

                var result = await _masterDataService.CreateShippingCompanyAsync(dto, userId);
                
                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("システム管理者権限なしで運送会社作成を試行: {Message}", ex.Message);
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "運送会社作成中にエラーが発生しました");
                return StatusCode(500, ApiResponseDto<MasterDataItemDto>.ErrorResponse("運送会社の作成中にエラーが発生しました"));
            }
        }

        /// <summary>
        /// 運送会社の更新
        /// </summary>
        /// <param name="id">運送会社ID</param>
        /// <param name="dto">運送会社更新DTO</param>
        /// <returns>更新された運送会社情報</returns>
        [HttpPut("shipping-companies/{id}")]
        [Authorize]
        public async Task<ActionResult<ApiResponseDto<MasterDataItemDto>>> UpdateShippingCompany(int id, [FromBody] MasterDataUpdateDto dto)
        {
            try
            {
                // システム管理者権限チェック
                AuthorizationHelper.RequireSystemAdmin(User);
                var userId = AuthorizationHelper.GetUserIdRequired(User);

                // IDの整合性チェック
                if (id != dto.Id)
                {
                    return BadRequest(ApiResponseDto<MasterDataItemDto>.ErrorResponse("URLのIDとリクエストボディのIDが一致しません"));
                }

                var result = await _masterDataService.UpdateShippingCompanyAsync(dto, userId);
                
                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("システム管理者権限なしで運送会社更新を試行: {Message}", ex.Message);
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "運送会社更新中にエラーが発生しました: ID {Id}", id);
                return StatusCode(500, ApiResponseDto<MasterDataItemDto>.ErrorResponse("運送会社の更新中にエラーが発生しました"));
            }
        }

        /// <summary>
        /// 運送会社の削除（論理削除）
        /// </summary>
        /// <param name="id">運送会社ID</param>
        /// <returns>削除結果</returns>
        [HttpDelete("shipping-companies/{id}")]
        [Authorize]
        public async Task<ActionResult<ApiResponseDto<bool>>> DeleteShippingCompany(int id)
        {
            try
            {
                // システム管理者権限チェック
                AuthorizationHelper.RequireSystemAdmin(User);
                var userId = AuthorizationHelper.GetUserIdRequired(User);

                var result = await _masterDataService.DeleteShippingCompanyAsync(id, userId);
                
                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("システム管理者権限なしで運送会社削除を試行: {Message}", ex.Message);
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "運送会社削除中にエラーが発生しました: ID {Id}", id);
                return StatusCode(500, ApiResponseDto<bool>.ErrorResponse("運送会社の削除中にエラーが発生しました"));
            }
        }

        // =============================================
        // トラブル区分管理エンドポイント
        // =============================================

        /// <summary>
        /// トラブル区分の作成
        /// </summary>
        /// <param name="dto">トラブル区分作成DTO</param>
        /// <returns>作成されたトラブル区分情報</returns>
        [HttpPost("trouble-categories")]
        [Authorize]
        public async Task<ActionResult<ApiResponseDto<MasterDataItemDto>>> CreateTroubleCategory([FromBody] MasterDataCreateDto dto)
        {
            try
            {
                // システム管理者権限チェック
                AuthorizationHelper.RequireSystemAdmin(User);
                var userId = AuthorizationHelper.GetUserIdRequired(User);

                var result = await _masterDataService.CreateTroubleCategoryAsync(dto, userId);
                
                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("システム管理者権限なしでトラブル区分作成を試行: {Message}", ex.Message);
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "トラブル区分作成中にエラーが発生しました");
                return StatusCode(500, ApiResponseDto<MasterDataItemDto>.ErrorResponse("トラブル区分の作成中にエラーが発生しました"));
            }
        }

        /// <summary>
        /// トラブル区分の更新
        /// </summary>
        /// <param name="id">トラブル区分ID</param>
        /// <param name="dto">トラブル区分更新DTO</param>
        /// <returns>更新されたトラブル区分情報</returns>
        [HttpPut("trouble-categories/{id}")]
        [Authorize]
        public async Task<ActionResult<ApiResponseDto<MasterDataItemDto>>> UpdateTroubleCategory(int id, [FromBody] MasterDataUpdateDto dto)
        {
            try
            {
                // システム管理者権限チェック
                AuthorizationHelper.RequireSystemAdmin(User);
                var userId = AuthorizationHelper.GetUserIdRequired(User);

                // IDの整合性チェック
                if (id != dto.Id)
                {
                    return BadRequest(ApiResponseDto<MasterDataItemDto>.ErrorResponse("URLのIDとリクエストボディのIDが一致しません"));
                }

                var result = await _masterDataService.UpdateTroubleCategoryAsync(dto, userId);
                
                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("システム管理者権限なしでトラブル区分更新を試行: {Message}", ex.Message);
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "トラブル区分更新中にエラーが発生しました: ID {Id}", id);
                return StatusCode(500, ApiResponseDto<MasterDataItemDto>.ErrorResponse("トラブル区分の更新中にエラーが発生しました"));
            }
        }

        /// <summary>
        /// トラブル区分の削除（論理削除）
        /// </summary>
        /// <param name="id">トラブル区分ID</param>
        /// <returns>削除結果</returns>
        [HttpDelete("trouble-categories/{id}")]
        [Authorize]
        public async Task<ActionResult<ApiResponseDto<bool>>> DeleteTroubleCategory(int id)
        {
            try
            {
                // システム管理者権限チェック
                AuthorizationHelper.RequireSystemAdmin(User);
                var userId = AuthorizationHelper.GetUserIdRequired(User);

                var result = await _masterDataService.DeleteTroubleCategoryAsync(id, userId);
                
                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("システム管理者権限なしでトラブル区分削除を試行: {Message}", ex.Message);
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "トラブル区分削除中にエラーが発生しました: ID {Id}", id);
                return StatusCode(500, ApiResponseDto<bool>.ErrorResponse("トラブル区分の削除中にエラーが発生しました"));
            }
        }

        // =============================================
        // トラブル詳細区分管理エンドポイント
        // =============================================

        /// <summary>
        /// トラブル詳細区分の作成
        /// </summary>
        /// <param name="dto">トラブル詳細区分作成DTO</param>
        /// <returns>作成されたトラブル詳細区分情報</returns>
        [HttpPost("trouble-detail-categories")]
        [Authorize]
        public async Task<ActionResult<ApiResponseDto<MasterDataItemDto>>> CreateTroubleDetailCategory([FromBody] TroubleDetailCategoryCreateDto dto)
        {
            try
            {
                // システム管理者権限チェック
                AuthorizationHelper.RequireSystemAdmin(User);
                var userId = AuthorizationHelper.GetUserIdRequired(User);

                var result = await _masterDataService.CreateTroubleDetailCategoryAsync(dto, userId);
                
                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("システム管理者権限なしでトラブル詳細区分作成を試行: {Message}", ex.Message);
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "トラブル詳細区分作成中にエラーが発生しました");
                return StatusCode(500, ApiResponseDto<MasterDataItemDto>.ErrorResponse("トラブル詳細区分の作成中にエラーが発生しました"));
            }
        }

        /// <summary>
        /// トラブル詳細区分の更新
        /// </summary>
        /// <param name="id">トラブル詳細区分ID</param>
        /// <param name="dto">トラブル詳細区分更新DTO</param>
        /// <returns>更新されたトラブル詳細区分情報</returns>
        [HttpPut("trouble-detail-categories/{id}")]
        [Authorize]
        public async Task<ActionResult<ApiResponseDto<MasterDataItemDto>>> UpdateTroubleDetailCategory(int id, [FromBody] TroubleDetailCategoryUpdateDto dto)
        {
            try
            {
                // システム管理者権限チェック
                AuthorizationHelper.RequireSystemAdmin(User);
                var userId = AuthorizationHelper.GetUserIdRequired(User);

                // IDの整合性チェック
                if (id != dto.Id)
                {
                    return BadRequest(ApiResponseDto<MasterDataItemDto>.ErrorResponse("URLのIDとリクエストボディのIDが一致しません"));
                }

                var result = await _masterDataService.UpdateTroubleDetailCategoryAsync(dto, userId);
                
                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("システム管理者権限なしでトラブル詳細区分更新を試行: {Message}", ex.Message);
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "トラブル詳細区分更新中にエラーが発生しました: ID {Id}", id);
                return StatusCode(500, ApiResponseDto<MasterDataItemDto>.ErrorResponse("トラブル詳細区分の更新中にエラーが発生しました"));
            }
        }

        /// <summary>
        /// トラブル詳細区分の削除（論理削除）
        /// </summary>
        /// <param name="id">トラブル詳細区分ID</param>
        /// <returns>削除結果</returns>
        [HttpDelete("trouble-detail-categories/{id}")]
        [Authorize]
        public async Task<ActionResult<ApiResponseDto<bool>>> DeleteTroubleDetailCategory(int id)
        {
            try
            {
                // システム管理者権限チェック
                AuthorizationHelper.RequireSystemAdmin(User);
                var userId = AuthorizationHelper.GetUserIdRequired(User);

                var result = await _masterDataService.DeleteTroubleDetailCategoryAsync(id, userId);
                
                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("システム管理者権限なしでトラブル詳細区分削除を試行: {Message}", ex.Message);
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "トラブル詳細区分削除中にエラーが発生しました: ID {Id}", id);
                return StatusCode(500, ApiResponseDto<bool>.ErrorResponse("トラブル詳細区分の削除中にエラーが発生しました"));
            }
        }

        // =============================================
        // 単位管理エンドポイント
        // =============================================

        /// <summary>
        /// 単位の作成
        /// </summary>
        /// <param name="dto">単位作成DTO</param>
        /// <returns>作成された単位情報</returns>
        [HttpPost("units")]
        [Authorize]
        public async Task<ActionResult<ApiResponseDto<MasterDataItemDto>>> CreateUnit([FromBody] UnitCreateDto dto)
        {
            try
            {
                // システム管理者権限チェック
                AuthorizationHelper.RequireSystemAdmin(User);
                var userId = AuthorizationHelper.GetUserIdRequired(User);

                var result = await _masterDataService.CreateUnitAsync(dto, userId);
                
                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("システム管理者権限なしで単位作成を試行: {Message}", ex.Message);
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "単位作成中にエラーが発生しました");
                return StatusCode(500, ApiResponseDto<MasterDataItemDto>.ErrorResponse("単位の作成中にエラーが発生しました"));
            }
        }

        /// <summary>
        /// 単位の更新
        /// </summary>
        /// <param name="id">単位ID</param>
        /// <param name="dto">単位更新DTO</param>
        /// <returns>更新された単位情報</returns>
        [HttpPut("units/{id}")]
        [Authorize]
        public async Task<ActionResult<ApiResponseDto<MasterDataItemDto>>> UpdateUnit(int id, [FromBody] UnitUpdateDto dto)
        {
            try
            {
                // システム管理者権限チェック
                AuthorizationHelper.RequireSystemAdmin(User);
                var userId = AuthorizationHelper.GetUserIdRequired(User);

                // IDの整合性チェック
                if (id != dto.Id)
                {
                    return BadRequest(ApiResponseDto<MasterDataItemDto>.ErrorResponse("URLのIDとリクエストボディのIDが一致しません"));
                }

                var result = await _masterDataService.UpdateUnitAsync(dto, userId);
                
                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("システム管理者権限なしで単位更新を試行: {Message}", ex.Message);
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "単位更新中にエラーが発生しました: ID {Id}", id);
                return StatusCode(500, ApiResponseDto<MasterDataItemDto>.ErrorResponse("単位の更新中にエラーが発生しました"));
            }
        }

        /// <summary>
        /// 単位の削除（論理削除）
        /// </summary>
        /// <param name="id">単位ID</param>
        /// <returns>削除結果</returns>
        [HttpDelete("units/{id}")]
        [Authorize]
        public async Task<ActionResult<ApiResponseDto<bool>>> DeleteUnit(int id)
        {
            try
            {
                // システム管理者権限チェック
                AuthorizationHelper.RequireSystemAdmin(User);
                var userId = AuthorizationHelper.GetUserIdRequired(User);

                var result = await _masterDataService.DeleteUnitAsync(id, userId);
                
                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("システム管理者権限なしで単位削除を試行: {Message}", ex.Message);
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "単位削除中にエラーが発生しました: ID {Id}", id);
                return StatusCode(500, ApiResponseDto<bool>.ErrorResponse("単位の削除中にエラーが発生しました"));
            }
        }

        // =============================================
        // システムパラメータ管理エンドポイント
        // =============================================

        /// <summary>
        /// システムパラメータの作成
        /// </summary>
        /// <param name="dto">システムパラメータ作成DTO</param>
        /// <returns>作成されたシステムパラメータ情報</returns>
        [HttpPost("system-parameters")]
        [Authorize]
        public async Task<ActionResult<ApiResponseDto<MasterDataItemDto>>> CreateSystemParameter([FromBody] SystemParameterCreateDto dto)
        {
            try
            {
                // システム管理者権限チェック
                AuthorizationHelper.RequireSystemAdmin(User);
                var userId = AuthorizationHelper.GetUserIdRequired(User);

                var result = await _masterDataService.CreateSystemParameterAsync(dto, userId);
                
                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("システム管理者権限なしでシステムパラメータ作成を試行: {Message}", ex.Message);
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "システムパラメータ作成中にエラーが発生しました");
                return StatusCode(500, ApiResponseDto<MasterDataItemDto>.ErrorResponse("システムパラメータの作成中にエラーが発生しました"));
            }
        }

        /// <summary>
        /// システムパラメータの更新
        /// </summary>
        /// <param name="id">システムパラメータID</param>
        /// <param name="dto">システムパラメータ更新DTO</param>
        /// <returns>更新されたシステムパラメータ情報</returns>
        [HttpPut("system-parameters/{id}")]
        [Authorize]
        public async Task<ActionResult<ApiResponseDto<MasterDataItemDto>>> UpdateSystemParameter(int id, [FromBody] SystemParameterUpdateDto dto)
        {
            try
            {
                // システム管理者権限チェック
                AuthorizationHelper.RequireSystemAdmin(User);
                var userId = AuthorizationHelper.GetUserIdRequired(User);

                // IDの整合性チェック
                if (id != dto.Id)
                {
                    return BadRequest(ApiResponseDto<MasterDataItemDto>.ErrorResponse("URLのIDとリクエストボディのIDが一致しません"));
                }

                var result = await _masterDataService.UpdateSystemParameterAsync(dto, userId);
                
                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("システム管理者権限なしでシステムパラメータ更新を試行: {Message}", ex.Message);
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "システムパラメータ更新中にエラーが発生しました: ID {Id}", id);
                return StatusCode(500, ApiResponseDto<MasterDataItemDto>.ErrorResponse("システムパラメータの更新中にエラーが発生しました"));
            }
        }

        /// <summary>
        /// システムパラメータの削除（論理削除）
        /// </summary>
        /// <param name="id">システムパラメータID</param>
        /// <returns>削除結果</returns>
        [HttpDelete("system-parameters/{id}")]
        [Authorize]
        public async Task<ActionResult<ApiResponseDto<bool>>> DeleteSystemParameter(int id)
        {
            try
            {
                // システム管理者権限チェック
                AuthorizationHelper.RequireSystemAdmin(User);
                var userId = AuthorizationHelper.GetUserIdRequired(User);

                var result = await _masterDataService.DeleteSystemParameterAsync(id, userId);
                
                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("システム管理者権限なしでシステムパラメータ削除を試行: {Message}", ex.Message);
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "システムパラメータ削除中にエラーが発生しました: ID {Id}", id);
                return StatusCode(500, ApiResponseDto<bool>.ErrorResponse("システムパラメータの削除中にエラーが発生しました"));
            }
        }
    }
}

