using LogisticsTroubleManagement.DTOs;
using LogisticsTroubleManagement.Models;
using LogisticsTroubleManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<ActionResult<ApiResponseDto<string[]>>> GetOrganizations()
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
        public async Task<ActionResult<ApiResponseDto<string[]>>> GetOccurrenceLocations()
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
        public async Task<ActionResult<ApiResponseDto<string[]>>> GetShippingWarehouses()
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
        public async Task<ActionResult<ApiResponseDto<string[]>>> GetShippingCompanies()
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
        public async Task<ActionResult<ApiResponseDto<string[]>>> GetTroubleCategories()
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
        public async Task<ActionResult<ApiResponseDto<string[]>>> GetTroubleDetailCategories()
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
        public async Task<ActionResult<ApiResponseDto<string[]>>> GetUserRoles()
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
    }
}

