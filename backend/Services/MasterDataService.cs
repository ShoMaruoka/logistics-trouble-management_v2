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
        public async Task<ApiResponseDto<MasterDataItemDto[]>> GetOrganizationsAsync()
        {
            try
            {
                var organizations = await _context.Organizations
                    .Where(o => o.IsActive)
                    .OrderBy(o => o.DisplayOrder)
                    .ThenBy(o => o.Id)
                    .Select(o => new MasterDataItemDto
                    {
                        Id = o.Id,
                        Name = o.Name,
                        DisplayOrder = o.DisplayOrder,
                        IsActive = o.IsActive,
                        CreatedAt = o.CreatedAt,
                        UpdatedAt = o.UpdatedAt
                    })
                    .ToArrayAsync();

                return ApiResponseDto<MasterDataItemDto[]>.SuccessResponse(organizations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "組織一覧の取得中にエラーが発生しました");
                return ApiResponseDto<MasterDataItemDto[]>.ErrorResponse("組織一覧の取得に失敗しました");
            }
        }

        /// <summary>
        /// 発生場所一覧の取得
        /// </summary>
        public async Task<ApiResponseDto<MasterDataItemDto[]>> GetOccurrenceLocationsAsync()
        {
            try
            {
                var locations = await _context.OccurrenceLocations
                    .Where(l => l.IsActive)
                    .OrderBy(l => l.DisplayOrder)
                    .ThenBy(l => l.Id)
                    .Select(l => new MasterDataItemDto
                    {
                        Id = l.Id,
                        Name = l.Name,
                        DisplayOrder = l.DisplayOrder,
                        IsActive = l.IsActive,
                        CreatedAt = l.CreatedAt,
                        UpdatedAt = l.UpdatedAt
                    })
                    .ToArrayAsync();

                return ApiResponseDto<MasterDataItemDto[]>.SuccessResponse(locations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "発生場所一覧の取得中にエラーが発生しました");
                return ApiResponseDto<MasterDataItemDto[]>.ErrorResponse("発生場所一覧の取得に失敗しました");
            }
        }


        /// <summary>
        /// 運送会社一覧の取得
        /// </summary>
        public async Task<ApiResponseDto<MasterDataItemDto[]>> GetShippingCompaniesAsync()
        {
            try
            {
                var companies = await _context.ShippingCompanies
                    .Where(c => c.IsActive)
                    .OrderBy(c => c.DisplayOrder)
                    .ThenBy(c => c.Id)
                    .Select(c => new MasterDataItemDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        DisplayOrder = c.DisplayOrder,
                        IsActive = c.IsActive,
                        CreatedAt = c.CreatedAt,
                        UpdatedAt = c.UpdatedAt
                    })
                    .ToArrayAsync();

                return ApiResponseDto<MasterDataItemDto[]>.SuccessResponse(companies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "運送会社一覧の取得中にエラーが発生しました");
                return ApiResponseDto<MasterDataItemDto[]>.ErrorResponse("運送会社一覧の取得に失敗しました");
            }
        }

        /// <summary>
        /// トラブル区分一覧の取得
        /// </summary>
        public async Task<ApiResponseDto<MasterDataItemDto[]>> GetTroubleCategoriesAsync()
        {
            try
            {
                var categories = await _context.TroubleCategories
                    .Where(c => c.IsActive)
                    .OrderBy(c => c.DisplayOrder)
                    .ThenBy(c => c.Id)
                    .Select(c => new MasterDataItemDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        DisplayOrder = c.DisplayOrder,
                        IsActive = c.IsActive,
                        CreatedAt = c.CreatedAt,
                        UpdatedAt = c.UpdatedAt
                    })
                    .ToArrayAsync();

                return ApiResponseDto<MasterDataItemDto[]>.SuccessResponse(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "トラブル区分一覧の取得中にエラーが発生しました");
                return ApiResponseDto<MasterDataItemDto[]>.ErrorResponse("トラブル区分一覧の取得に失敗しました");
            }
        }

        /// <summary>
        /// トラブル詳細区分一覧の取得
        /// </summary>
        public async Task<ApiResponseDto<TroubleDetailCategoryItemDto[]>> GetTroubleDetailCategoriesAsync()
        {
            try
            {
                var detailCategories = await _context.TroubleDetailCategories
                    .Where(dc => dc.IsActive)
                    .OrderBy(dc => dc.DisplayOrder)
                    .ThenBy(dc => dc.Id)
                    .Select(dc => new TroubleDetailCategoryItemDto
                    {
                        Id = dc.Id,
                        Name = dc.Name,
                        DisplayOrder = dc.DisplayOrder,
                        IsActive = dc.IsActive,
                        TroubleCategoryId = dc.TroubleCategoryId,
                        CreatedAt = dc.CreatedAt,
                        UpdatedAt = dc.UpdatedAt
                    })
                    .ToArrayAsync();

                return ApiResponseDto<TroubleDetailCategoryItemDto[]>.SuccessResponse(detailCategories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "トラブル詳細区分一覧の取得中にエラーが発生しました");
                return ApiResponseDto<TroubleDetailCategoryItemDto[]>.ErrorResponse("トラブル詳細区分一覧の取得に失敗しました");
            }
        }

        /// <summary>
        /// ユーザー役割一覧の取得
        /// </summary>
        public async Task<ApiResponseDto<MasterDataItemDto[]>> GetUserRolesAsync()
        {
            try
            {
                var roles = await _context.UserRoles
                    .OrderBy(r => r.DisplayOrder)
                    .ThenBy(r => r.Id)
                    .Select(r => new MasterDataItemDto
                    {
                        Id = r.Id,
                        Name = r.RoleName,
                        DisplayOrder = r.DisplayOrder,
                        IsActive = true, // UserRoleにはIsActiveフィールドがないためtrueを設定
                        CreatedAt = r.CreatedAt, // UserRoleエンティティのCreatedAtフィールドを使用
                        UpdatedAt = null // UserRoleにはUpdatedAtフィールドがないためnullを設定
                    })
                    .ToArrayAsync();

                return ApiResponseDto<MasterDataItemDto[]>.SuccessResponse(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ユーザー役割一覧の取得中にエラーが発生しました");
                return ApiResponseDto<MasterDataItemDto[]>.ErrorResponse("ユーザー役割一覧の取得に失敗しました");
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

        /// <summary>
        /// 倉庫一覧の取得
        /// </summary>
        public async Task<ApiResponseDto<List<MasterDataItemDto>>> GetWarehousesAsync()
        {
            try
            {
                var warehouses = await _context.Warehouses
                    .Where(w => w.IsActive)
                    .OrderBy(w => w.DisplayOrder)
                    .ThenBy(w => w.Id)
                    .ToListAsync();

                var result = warehouses.Select(w => new MasterDataItemDto
                {
                    Id = w.Id,
                    Name = w.Name,
                    DisplayOrder = w.DisplayOrder,
                    IsActive = w.IsActive,
                    CreatedAt = w.CreatedAt,
                    UpdatedAt = w.UpdatedAt
                }).ToList();

                return ApiResponseDto<List<MasterDataItemDto>>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "倉庫一覧の取得中にエラーが発生しました");
                return ApiResponseDto<List<MasterDataItemDto>>.ErrorResponse("倉庫一覧の取得に失敗しました");
            }
        }

        /// <summary>
        /// 単位一覧の取得
        /// </summary>
        public async Task<ApiResponseDto<List<UnitItemDto>>> GetUnitsAsync()
        {
            try
            {
                var units = await _context.Units
                    .Where(u => u.IsActive)
                    .OrderBy(u => u.DisplayOrder)
                    .ThenBy(u => u.Id)
                    .ToListAsync();

                var result = units.Select(u => new UnitItemDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Code = u.Code,
                    DisplayOrder = u.DisplayOrder,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt
                }).ToList();

                return ApiResponseDto<List<UnitItemDto>>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "単位一覧の取得中にエラーが発生しました");
                return ApiResponseDto<List<UnitItemDto>>.ErrorResponse("単位一覧の取得に失敗しました");
            }
        }

        /// <summary>
        /// システムパラメータ一覧の取得
        /// </summary>
        public async Task<ApiResponseDto<List<SystemParameterItemDto>>> GetSystemParametersAsync()
        {
            try
            {
                var systemParameters = await _context.SystemParameters
                    .Where(sp => sp.IsActive)
                    .OrderBy(sp => sp.DisplayOrder)
                    .ThenBy(sp => sp.Id)
                    .ToListAsync();

                var result = systemParameters.Select(sp => new SystemParameterItemDto
                {
                    Id = sp.Id,
                    Name = sp.ParameterKey, // パラメータキーを名前として使用
                    ParameterKey = sp.ParameterKey,
                    ParameterValue = sp.ParameterValue,
                    Description = sp.Description,
                    DataType = sp.DataType,
                    DisplayOrder = sp.DisplayOrder,
                    IsActive = sp.IsActive,
                    CreatedAt = sp.CreatedAt,
                    UpdatedAt = sp.UpdatedAt
                }).ToList();

                return ApiResponseDto<List<SystemParameterItemDto>>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "システムパラメータ一覧の取得中にエラーが発生しました");
                return ApiResponseDto<List<SystemParameterItemDto>>.ErrorResponse("システムパラメータ一覧の取得に失敗しました");
            }
        }

        // =============================================
        // CRUD操作メソッド（システム管理者のみ）
        // =============================================

        /// <summary>
        /// 組織の作成
        /// </summary>
        public async Task<ApiResponseDto<MasterDataItemDto>> CreateOrganizationAsync(MasterDataCreateDto dto, int userId)
        {
            try
            {
                // 1. Name非空チェック
                if (string.IsNullOrWhiteSpace(dto.Name))
                {
                    _logger.LogWarning("組織作成失敗: Nameが空です (UserId: {UserId})", userId);
                    return ApiResponseDto<MasterDataItemDto>.ErrorResponse("組織名は必須です");
                }

                // 2. 一意性チェック
                var existingOrganization = await _context.Organizations
                    .AnyAsync(o => o.Name == dto.Name);
                if (existingOrganization)
                {
                    _logger.LogWarning("組織作成失敗: 重複する組織名です - {Name} (UserId: {UserId})", dto.Name, userId);
                    return ApiResponseDto<MasterDataItemDto>.ErrorResponse($"組織名「{dto.Name}」は既に存在します");
                }

                var organization = new Organization
                {
                    Name = dto.Name,
                    DisplayOrder = dto.DisplayOrder,
                    IsActive = dto.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Organizations.Add(organization);
                await _context.SaveChangesAsync();

                var result = new MasterDataItemDto
                {
                    Id = organization.Id,
                    Name = organization.Name,
                    DisplayOrder = organization.DisplayOrder,
                    IsActive = organization.IsActive,
                    CreatedAt = organization.CreatedAt,
                    UpdatedAt = organization.UpdatedAt
                };

                _logger.LogInformation("組織を作成しました: {Name} (ID: {Id})", dto.Name, organization.Id);
                return ApiResponseDto<MasterDataItemDto>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "組織の作成中にエラーが発生しました: {Name}", dto.Name);
                return ApiResponseDto<MasterDataItemDto>.ErrorResponse("組織の作成に失敗しました");
            }
        }

        /// <summary>
        /// 組織の更新
        /// </summary>
        public async Task<ApiResponseDto<MasterDataItemDto>> UpdateOrganizationAsync(MasterDataUpdateDto dto, int userId)
        {
            try
            {
                // 1. Name非空チェック
                if (string.IsNullOrWhiteSpace(dto.Name))
                {
                    _logger.LogWarning("組織更新失敗: Nameが空です (Id: {Id}, UserId: {UserId})", dto.Id, userId);
                    return ApiResponseDto<MasterDataItemDto>.ErrorResponse("組織名は必須です");
                }

                var organization = await _context.Organizations.FindAsync(dto.Id);
                if (organization == null)
                {
                    return ApiResponseDto<MasterDataItemDto>.ErrorResponse("指定された組織が見つかりません");
                }

                // 2. 一意性チェック（同じIdを除外）
                var existingOrganization = await _context.Organizations
                    .AnyAsync(o => o.Name == dto.Name && o.Id != dto.Id);
                if (existingOrganization)
                {
                    _logger.LogWarning("組織更新失敗: 重複する組織名です - {Name} (Id: {Id}, UserId: {UserId})", dto.Name, dto.Id, userId);
                    return ApiResponseDto<MasterDataItemDto>.ErrorResponse($"組織名「{dto.Name}」は既に存在します");
                }

                organization.Name = dto.Name;
                organization.DisplayOrder = dto.DisplayOrder;
                organization.IsActive = dto.IsActive;
                organization.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var result = new MasterDataItemDto
                {
                    Id = organization.Id,
                    Name = organization.Name,
                    DisplayOrder = organization.DisplayOrder,
                    IsActive = organization.IsActive,
                    CreatedAt = organization.CreatedAt,
                    UpdatedAt = organization.UpdatedAt
                };

                _logger.LogInformation("組織を更新しました: {Name} (ID: {Id})", dto.Name, dto.Id);
                return ApiResponseDto<MasterDataItemDto>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "組織の更新中にエラーが発生しました: {Name} (ID: {Id})", dto.Name, dto.Id);
                return ApiResponseDto<MasterDataItemDto>.ErrorResponse("組織の更新に失敗しました");
            }
        }

        /// <summary>
        /// 組織の削除（論理削除）
        /// </summary>
        public async Task<ApiResponseDto<bool>> DeleteOrganizationAsync(int id, int userId)
        {
            try
            {
                var organization = await _context.Organizations.FindAsync(id);
                if (organization == null)
                {
                    return ApiResponseDto<bool>.ErrorResponse("指定された組織が見つかりません");
                }

                // 参照整合性チェック - インシデントで参照されているかチェック
                var hasReferences = await _context.Incidents
                    .AnyAsync(i => i.Organization == id);
                if (hasReferences)
                {
                    _logger.LogWarning("組織削除失敗: インシデントで参照されています (Id: {Id}, UserId: {UserId})", id, userId);
                    return ApiResponseDto<bool>.ErrorResponse("この組織はインシデントで使用されているため削除できません");
                }

                organization.IsActive = false;
                organization.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("組織を削除しました: {Name} (ID: {Id})", organization.Name, id);
                return ApiResponseDto<bool>.SuccessResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "組織の削除中にエラーが発生しました: ID {Id}", id);
                return ApiResponseDto<bool>.ErrorResponse("組織の削除に失敗しました");
            }
        }

        /// <summary>
        /// 発生場所の作成
        /// </summary>
        public async Task<ApiResponseDto<MasterDataItemDto>> CreateOccurrenceLocationAsync(MasterDataCreateDto dto, int userId)
        {
            try
            {
                // 1. Name非空チェック
                if (string.IsNullOrWhiteSpace(dto.Name))
                {
                    _logger.LogWarning("発生場所作成失敗: Nameが空です (UserId: {UserId})", userId);
                    return ApiResponseDto<MasterDataItemDto>.ErrorResponse("発生場所名は必須です");
                }

                // 2. 一意性チェック
                var existingLocation = await _context.OccurrenceLocations
                    .AnyAsync(l => l.Name == dto.Name);
                if (existingLocation)
                {
                    _logger.LogWarning("発生場所作成失敗: 重複する発生場所名です - {Name} (UserId: {UserId})", dto.Name, userId);
                    return ApiResponseDto<MasterDataItemDto>.ErrorResponse($"発生場所名「{dto.Name}」は既に存在します");
                }

                var location = new OccurrenceLocation
                {
                    Name = dto.Name,
                    DisplayOrder = dto.DisplayOrder,
                    IsActive = dto.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.OccurrenceLocations.Add(location);
                await _context.SaveChangesAsync();

                var result = new MasterDataItemDto
                {
                    Id = location.Id,
                    Name = location.Name,
                    DisplayOrder = location.DisplayOrder,
                    IsActive = location.IsActive,
                    CreatedAt = location.CreatedAt,
                    UpdatedAt = location.UpdatedAt
                };

                _logger.LogInformation("発生場所を作成しました: {Name} (ID: {Id})", dto.Name, location.Id);
                return ApiResponseDto<MasterDataItemDto>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "発生場所の作成中にエラーが発生しました: {Name}", dto.Name);
                return ApiResponseDto<MasterDataItemDto>.ErrorResponse("発生場所の作成に失敗しました");
            }
        }

        /// <summary>
        /// 発生場所の更新
        /// </summary>
        public async Task<ApiResponseDto<MasterDataItemDto>> UpdateOccurrenceLocationAsync(MasterDataUpdateDto dto, int userId)
        {
            try
            {
                // 1. Name非空チェック
                if (string.IsNullOrWhiteSpace(dto.Name))
                {
                    _logger.LogWarning("発生場所更新失敗: Nameが空です (Id: {Id}, UserId: {UserId})", dto.Id, userId);
                    return ApiResponseDto<MasterDataItemDto>.ErrorResponse("発生場所名は必須です");
                }

                var location = await _context.OccurrenceLocations.FindAsync(dto.Id);
                if (location == null)
                {
                    return ApiResponseDto<MasterDataItemDto>.ErrorResponse("指定された発生場所が見つかりません");
                }

                // 2. 一意性チェック（同じIdを除外）
                var existingLocation = await _context.OccurrenceLocations
                    .AnyAsync(l => l.Name == dto.Name && l.Id != dto.Id);
                if (existingLocation)
                {
                    _logger.LogWarning("発生場所更新失敗: 重複する発生場所名です - {Name} (Id: {Id}, UserId: {UserId})", dto.Name, dto.Id, userId);
                    return ApiResponseDto<MasterDataItemDto>.ErrorResponse($"発生場所名「{dto.Name}」は既に存在します");
                }

                location.Name = dto.Name;
                location.DisplayOrder = dto.DisplayOrder;
                location.IsActive = dto.IsActive;
                location.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var result = new MasterDataItemDto
                {
                    Id = location.Id,
                    Name = location.Name,
                    DisplayOrder = location.DisplayOrder,
                    IsActive = location.IsActive,
                    CreatedAt = location.CreatedAt,
                    UpdatedAt = location.UpdatedAt
                };

                _logger.LogInformation("発生場所を更新しました: {Name} (ID: {Id})", dto.Name, dto.Id);
                return ApiResponseDto<MasterDataItemDto>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "発生場所の更新中にエラーが発生しました: {Name} (ID: {Id})", dto.Name, dto.Id);
                return ApiResponseDto<MasterDataItemDto>.ErrorResponse("発生場所の更新に失敗しました");
            }
        }

        /// <summary>
        /// 発生場所の削除（論理削除）
        /// </summary>
        public async Task<ApiResponseDto<bool>> DeleteOccurrenceLocationAsync(int id, int userId)
        {
            try
            {
                var location = await _context.OccurrenceLocations.FindAsync(id);
                if (location == null)
                {
                    return ApiResponseDto<bool>.ErrorResponse("指定された発生場所が見つかりません");
                }

                // 参照整合性チェック - インシデントで参照されているかチェック
                var hasReferences = await _context.Incidents
                    .AnyAsync(i => i.OccurrenceLocation == id);
                if (hasReferences)
                {
                    _logger.LogWarning("発生場所削除失敗: インシデントで参照されています (Id: {Id}, UserId: {UserId})", id, userId);
                    return ApiResponseDto<bool>.ErrorResponse("この発生場所はインシデントで使用されているため削除できません");
                }

                location.IsActive = false;
                location.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("発生場所を削除しました: {Name} (ID: {Id})", location.Name, id);
                return ApiResponseDto<bool>.SuccessResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "発生場所の削除中にエラーが発生しました: ID {Id}", id);
                return ApiResponseDto<bool>.ErrorResponse("発生場所の削除に失敗しました");
            }
        }

        /// <summary>
        /// 倉庫の作成
        /// </summary>
        public async Task<ApiResponseDto<MasterDataItemDto>> CreateWarehouseAsync(MasterDataCreateDto dto, int userId)
        {
            try
            {
                // 1. Name非空チェック
                if (string.IsNullOrWhiteSpace(dto.Name))
                {
                    _logger.LogWarning("倉庫作成失敗: Nameが空です (UserId: {UserId})", userId);
                    return ApiResponseDto<MasterDataItemDto>.ErrorResponse("倉庫名は必須です");
                }

                // 2. 一意性チェック
                var existingWarehouse = await _context.Warehouses
                    .AnyAsync(w => w.Name == dto.Name);
                if (existingWarehouse)
                {
                    _logger.LogWarning("倉庫作成失敗: 重複する倉庫名です - {Name} (UserId: {UserId})", dto.Name, userId);
                    return ApiResponseDto<MasterDataItemDto>.ErrorResponse($"倉庫名「{dto.Name}」は既に存在します");
                }

                var warehouse = new Warehouse
                {
                    Name = dto.Name,
                    DisplayOrder = dto.DisplayOrder,
                    IsActive = dto.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Warehouses.Add(warehouse);
                await _context.SaveChangesAsync();

                var result = new MasterDataItemDto
                {
                    Id = warehouse.Id,
                    Name = warehouse.Name,
                    DisplayOrder = warehouse.DisplayOrder,
                    IsActive = warehouse.IsActive,
                    CreatedAt = warehouse.CreatedAt,
                    UpdatedAt = warehouse.UpdatedAt
                };

                _logger.LogInformation("倉庫を作成しました: {Name} (ID: {Id})", dto.Name, warehouse.Id);
                return ApiResponseDto<MasterDataItemDto>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "倉庫の作成中にエラーが発生しました: {Name}", dto.Name);
                return ApiResponseDto<MasterDataItemDto>.ErrorResponse("倉庫の作成に失敗しました");
            }
        }

        /// <summary>
        /// 倉庫の更新
        /// </summary>
        public async Task<ApiResponseDto<MasterDataItemDto>> UpdateWarehouseAsync(MasterDataUpdateDto dto, int userId)
        {
            try
            {
                // 1. Name非空チェック
                if (string.IsNullOrWhiteSpace(dto.Name))
                {
                    _logger.LogWarning("倉庫更新失敗: Nameが空です (Id: {Id}, UserId: {UserId})", dto.Id, userId);
                    return ApiResponseDto<MasterDataItemDto>.ErrorResponse("倉庫名は必須です");
                }

                var warehouse = await _context.Warehouses.FindAsync(dto.Id);
                if (warehouse == null)
                {
                    return ApiResponseDto<MasterDataItemDto>.ErrorResponse("指定された倉庫が見つかりません");
                }

                // 2. 一意性チェック（同じIdを除外）
                var existingWarehouse = await _context.Warehouses
                    .AnyAsync(w => w.Name == dto.Name && w.Id != dto.Id);
                if (existingWarehouse)
                {
                    _logger.LogWarning("倉庫更新失敗: 重複する倉庫名です - {Name} (Id: {Id}, UserId: {UserId})", dto.Name, dto.Id, userId);
                    return ApiResponseDto<MasterDataItemDto>.ErrorResponse($"倉庫名「{dto.Name}」は既に存在します");
                }

                warehouse.Name = dto.Name;
                warehouse.DisplayOrder = dto.DisplayOrder;
                warehouse.IsActive = dto.IsActive;
                warehouse.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var result = new MasterDataItemDto
                {
                    Id = warehouse.Id,
                    Name = warehouse.Name,
                    DisplayOrder = warehouse.DisplayOrder,
                    IsActive = warehouse.IsActive,
                    CreatedAt = warehouse.CreatedAt,
                    UpdatedAt = warehouse.UpdatedAt
                };

                _logger.LogInformation("倉庫を更新しました: {Name} (ID: {Id})", dto.Name, dto.Id);
                return ApiResponseDto<MasterDataItemDto>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "倉庫の更新中にエラーが発生しました: {Name} (ID: {Id})", dto.Name, dto.Id);
                return ApiResponseDto<MasterDataItemDto>.ErrorResponse("倉庫の更新に失敗しました");
            }
        }

        /// <summary>
        /// 倉庫の削除（論理削除）
        /// </summary>
        public async Task<ApiResponseDto<bool>> DeleteWarehouseAsync(int id, int userId)
        {
            try
            {
                var warehouse = await _context.Warehouses.FindAsync(id);
                if (warehouse == null)
                {
                    return ApiResponseDto<bool>.ErrorResponse("指定された倉庫が見つかりません");
                }

                // 参照整合性チェック - インシデントで参照されているかチェック
                var hasReferences = await _context.Incidents
                    .AnyAsync(i => i.ShippingWarehouse == id);
                if (hasReferences)
                {
                    _logger.LogWarning("倉庫削除失敗: インシデントで参照されています (Id: {Id}, UserId: {UserId})", id, userId);
                    return ApiResponseDto<bool>.ErrorResponse("この倉庫はインシデントで使用されているため削除できません");
                }

                warehouse.IsActive = false;
                warehouse.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("倉庫を削除しました: {Name} (ID: {Id})", warehouse.Name, id);
                return ApiResponseDto<bool>.SuccessResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "倉庫の削除中にエラーが発生しました: ID {Id}", id);
                return ApiResponseDto<bool>.ErrorResponse("倉庫の削除に失敗しました");
            }
        }

        // 他のマスタテーブルのCRUD操作も同様に実装...
        // ここでは主要な3つのテーブル（組織、発生場所、倉庫）のみ実装
        // 残りのテーブル（運送会社、トラブル区分、トラブル詳細区分、単位、システムパラメータ）も同様のパターンで実装可能

        // 残りのメソッドは一時的にNotImplementedExceptionをスローしてコンパイルエラーを回避
        /// <summary>
        /// 運送会社の作成
        /// </summary>
        public async Task<ApiResponseDto<MasterDataItemDto>> CreateShippingCompanyAsync(MasterDataCreateDto dto, int userId)
        {
            try
            {
                // 1. Name非空チェック
                if (string.IsNullOrWhiteSpace(dto.Name))
                {
                    _logger.LogWarning("運送会社作成失敗: Nameが空です (UserId: {UserId})", userId);
                    return ApiResponseDto<MasterDataItemDto>.ErrorResponse("運送会社名は必須です");
                }

                // 2. 一意性チェック
                var existingShippingCompany = await _context.ShippingCompanies
                    .AnyAsync(sc => sc.Name == dto.Name);
                if (existingShippingCompany)
                {
                    _logger.LogWarning("運送会社作成失敗: 重複する運送会社名です - {Name} (UserId: {UserId})", dto.Name, userId);
                    return ApiResponseDto<MasterDataItemDto>.ErrorResponse($"運送会社名「{dto.Name}」は既に存在します");
                }

                var shippingCompany = new ShippingCompany
                {
                    Name = dto.Name,
                    DisplayOrder = dto.DisplayOrder,
                    IsActive = dto.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.ShippingCompanies.Add(shippingCompany);
                await _context.SaveChangesAsync();

                var result = new MasterDataItemDto
                {
                    Id = shippingCompany.Id,
                    Name = shippingCompany.Name,
                    DisplayOrder = shippingCompany.DisplayOrder,
                    IsActive = shippingCompany.IsActive,
                    CreatedAt = shippingCompany.CreatedAt,
                    UpdatedAt = shippingCompany.UpdatedAt
                };

                _logger.LogInformation("運送会社を作成しました: {Name} (ID: {Id})", dto.Name, shippingCompany.Id);
                return ApiResponseDto<MasterDataItemDto>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "運送会社の作成中にエラーが発生しました: {Name}", dto.Name);
                return ApiResponseDto<MasterDataItemDto>.ErrorResponse("運送会社の作成に失敗しました");
            }
        }

        /// <summary>
        /// 運送会社の更新
        /// </summary>
        public async Task<ApiResponseDto<MasterDataItemDto>> UpdateShippingCompanyAsync(MasterDataUpdateDto dto, int userId)
        {
            try
            {
                // 1. Name非空チェック
                if (string.IsNullOrWhiteSpace(dto.Name))
                {
                    _logger.LogWarning("運送会社更新失敗: Nameが空です (Id: {Id}, UserId: {UserId})", dto.Id, userId);
                    return ApiResponseDto<MasterDataItemDto>.ErrorResponse("運送会社名は必須です");
                }

                var shippingCompany = await _context.ShippingCompanies.FindAsync(dto.Id);
                if (shippingCompany == null)
                {
                    return ApiResponseDto<MasterDataItemDto>.ErrorResponse("運送会社が見つかりません");
                }

                // 2. 一意性チェック（同じIdを除外）
                var existingShippingCompany = await _context.ShippingCompanies
                    .AnyAsync(sc => sc.Name == dto.Name && sc.Id != dto.Id);
                if (existingShippingCompany)
                {
                    _logger.LogWarning("運送会社更新失敗: 重複する運送会社名です - {Name} (Id: {Id}, UserId: {UserId})", dto.Name, dto.Id, userId);
                    return ApiResponseDto<MasterDataItemDto>.ErrorResponse($"運送会社名「{dto.Name}」は既に存在します");
                }

                shippingCompany.Name = dto.Name;
                shippingCompany.DisplayOrder = dto.DisplayOrder;
                shippingCompany.IsActive = dto.IsActive;
                shippingCompany.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var result = new MasterDataItemDto
                {
                    Id = shippingCompany.Id,
                    Name = shippingCompany.Name,
                    DisplayOrder = shippingCompany.DisplayOrder,
                    IsActive = shippingCompany.IsActive,
                    CreatedAt = shippingCompany.CreatedAt,
                    UpdatedAt = shippingCompany.UpdatedAt
                };

                _logger.LogInformation("運送会社を更新しました: {Name} (ID: {Id})", dto.Name, dto.Id);
                return ApiResponseDto<MasterDataItemDto>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "運送会社の更新中にエラーが発生しました: ID {Id}", dto.Id);
                return ApiResponseDto<MasterDataItemDto>.ErrorResponse("運送会社の更新に失敗しました");
            }
        }

        /// <summary>
        /// 運送会社の削除（論理削除）
        /// </summary>
        public async Task<ApiResponseDto<bool>> DeleteShippingCompanyAsync(int id, int userId)
        {
            try
            {
                var shippingCompany = await _context.ShippingCompanies.FindAsync(id);
                if (shippingCompany == null)
                {
                    return ApiResponseDto<bool>.ErrorResponse("運送会社が見つかりません");
                }

                // 参照整合性チェック - インシデントで参照されているかチェック
                var hasReferences = await _context.Incidents
                    .AnyAsync(i => i.ShippingCompany == id);
                if (hasReferences)
                {
                    _logger.LogWarning("運送会社削除失敗: インシデントで参照されています (Id: {Id}, UserId: {UserId})", id, userId);
                    return ApiResponseDto<bool>.ErrorResponse("この運送会社はインシデントで使用されているため削除できません");
                }

                shippingCompany.IsActive = false;
                shippingCompany.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("運送会社を削除しました: ID {Id}", id);
                return ApiResponseDto<bool>.SuccessResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "運送会社の削除中にエラーが発生しました: ID {Id}", id);
                return ApiResponseDto<bool>.ErrorResponse("運送会社の削除に失敗しました");
            }
        }

        /// <summary>
        /// トラブル区分の作成
        /// </summary>
        public async Task<ApiResponseDto<MasterDataItemDto>> CreateTroubleCategoryAsync(MasterDataCreateDto dto, int userId)
        {
            try
            {
                // 1. Name非空チェック
                if (string.IsNullOrWhiteSpace(dto.Name))
                {
                    _logger.LogWarning("トラブル区分作成失敗: Nameが空です (UserId: {UserId})", userId);
                    return ApiResponseDto<MasterDataItemDto>.ErrorResponse("トラブル区分名は必須です");
                }

                // 2. 一意性チェック
                var existingTroubleCategory = await _context.TroubleCategories
                    .AnyAsync(tc => tc.Name == dto.Name);
                if (existingTroubleCategory)
                {
                    _logger.LogWarning("トラブル区分作成失敗: 重複するトラブル区分名です - {Name} (UserId: {UserId})", dto.Name, userId);
                    return ApiResponseDto<MasterDataItemDto>.ErrorResponse($"トラブル区分名「{dto.Name}」は既に存在します");
                }

                var troubleCategory = new TroubleCategory
                {
                    Name = dto.Name,
                    DisplayOrder = dto.DisplayOrder,
                    IsActive = dto.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.TroubleCategories.Add(troubleCategory);
                await _context.SaveChangesAsync();

                var result = new MasterDataItemDto
                {
                    Id = troubleCategory.Id,
                    Name = troubleCategory.Name,
                    DisplayOrder = troubleCategory.DisplayOrder,
                    IsActive = troubleCategory.IsActive,
                    CreatedAt = troubleCategory.CreatedAt,
                    UpdatedAt = troubleCategory.UpdatedAt
                };

                _logger.LogInformation("トラブル区分を作成しました: {Name} (ID: {Id})", dto.Name, troubleCategory.Id);
                return ApiResponseDto<MasterDataItemDto>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "トラブル区分の作成中にエラーが発生しました: {Name}", dto.Name);
                return ApiResponseDto<MasterDataItemDto>.ErrorResponse("トラブル区分の作成に失敗しました");
            }
        }

        /// <summary>
        /// トラブル区分の更新
        /// </summary>
        public async Task<ApiResponseDto<MasterDataItemDto>> UpdateTroubleCategoryAsync(MasterDataUpdateDto dto, int userId)
        {
            try
            {
                // 1. Name非空チェック
                if (string.IsNullOrWhiteSpace(dto.Name))
                {
                    _logger.LogWarning("トラブル区分更新失敗: Nameが空です (Id: {Id}, UserId: {UserId})", dto.Id, userId);
                    return ApiResponseDto<MasterDataItemDto>.ErrorResponse("トラブル区分名は必須です");
                }

                var troubleCategory = await _context.TroubleCategories.FindAsync(dto.Id);
                if (troubleCategory == null)
                {
                    return ApiResponseDto<MasterDataItemDto>.ErrorResponse("トラブル区分が見つかりません");
                }

                // 2. 一意性チェック（同じIdを除外）
                var existingTroubleCategory = await _context.TroubleCategories
                    .AnyAsync(tc => tc.Name == dto.Name && tc.Id != dto.Id);
                if (existingTroubleCategory)
                {
                    _logger.LogWarning("トラブル区分更新失敗: 重複するトラブル区分名です - {Name} (Id: {Id}, UserId: {UserId})", dto.Name, dto.Id, userId);
                    return ApiResponseDto<MasterDataItemDto>.ErrorResponse($"トラブル区分名「{dto.Name}」は既に存在します");
                }

                troubleCategory.Name = dto.Name;
                troubleCategory.DisplayOrder = dto.DisplayOrder;
                troubleCategory.IsActive = dto.IsActive;
                troubleCategory.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var result = new MasterDataItemDto
                {
                    Id = troubleCategory.Id,
                    Name = troubleCategory.Name,
                    DisplayOrder = troubleCategory.DisplayOrder,
                    IsActive = troubleCategory.IsActive,
                    CreatedAt = troubleCategory.CreatedAt,
                    UpdatedAt = troubleCategory.UpdatedAt
                };

                _logger.LogInformation("トラブル区分を更新しました: {Name} (ID: {Id})", dto.Name, dto.Id);
                return ApiResponseDto<MasterDataItemDto>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "トラブル区分の更新中にエラーが発生しました: ID {Id}", dto.Id);
                return ApiResponseDto<MasterDataItemDto>.ErrorResponse("トラブル区分の更新に失敗しました");
            }
        }

        /// <summary>
        /// トラブル区分の削除（論理削除）
        /// </summary>
        public async Task<ApiResponseDto<bool>> DeleteTroubleCategoryAsync(int id, int userId)
        {
            try
            {
                var troubleCategory = await _context.TroubleCategories.FindAsync(id);
                if (troubleCategory == null)
                {
                    return ApiResponseDto<bool>.ErrorResponse("トラブル区分が見つかりません");
                }

                // 参照整合性チェック - インシデントまたはトラブル詳細区分で参照されているかチェック
                var hasIncidentReferences = await _context.Incidents
                    .AnyAsync(i => i.TroubleCategory == id);
                var hasDetailCategoryReferences = await _context.TroubleDetailCategories
                    .AnyAsync(tdc => tdc.TroubleCategoryId == id);
                
                if (hasIncidentReferences || hasDetailCategoryReferences)
                {
                    _logger.LogWarning("トラブル区分削除失敗: インシデントまたはトラブル詳細区分で参照されています (Id: {Id}, UserId: {UserId})", id, userId);
                    return ApiResponseDto<bool>.ErrorResponse("このトラブル区分はインシデントまたはトラブル詳細区分で使用されているため削除できません");
                }

                troubleCategory.IsActive = false;
                troubleCategory.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("トラブル区分を削除しました: ID {Id}", id);
                return ApiResponseDto<bool>.SuccessResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "トラブル区分の削除中にエラーが発生しました: ID {Id}", id);
                return ApiResponseDto<bool>.ErrorResponse("トラブル区分の削除に失敗しました");
            }
        }

        /// <summary>
        /// トラブル詳細区分の作成
        /// </summary>
        public async Task<ApiResponseDto<MasterDataItemDto>> CreateTroubleDetailCategoryAsync(TroubleDetailCategoryCreateDto dto, int userId)
        {
            try
            {
                // 1. Name非空チェック
                if (string.IsNullOrWhiteSpace(dto.Name))
                {
                    _logger.LogWarning("トラブル詳細区分作成失敗: Nameが空です (UserId: {UserId})", userId);
                    return ApiResponseDto<MasterDataItemDto>.ErrorResponse("トラブル詳細区分名は必須です");
                }

                // 2. TroubleCategoryId存在チェック
                var troubleCategoryExists = await _context.TroubleCategories
                    .AnyAsync(tc => tc.Id == dto.TroubleCategoryId);
                if (!troubleCategoryExists)
                {
                    _logger.LogWarning("トラブル詳細区分作成失敗: 存在しないトラブル区分IDです - {TroubleCategoryId} (UserId: {UserId})", dto.TroubleCategoryId, userId);
                    return ApiResponseDto<MasterDataItemDto>.ErrorResponse($"指定されたトラブル区分ID（{dto.TroubleCategoryId}）は存在しません");
                }

                // 3. 一意性チェック
                var existingTroubleDetailCategory = await _context.TroubleDetailCategories
                    .AnyAsync(tdc => tdc.Name == dto.Name);
                if (existingTroubleDetailCategory)
                {
                    _logger.LogWarning("トラブル詳細区分作成失敗: 重複するトラブル詳細区分名です - {Name} (UserId: {UserId})", dto.Name, userId);
                    return ApiResponseDto<MasterDataItemDto>.ErrorResponse($"トラブル詳細区分名「{dto.Name}」は既に存在します");
                }

                var troubleDetailCategory = new TroubleDetailCategory
                {
                    Name = dto.Name,
                    TroubleCategoryId = dto.TroubleCategoryId,
                    DisplayOrder = dto.DisplayOrder,
                    IsActive = dto.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.TroubleDetailCategories.Add(troubleDetailCategory);
                await _context.SaveChangesAsync();

                var result = new MasterDataItemDto
                {
                    Id = troubleDetailCategory.Id,
                    Name = troubleDetailCategory.Name,
                    DisplayOrder = troubleDetailCategory.DisplayOrder,
                    IsActive = troubleDetailCategory.IsActive,
                    CreatedAt = troubleDetailCategory.CreatedAt,
                    UpdatedAt = troubleDetailCategory.UpdatedAt
                };

                _logger.LogInformation("トラブル詳細区分を作成しました: {Name} (ID: {Id})", dto.Name, troubleDetailCategory.Id);
                return ApiResponseDto<MasterDataItemDto>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "トラブル詳細区分の作成中にエラーが発生しました: {Name}", dto.Name);
                return ApiResponseDto<MasterDataItemDto>.ErrorResponse("トラブル詳細区分の作成に失敗しました");
            }
        }

        /// <summary>
        /// トラブル詳細区分の更新
        /// </summary>
        public async Task<ApiResponseDto<MasterDataItemDto>> UpdateTroubleDetailCategoryAsync(TroubleDetailCategoryUpdateDto dto, int userId)
        {
            try
            {
                // 1. Name非空チェック
                if (string.IsNullOrWhiteSpace(dto.Name))
                {
                    _logger.LogWarning("トラブル詳細区分更新失敗: Nameが空です (Id: {Id}, UserId: {UserId})", dto.Id, userId);
                    return ApiResponseDto<MasterDataItemDto>.ErrorResponse("トラブル詳細区分名は必須です");
                }

                var troubleDetailCategory = await _context.TroubleDetailCategories.FindAsync(dto.Id);
                if (troubleDetailCategory == null)
                {
                    return ApiResponseDto<MasterDataItemDto>.ErrorResponse("トラブル詳細区分が見つかりません");
                }

                // 2. TroubleCategoryId存在チェック
                var troubleCategoryExists = await _context.TroubleCategories
                    .AnyAsync(tc => tc.Id == dto.TroubleCategoryId);
                if (!troubleCategoryExists)
                {
                    _logger.LogWarning("トラブル詳細区分更新失敗: 存在しないトラブル区分IDです - {TroubleCategoryId} (Id: {Id}, UserId: {UserId})", dto.TroubleCategoryId, dto.Id, userId);
                    return ApiResponseDto<MasterDataItemDto>.ErrorResponse($"指定されたトラブル区分ID（{dto.TroubleCategoryId}）は存在しません");
                }

                // 3. 一意性チェック（同じIdを除外）
                var existingTroubleDetailCategory = await _context.TroubleDetailCategories
                    .AnyAsync(tdc => tdc.Name == dto.Name && tdc.Id != dto.Id);
                if (existingTroubleDetailCategory)
                {
                    _logger.LogWarning("トラブル詳細区分更新失敗: 重複するトラブル詳細区分名です - {Name} (Id: {Id}, UserId: {UserId})", dto.Name, dto.Id, userId);
                    return ApiResponseDto<MasterDataItemDto>.ErrorResponse($"トラブル詳細区分名「{dto.Name}」は既に存在します");
                }

                troubleDetailCategory.Name = dto.Name;
                troubleDetailCategory.TroubleCategoryId = dto.TroubleCategoryId;
                troubleDetailCategory.DisplayOrder = dto.DisplayOrder;
                troubleDetailCategory.IsActive = dto.IsActive;
                troubleDetailCategory.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var result = new MasterDataItemDto
                {
                    Id = troubleDetailCategory.Id,
                    Name = troubleDetailCategory.Name,
                    DisplayOrder = troubleDetailCategory.DisplayOrder,
                    IsActive = troubleDetailCategory.IsActive,
                    CreatedAt = troubleDetailCategory.CreatedAt,
                    UpdatedAt = troubleDetailCategory.UpdatedAt
                };

                _logger.LogInformation("トラブル詳細区分を更新しました: {Name} (ID: {Id})", dto.Name, dto.Id);
                return ApiResponseDto<MasterDataItemDto>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "トラブル詳細区分の更新中にエラーが発生しました: ID {Id}", dto.Id);
                return ApiResponseDto<MasterDataItemDto>.ErrorResponse("トラブル詳細区分の更新に失敗しました");
            }
        }

        /// <summary>
        /// トラブル詳細区分の削除（論理削除）
        /// </summary>
        public async Task<ApiResponseDto<bool>> DeleteTroubleDetailCategoryAsync(int id, int userId)
        {
            try
            {
                var troubleDetailCategory = await _context.TroubleDetailCategories.FindAsync(id);
                if (troubleDetailCategory == null)
                {
                    return ApiResponseDto<bool>.ErrorResponse("トラブル詳細区分が見つかりません");
                }

                // 参照整合性チェック - インシデントで参照されているかチェック
                var hasReferences = await _context.Incidents
                    .AnyAsync(i => i.TroubleDetailCategory == id);
                if (hasReferences)
                {
                    _logger.LogWarning("トラブル詳細区分削除失敗: インシデントで参照されています (Id: {Id}, UserId: {UserId})", id, userId);
                    return ApiResponseDto<bool>.ErrorResponse("このトラブル詳細区分はインシデントで使用されているため削除できません");
                }

                troubleDetailCategory.IsActive = false;
                troubleDetailCategory.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("トラブル詳細区分を削除しました: ID {Id}", id);
                return ApiResponseDto<bool>.SuccessResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "トラブル詳細区分の削除中にエラーが発生しました: ID {Id}", id);
                return ApiResponseDto<bool>.ErrorResponse("トラブル詳細区分の削除に失敗しました");
            }
        }

        /// <summary>
        /// 単位の作成
        /// </summary>
        public async Task<ApiResponseDto<MasterDataItemDto>> CreateUnitAsync(UnitCreateDto dto, int userId)
        {
            try
            {
                // 1. Name非空チェック
                if (string.IsNullOrWhiteSpace(dto.Name))
                {
                    _logger.LogWarning("単位作成失敗: Nameが空です (UserId: {UserId})", userId);
                    return ApiResponseDto<MasterDataItemDto>.ErrorResponse("単位名は必須です");
                }

                // 2. 一意性チェック（Name）
                var existingUnitByName = await _context.Units
                    .AnyAsync(u => u.Name == dto.Name);
                if (existingUnitByName)
                {
                    _logger.LogWarning("単位作成失敗: 重複する単位名です - {Name} (UserId: {UserId})", dto.Name, userId);
                    return ApiResponseDto<MasterDataItemDto>.ErrorResponse($"単位名「{dto.Name}」は既に存在します");
                }

                // 3. 一意性チェック（Code）
                var existingUnitByCode = await _context.Units
                    .AnyAsync(u => u.Code == dto.Code);
                if (existingUnitByCode)
                {
                    _logger.LogWarning("単位作成失敗: 重複する単位コードです - {Code} (UserId: {UserId})", dto.Code, userId);
                    return ApiResponseDto<MasterDataItemDto>.ErrorResponse($"単位コード「{dto.Code}」は既に存在します");
                }

                var unit = new Unit
                {
                    Code = dto.Code,
                    Name = dto.Name,
                    DisplayOrder = dto.DisplayOrder,
                    IsActive = dto.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Units.Add(unit);
                await _context.SaveChangesAsync();

                var result = new MasterDataItemDto
                {
                    Id = unit.Id,
                    Name = unit.Name,
                    DisplayOrder = unit.DisplayOrder,
                    IsActive = unit.IsActive,
                    CreatedAt = unit.CreatedAt,
                    UpdatedAt = unit.UpdatedAt
                };

                _logger.LogInformation("単位を作成しました: {Name} (ID: {Id})", dto.Name, unit.Id);
                return ApiResponseDto<MasterDataItemDto>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "単位の作成中にエラーが発生しました: {Name}", dto.Name);
                return ApiResponseDto<MasterDataItemDto>.ErrorResponse("単位の作成に失敗しました");
            }
        }

        /// <summary>
        /// 単位の更新
        /// </summary>
        public async Task<ApiResponseDto<MasterDataItemDto>> UpdateUnitAsync(UnitUpdateDto dto, int userId)
        {
            try
            {
                // 1. Name非空チェック
                if (string.IsNullOrWhiteSpace(dto.Name))
                {
                    _logger.LogWarning("単位更新失敗: Nameが空です (Id: {Id}, UserId: {UserId})", dto.Id, userId);
                    return ApiResponseDto<MasterDataItemDto>.ErrorResponse("単位名は必須です");
                }

                var unit = await _context.Units.FindAsync(dto.Id);
                if (unit == null)
                {
                    return ApiResponseDto<MasterDataItemDto>.ErrorResponse("単位が見つかりません");
                }

                // 2. 一意性チェック（Name、同じIdを除外）
                var existingUnitByName = await _context.Units
                    .AnyAsync(u => u.Name == dto.Name && u.Id != dto.Id);
                if (existingUnitByName)
                {
                    _logger.LogWarning("単位更新失敗: 重複する単位名です - {Name} (Id: {Id}, UserId: {UserId})", dto.Name, dto.Id, userId);
                    return ApiResponseDto<MasterDataItemDto>.ErrorResponse($"単位名「{dto.Name}」は既に存在します");
                }

                // 3. 一意性チェック（Code、同じIdを除外）
                var existingUnitByCode = await _context.Units
                    .AnyAsync(u => u.Code == dto.Code && u.Id != dto.Id);
                if (existingUnitByCode)
                {
                    _logger.LogWarning("単位更新失敗: 重複する単位コードです - {Code} (Id: {Id}, UserId: {UserId})", dto.Code, dto.Id, userId);
                    return ApiResponseDto<MasterDataItemDto>.ErrorResponse($"単位コード「{dto.Code}」は既に存在します");
                }

                unit.Code = dto.Code;
                unit.Name = dto.Name;
                unit.DisplayOrder = dto.DisplayOrder;
                unit.IsActive = dto.IsActive;
                unit.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var result = new MasterDataItemDto
                {
                    Id = unit.Id,
                    Name = unit.Name,
                    DisplayOrder = unit.DisplayOrder,
                    IsActive = unit.IsActive,
                    CreatedAt = unit.CreatedAt,
                    UpdatedAt = unit.UpdatedAt
                };

                _logger.LogInformation("単位を更新しました: {Name} (ID: {Id})", dto.Name, dto.Id);
                return ApiResponseDto<MasterDataItemDto>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "単位の更新中にエラーが発生しました: ID {Id}", dto.Id);
                return ApiResponseDto<MasterDataItemDto>.ErrorResponse("単位の更新に失敗しました");
            }
        }

        /// <summary>
        /// 単位の削除（論理削除）
        /// </summary>
        public async Task<ApiResponseDto<bool>> DeleteUnitAsync(int id, int userId)
        {
            try
            {
                var unit = await _context.Units.FindAsync(id);
                if (unit == null)
                {
                    return ApiResponseDto<bool>.ErrorResponse("単位が見つかりません");
                }

                // 参照整合性チェック - インシデントで参照されているかチェック
                var hasReferences = await _context.Incidents
                    .AnyAsync(i => i.Unit == id);
                if (hasReferences)
                {
                    _logger.LogWarning("単位削除失敗: インシデントで参照されています (Id: {Id}, UserId: {UserId})", id, userId);
                    return ApiResponseDto<bool>.ErrorResponse("この単位はインシデントで使用されているため削除できません");
                }

                unit.IsActive = false;
                unit.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("単位を削除しました: ID {Id}", id);
                return ApiResponseDto<bool>.SuccessResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "単位の削除中にエラーが発生しました: ID {Id}", id);
                return ApiResponseDto<bool>.ErrorResponse("単位の削除に失敗しました");
            }
        }

        /// <summary>
        /// システムパラメータの作成
        /// </summary>
        public async Task<ApiResponseDto<MasterDataItemDto>> CreateSystemParameterAsync(SystemParameterCreateDto dto, int userId)
        {
            try
            {
                // 1. ParameterKey非空チェック
                if (string.IsNullOrWhiteSpace(dto.ParameterKey))
                {
                    _logger.LogWarning("システムパラメータ作成失敗: ParameterKeyが空です (UserId: {UserId})", userId);
                    return ApiResponseDto<MasterDataItemDto>.ErrorResponse("パラメータキーは必須です");
                }

                // 2. 一意性チェック（ParameterKey）
                var existingSystemParameter = await _context.SystemParameters
                    .AnyAsync(sp => sp.ParameterKey == dto.ParameterKey);
                if (existingSystemParameter)
                {
                    _logger.LogWarning("システムパラメータ作成失敗: 重複するパラメータキーです - {ParameterKey} (UserId: {UserId})", dto.ParameterKey, userId);
                    return ApiResponseDto<MasterDataItemDto>.ErrorResponse($"パラメータキー「{dto.ParameterKey}」は既に存在します");
                }

                var systemParameter = new SystemParameter
                {
                    Name = dto.Name,
                    ParameterKey = dto.ParameterKey,
                    ParameterValue = dto.ParameterValue,
                    Description = dto.Description,
                    DataType = dto.DataType,
                    DisplayOrder = dto.DisplayOrder,
                    IsActive = dto.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.SystemParameters.Add(systemParameter);
                await _context.SaveChangesAsync();

                var result = new MasterDataItemDto
                {
                    Id = systemParameter.Id,
                    Name = systemParameter.ParameterKey, // パラメータキーを名前として使用
                    DisplayOrder = systemParameter.DisplayOrder,
                    IsActive = systemParameter.IsActive,
                    CreatedAt = systemParameter.CreatedAt,
                    UpdatedAt = systemParameter.UpdatedAt
                };

                _logger.LogInformation("システムパラメータを作成しました: {ParameterKey} (ID: {Id})", dto.ParameterKey, systemParameter.Id);
                return ApiResponseDto<MasterDataItemDto>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "システムパラメータの作成中にエラーが発生しました: {ParameterKey}", dto.ParameterKey);
                return ApiResponseDto<MasterDataItemDto>.ErrorResponse("システムパラメータの作成に失敗しました");
            }
        }

        /// <summary>
        /// システムパラメータの更新
        /// </summary>
        public async Task<ApiResponseDto<MasterDataItemDto>> UpdateSystemParameterAsync(SystemParameterUpdateDto dto, int userId)
        {
            try
            {
                // 1. ParameterKey非空チェック
                if (string.IsNullOrWhiteSpace(dto.ParameterKey))
                {
                    _logger.LogWarning("システムパラメータ更新失敗: ParameterKeyが空です (Id: {Id}, UserId: {UserId})", dto.Id, userId);
                    return ApiResponseDto<MasterDataItemDto>.ErrorResponse("パラメータキーは必須です");
                }

                var systemParameter = await _context.SystemParameters.FindAsync(dto.Id);
                if (systemParameter == null)
                {
                    return ApiResponseDto<MasterDataItemDto>.ErrorResponse("システムパラメータが見つかりません");
                }

                // 2. 一意性チェック（ParameterKey、同じIdを除外）
                var existingSystemParameter = await _context.SystemParameters
                    .AnyAsync(sp => sp.ParameterKey == dto.ParameterKey && sp.Id != dto.Id);
                if (existingSystemParameter)
                {
                    _logger.LogWarning("システムパラメータ更新失敗: 重複するパラメータキーです - {ParameterKey} (Id: {Id}, UserId: {UserId})", dto.ParameterKey, dto.Id, userId);
                    return ApiResponseDto<MasterDataItemDto>.ErrorResponse($"パラメータキー「{dto.ParameterKey}」は既に存在します");
                }

                systemParameter.Name = dto.Name;
                systemParameter.ParameterKey = dto.ParameterKey;
                systemParameter.ParameterValue = dto.ParameterValue;
                systemParameter.Description = dto.Description;
                systemParameter.DataType = dto.DataType;
                systemParameter.DisplayOrder = dto.DisplayOrder;
                systemParameter.IsActive = dto.IsActive;
                systemParameter.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var result = new MasterDataItemDto
                {
                    Id = systemParameter.Id,
                    Name = systemParameter.Name,
                    DisplayOrder = systemParameter.DisplayOrder,
                    IsActive = systemParameter.IsActive,
                    CreatedAt = systemParameter.CreatedAt,
                    UpdatedAt = systemParameter.UpdatedAt
                };

                _logger.LogInformation("システムパラメータを更新しました: {ParameterKey} (ID: {Id})", dto.ParameterKey, dto.Id);
                return ApiResponseDto<MasterDataItemDto>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "システムパラメータの更新中にエラーが発生しました: ID {Id}", dto.Id);
                return ApiResponseDto<MasterDataItemDto>.ErrorResponse("システムパラメータの更新に失敗しました");
            }
        }

        /// <summary>
        /// システムパラメータの削除（論理削除）
        /// </summary>
        public async Task<ApiResponseDto<bool>> DeleteSystemParameterAsync(int id, int userId)
        {
            try
            {
                var systemParameter = await _context.SystemParameters.FindAsync(id);
                if (systemParameter == null)
                {
                    return ApiResponseDto<bool>.ErrorResponse("システムパラメータが見つかりません");
                }

                // システムパラメータは他のテーブルから参照されていないため、参照整合性チェックは不要
                // ただし、重要なシステムパラメータの削除を防ぐための追加チェックは可能

                systemParameter.IsActive = false;
                systemParameter.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("システムパラメータを削除しました: ID {Id}", id);
                return ApiResponseDto<bool>.SuccessResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "システムパラメータの削除中にエラーが発生しました: ID {Id}", id);
                return ApiResponseDto<bool>.ErrorResponse("システムパラメータの削除に失敗しました");
            }
        }

        // =============================================
        // ユーザーロール管理メソッド
        // =============================================

        /// <summary>
        /// ユーザーロールの作成
        /// </summary>
        public async Task<ApiResponseDto<MasterDataItemDto>> CreateUserRoleAsync(UserRoleCreateDto dto, int userId)
        {
            try
            {
                // ロール名の重複チェック
                var existingRole = await _context.UserRoles
                    .FirstOrDefaultAsync(r => r.RoleName == dto.RoleName);

                if (existingRole != null)
                {
                    return ApiResponseDto<MasterDataItemDto>.ErrorResponse("同じロール名が既に存在します");
                }

                var userRole = new UserRole
                {
                    RoleName = dto.RoleName,
                    DisplayOrder = dto.DisplayOrder,
                    CreatedAt = DateTime.UtcNow
                };

                _context.UserRoles.Add(userRole);
                await _context.SaveChangesAsync();

                var result = new MasterDataItemDto
                {
                    Id = userRole.Id,
                    Name = userRole.RoleName,
                    DisplayOrder = userRole.DisplayOrder,
                    IsActive = true, // UserRoleにはIsActiveフィールドがないためtrueを設定
                    CreatedAt = userRole.CreatedAt,
                    UpdatedAt = null // UserRoleにはUpdatedAtフィールドがないためnullを設定
                };

                _logger.LogInformation("ユーザーロールを作成しました: ID {Id}, ロール名 {RoleName}", userRole.Id, userRole.RoleName);
                return ApiResponseDto<MasterDataItemDto>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ユーザーロールの作成中にエラーが発生しました: ロール名 {RoleName}", dto.RoleName);
                return ApiResponseDto<MasterDataItemDto>.ErrorResponse("ユーザーロールの作成に失敗しました");
            }
        }

        /// <summary>
        /// ユーザーロールの更新
        /// </summary>
        public async Task<ApiResponseDto<MasterDataItemDto>> UpdateUserRoleAsync(UserRoleUpdateDto dto, int userId)
        {
            try
            {
                var userRole = await _context.UserRoles.FindAsync(dto.Id);
                if (userRole == null)
                {
                    return ApiResponseDto<MasterDataItemDto>.ErrorResponse("指定されたユーザーロールが見つかりません");
                }

                // ロール名の重複チェック（自分以外）
                var existingRole = await _context.UserRoles
                    .FirstOrDefaultAsync(r => r.RoleName == dto.RoleName && r.Id != dto.Id);

                if (existingRole != null)
                {
                    return ApiResponseDto<MasterDataItemDto>.ErrorResponse("同じロール名が既に存在します");
                }

                // システム管理者ロール（ID=1）の変更を制限
                if (dto.Id == 1)
                {
                    return ApiResponseDto<MasterDataItemDto>.ErrorResponse("システム管理者ロールは変更できません");
                }

                userRole.RoleName = dto.RoleName;
                userRole.DisplayOrder = dto.DisplayOrder;
                // UserRoleにはUpdatedAtフィールドがないため、更新日時の設定は行わない

                await _context.SaveChangesAsync();

                var result = new MasterDataItemDto
                {
                    Id = userRole.Id,
                    Name = userRole.RoleName,
                    DisplayOrder = userRole.DisplayOrder,
                    IsActive = true, // UserRoleにはIsActiveフィールドがないためtrueを設定
                    CreatedAt = userRole.CreatedAt,
                    UpdatedAt = null // UserRoleにはUpdatedAtフィールドがないためnullを設定
                };

                _logger.LogInformation("ユーザーロールを更新しました: ID {Id}, ロール名 {RoleName}", userRole.Id, userRole.RoleName);
                return ApiResponseDto<MasterDataItemDto>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ユーザーロールの更新中にエラーが発生しました: ID {Id}, ロール名 {RoleName}", dto.Id, dto.RoleName);
                return ApiResponseDto<MasterDataItemDto>.ErrorResponse("ユーザーロールの更新に失敗しました");
            }
        }

        /// <summary>
        /// ユーザーロールの削除
        /// </summary>
        public async Task<ApiResponseDto<bool>> DeleteUserRoleAsync(int id, int userId)
        {
            try
            {
                var userRole = await _context.UserRoles.FindAsync(id);
                if (userRole == null)
                {
                    return ApiResponseDto<bool>.ErrorResponse("指定されたユーザーロールが見つかりません");
                }

                // システム管理者ロール（ID=1）の削除を制限
                if (id == 1)
                {
                    return ApiResponseDto<bool>.ErrorResponse("システム管理者ロールは削除できません");
                }

                // このロールを使用しているユーザーがいるかチェック
                var usersWithRole = await _context.Users
                    .AnyAsync(u => u.UserRoleId == id);

                if (usersWithRole)
                {
                    return ApiResponseDto<bool>.ErrorResponse("このロールを使用しているユーザーが存在するため削除できません");
                }

                _context.UserRoles.Remove(userRole);
                await _context.SaveChangesAsync();

                _logger.LogInformation("ユーザーロールを削除しました: ID {Id}, ロール名 {RoleName}", userRole.Id, userRole.RoleName);
                return ApiResponseDto<bool>.SuccessResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ユーザーロールの削除中にエラーが発生しました: ID {Id}", id);
                return ApiResponseDto<bool>.ErrorResponse("ユーザーロールの削除に失敗しました");
            }
        }
    }
}
