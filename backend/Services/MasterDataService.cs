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
                    .OrderBy(o => o.Id)
                    .Select(o => new MasterDataItemDto
                    {
                        Id = o.Id,
                        Name = o.Name,
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
                    .OrderBy(l => l.Id)
                    .Select(l => new MasterDataItemDto
                    {
                        Id = l.Id,
                        Name = l.Name,
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
        /// 出荷元倉庫一覧の取得
        /// </summary>
        public async Task<ApiResponseDto<MasterDataItemDto[]>> GetShippingWarehousesAsync()
        {
            try
            {
                var warehouses = await _context.Warehouses
                    .Where(w => w.IsActive)
                    .OrderBy(w => w.Id)
                    .Select(w => new MasterDataItemDto
                    {
                        Id = w.Id,
                        Name = w.Name,
                        IsActive = w.IsActive,
                        CreatedAt = w.CreatedAt,
                        UpdatedAt = w.UpdatedAt
                    })
                    .ToArrayAsync();

                return ApiResponseDto<MasterDataItemDto[]>.SuccessResponse(warehouses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "出荷元倉庫一覧の取得中にエラーが発生しました");
                return ApiResponseDto<MasterDataItemDto[]>.ErrorResponse("出荷元倉庫一覧の取得に失敗しました");
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
                    .OrderBy(c => c.Id)
                    .Select(c => new MasterDataItemDto
                    {
                        Id = c.Id,
                        Name = c.Name,
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
                    .OrderBy(c => c.Id)
                    .Select(c => new MasterDataItemDto
                    {
                        Id = c.Id,
                        Name = c.Name,
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
                    .OrderBy(dc => dc.Id)
                    .Select(dc => new TroubleDetailCategoryItemDto
                    {
                        Id = dc.Id,
                        Name = dc.Name,
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
                    .OrderBy(r => r.Id)
                    .Select(r => new MasterDataItemDto
                    {
                        Id = r.Id,
                        Name = r.RoleName,
                        IsActive = true, // UserRoleにはIsActiveフィールドがないためtrueを設定
                        CreatedAt = DateTime.UtcNow, // UserRoleには作成日時フィールドがないため現在時刻を設定
                        UpdatedAt = DateTime.UtcNow  // UserRoleには更新日時フィールドがないため現在時刻を設定
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
                    .OrderBy(w => w.Id)
                    .ToListAsync();

                var result = warehouses.Select(w => new MasterDataItemDto
                {
                    Id = w.Id,
                    Name = w.Name,
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
                    .OrderBy(u => u.Id)
                    .ToListAsync();

                var result = units.Select(u => new UnitItemDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Code = u.Code,
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
                    .OrderBy(sp => sp.Id)
                    .ToListAsync();

                var result = systemParameters.Select(sp => new SystemParameterItemDto
                {
                    Id = sp.Id,
                    Name = sp.ParameterKey, // パラメータキーを名前として使用
                    ParameterKey = sp.ParameterKey,
                    ParameterValue = sp.ParameterValue,
                    Description = sp.Description,
                    DataType = sp.DataType,
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
                var organization = new Organization
                {
                    Name = dto.Name,
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
                var organization = await _context.Organizations.FindAsync(dto.Id);
                if (organization == null)
                {
                    return ApiResponseDto<MasterDataItemDto>.ErrorResponse("指定された組織が見つかりません");
                }

                organization.Name = dto.Name;
                organization.IsActive = dto.IsActive;
                organization.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var result = new MasterDataItemDto
                {
                    Id = organization.Id,
                    Name = organization.Name,
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
                var location = new OccurrenceLocation
                {
                    Name = dto.Name,
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
                var location = await _context.OccurrenceLocations.FindAsync(dto.Id);
                if (location == null)
                {
                    return ApiResponseDto<MasterDataItemDto>.ErrorResponse("指定された発生場所が見つかりません");
                }

                location.Name = dto.Name;
                location.IsActive = dto.IsActive;
                location.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var result = new MasterDataItemDto
                {
                    Id = location.Id,
                    Name = location.Name,
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
                var warehouse = new Warehouse
                {
                    Name = dto.Name,
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
                var warehouse = await _context.Warehouses.FindAsync(dto.Id);
                if (warehouse == null)
                {
                    return ApiResponseDto<MasterDataItemDto>.ErrorResponse("指定された倉庫が見つかりません");
                }

                warehouse.Name = dto.Name;
                warehouse.IsActive = dto.IsActive;
                warehouse.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var result = new MasterDataItemDto
                {
                    Id = warehouse.Id,
                    Name = warehouse.Name,
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
                var shippingCompany = new ShippingCompany
                {
                    Name = dto.Name,
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
                var shippingCompany = await _context.ShippingCompanies.FindAsync(dto.Id);
                if (shippingCompany == null)
                {
                    return ApiResponseDto<MasterDataItemDto>.ErrorResponse("運送会社が見つかりません");
                }

                shippingCompany.Name = dto.Name;
                shippingCompany.IsActive = dto.IsActive;
                shippingCompany.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var result = new MasterDataItemDto
                {
                    Id = shippingCompany.Id,
                    Name = shippingCompany.Name,
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
                var troubleCategory = new TroubleCategory
                {
                    Name = dto.Name,
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
                var troubleCategory = await _context.TroubleCategories.FindAsync(dto.Id);
                if (troubleCategory == null)
                {
                    return ApiResponseDto<MasterDataItemDto>.ErrorResponse("トラブル区分が見つかりません");
                }

                troubleCategory.Name = dto.Name;
                troubleCategory.IsActive = dto.IsActive;
                troubleCategory.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var result = new MasterDataItemDto
                {
                    Id = troubleCategory.Id,
                    Name = troubleCategory.Name,
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
                var troubleDetailCategory = new TroubleDetailCategory
                {
                    Name = dto.Name,
                    TroubleCategoryId = dto.TroubleCategoryId,
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
                var troubleDetailCategory = await _context.TroubleDetailCategories.FindAsync(dto.Id);
                if (troubleDetailCategory == null)
                {
                    return ApiResponseDto<MasterDataItemDto>.ErrorResponse("トラブル詳細区分が見つかりません");
                }

                troubleDetailCategory.Name = dto.Name;
                troubleDetailCategory.TroubleCategoryId = dto.TroubleCategoryId;
                troubleDetailCategory.IsActive = dto.IsActive;
                troubleDetailCategory.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var result = new MasterDataItemDto
                {
                    Id = troubleDetailCategory.Id,
                    Name = troubleDetailCategory.Name,
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
                var unit = new Unit
                {
                    Code = dto.Code,
                    Name = dto.Name,
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
                var unit = await _context.Units.FindAsync(dto.Id);
                if (unit == null)
                {
                    return ApiResponseDto<MasterDataItemDto>.ErrorResponse("単位が見つかりません");
                }

                unit.Code = dto.Code;
                unit.Name = dto.Name;
                unit.IsActive = dto.IsActive;
                unit.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var result = new MasterDataItemDto
                {
                    Id = unit.Id,
                    Name = unit.Name,
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
                var systemParameter = new SystemParameter
                {
                    ParameterKey = dto.ParameterKey,
                    ParameterValue = dto.ParameterValue,
                    Description = dto.Description,
                    DataType = dto.DataType,
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
                var systemParameter = await _context.SystemParameters.FindAsync(dto.Id);
                if (systemParameter == null)
                {
                    return ApiResponseDto<MasterDataItemDto>.ErrorResponse("システムパラメータが見つかりません");
                }

                systemParameter.ParameterKey = dto.ParameterKey;
                systemParameter.ParameterValue = dto.ParameterValue;
                systemParameter.Description = dto.Description;
                systemParameter.DataType = dto.DataType;
                systemParameter.IsActive = dto.IsActive;
                systemParameter.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var result = new MasterDataItemDto
                {
                    Id = systemParameter.Id,
                    Name = systemParameter.ParameterKey, // パラメータキーを名前として使用
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
    }
}
