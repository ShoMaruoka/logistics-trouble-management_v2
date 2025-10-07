using LogisticsTroubleManagement.Data;
using LogisticsTroubleManagement.DTOs;
using LogisticsTroubleManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace LogisticsTroubleManagement.Services
{
    /// <summary>
    /// マスタデータサービスの実装
    /// </summary>
    public class MasterDataService : IMasterDataService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MasterDataService> _logger;

        public MasterDataService(
            ApplicationDbContext context,
            ILogger<MasterDataService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// 組織一覧の取得
        /// </summary>
        public async Task<ApiResponseDto<string[]>> GetOrganizationsAsync()
        {
            try
            {
                var organizations = await _context.Organizations
                    .Where(o => o.IsActive)
                    .OrderBy(o => o.Name)
                    .Select(o => o.Name)
                    .ToArrayAsync();

                return ApiResponseDto<string[]>.SuccessResponse(organizations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "組織一覧の取得中にエラーが発生しました");
                return ApiResponseDto<string[]>.ErrorResponse("組織一覧の取得に失敗しました");
            }
        }

        /// <summary>
        /// 発生場所一覧の取得
        /// </summary>
        public async Task<ApiResponseDto<string[]>> GetOccurrenceLocationsAsync()
        {
            try
            {
                var locations = await _context.OccurrenceLocations
                    .Where(l => l.IsActive)
                    .OrderBy(l => l.Name)
                    .Select(l => l.Name)
                    .ToArrayAsync();

                return ApiResponseDto<string[]>.SuccessResponse(locations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "発生場所一覧の取得中にエラーが発生しました");
                return ApiResponseDto<string[]>.ErrorResponse("発生場所一覧の取得に失敗しました");
            }
        }

        /// <summary>
        /// 出荷元倉庫一覧の取得
        /// </summary>
        public async Task<ApiResponseDto<string[]>> GetShippingWarehousesAsync()
        {
            try
            {
                var warehouses = await _context.Warehouses
                    .Where(w => w.IsActive)
                    .OrderBy(w => w.Name)
                    .Select(w => w.Name)
                    .ToArrayAsync();

                return ApiResponseDto<string[]>.SuccessResponse(warehouses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "出荷元倉庫一覧の取得中にエラーが発生しました");
                return ApiResponseDto<string[]>.ErrorResponse("出荷元倉庫一覧の取得に失敗しました");
            }
        }

        /// <summary>
        /// 運送会社一覧の取得
        /// </summary>
        public async Task<ApiResponseDto<string[]>> GetShippingCompaniesAsync()
        {
            try
            {
                var companies = await _context.ShippingCompanies
                    .Where(c => c.IsActive)
                    .OrderBy(c => c.Name)
                    .Select(c => c.Name)
                    .ToArrayAsync();

                return ApiResponseDto<string[]>.SuccessResponse(companies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "運送会社一覧の取得中にエラーが発生しました");
                return ApiResponseDto<string[]>.ErrorResponse("運送会社一覧の取得に失敗しました");
            }
        }

        /// <summary>
        /// トラブル区分一覧の取得
        /// </summary>
        public async Task<ApiResponseDto<string[]>> GetTroubleCategoriesAsync()
        {
            try
            {
                var categories = await _context.TroubleCategories
                    .Where(c => c.IsActive)
                    .OrderBy(c => c.Name)
                    .Select(c => c.Name)
                    .ToArrayAsync();

                return ApiResponseDto<string[]>.SuccessResponse(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "トラブル区分一覧の取得中にエラーが発生しました");
                return ApiResponseDto<string[]>.ErrorResponse("トラブル区分一覧の取得に失敗しました");
            }
        }

        /// <summary>
        /// トラブル詳細区分一覧の取得
        /// </summary>
        public async Task<ApiResponseDto<string[]>> GetTroubleDetailCategoriesAsync()
        {
            try
            {
                var detailCategories = await _context.TroubleDetailCategories
                    .Where(dc => dc.IsActive)
                    .OrderBy(dc => dc.Name)
                    .Select(dc => dc.Name)
                    .ToArrayAsync();

                return ApiResponseDto<string[]>.SuccessResponse(detailCategories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "トラブル詳細区分一覧の取得中にエラーが発生しました");
                return ApiResponseDto<string[]>.ErrorResponse("トラブル詳細区分一覧の取得に失敗しました");
            }
        }

        /// <summary>
        /// ユーザー役割一覧の取得
        /// </summary>
        public async Task<ApiResponseDto<string[]>> GetUserRolesAsync()
        {
            try
            {
                var roles = await _context.UserRoles
                    .OrderBy(r => r.RoleName)
                    .Select(r => r.RoleName)
                    .ToArrayAsync();

                return ApiResponseDto<string[]>.SuccessResponse(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ユーザー役割一覧の取得中にエラーが発生しました");
                return ApiResponseDto<string[]>.ErrorResponse("ユーザー役割一覧の取得に失敗しました");
            }
        }

        /// <summary>
        /// インシデントステータス一覧の取得
        /// </summary>
        public Task<ApiResponseDto<string[]>> GetIncidentStatusesAsync()
        {
            try
            {
                // IncidentStatusは静的クラスなので、そのまま返す
                var statuses = IncidentStatus.All;
                return Task.FromResult(ApiResponseDto<string[]>.SuccessResponse(statuses));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "インシデントステータス一覧の取得中にエラーが発生しました");
                return Task.FromResult(ApiResponseDto<string[]>.ErrorResponse("インシデントステータス一覧の取得に失敗しました"));
            }
        }
    }
}
