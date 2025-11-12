using AutoMapper;
using LogisticsTroubleManagement.Data;
using LogisticsTroubleManagement.DTOs;
using LogisticsTroubleManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace LogisticsTroubleManagement.Services
{
    /// <summary>
    /// インシデントサービスの実装
    /// </summary>
    public class IncidentService : IIncidentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<IncidentService> _logger;
        private readonly IIncidentStatusCalculationService _statusCalculationService;

        public IncidentService(
            ApplicationDbContext context,
            IMapper mapper,
            ILogger<IncidentService> logger,
            IIncidentStatusCalculationService statusCalculationService)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _statusCalculationService = statusCalculationService;
        }

        /// <summary>
        /// インシデント一覧の取得
        /// </summary>
        public async Task<PagedApiResponseDto<IncidentResponseDto>> GetIncidentsAsync(IncidentSearchDto searchDto)
        {
            try
            {
                var query = _context.Incidents.AsQueryable();

                // 検索条件の適用
                if (!string.IsNullOrEmpty(searchDto.Search))
                {
                    var searchTerm = searchDto.Search.ToLower();
                    query = query.Where(i =>
                        i.Details.ToLower().Contains(searchTerm) ||
                        i.VoucherNumber != null && i.VoucherNumber.ToLower().Contains(searchTerm) ||
                        i.CustomerCode != null && i.CustomerCode.ToLower().Contains(searchTerm) ||
                        i.ProductCode != null && i.ProductCode.ToLower().Contains(searchTerm));
                }

                if (searchDto.Year.HasValue)
                {
                    query = query.Where(i => i.OccurrenceDateTime.Year == searchDto.Year.Value);
                }

                if (searchDto.Month.HasValue)
                {
                    query = query.Where(i => i.OccurrenceDateTime.Month == searchDto.Month.Value);
                }

                if (searchDto.Warehouse.HasValue)
                {
                    query = query.Where(i => i.ShippingWarehouse == searchDto.Warehouse.Value);
                }


                if (searchDto.TroubleCategory.HasValue)
                {
                    query = query.Where(i => i.TroubleCategory == searchDto.TroubleCategory.Value);
                }

                // 総件数の取得
                var total = await query.CountAsync();

                // ページネーションの適用
                var incidents = await query
                    .OrderByDescending(i => i.OccurrenceDateTime)
                    .Skip((searchDto.Page - 1) * searchDto.Limit)
                    .Take(searchDto.Limit)
                    .ToListAsync();

                // 期限切れの可能性があるキャッシュをクリア
                _statusCalculationService.ClearExpiredIncidentStatusCache(incidents);

                // 動的ステータス計算
                var statuses = await _statusCalculationService.CalculateIncidentStatusesAsync(incidents);

                // DTOにマッピング（ステータスを含む）
                var incidentDtos = incidents.Select(incident => 
                {
                    var dto = _mapper.Map<IncidentResponseDto>(incident);
                    dto.Status = statuses[incident.Id];
                    return dto;
                }).ToList();

                return PagedApiResponseDto<IncidentResponseDto>.SuccessResponse(
                    incidentDtos, searchDto.Page, searchDto.Limit, total);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "インシデント一覧の取得中にエラーが発生しました。Page={Page}, Limit={Limit}", 
                    searchDto.Page, searchDto.Limit);
                return new PagedApiResponseDto<IncidentResponseDto>
                {
                    Success = false,
                    ErrorMessage = $"インシデント一覧の取得に失敗しました: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// インシデントの取得
        /// </summary>
        public async Task<ApiResponseDto<IncidentResponseDto>> GetIncidentAsync(int id)
        {
            try
            {
                var incident = await _context.Incidents
                    .FirstOrDefaultAsync(i => i.Id == id);

                if (incident == null)
                {
                    return ApiResponseDto<IncidentResponseDto>.ErrorResponse("インシデントが見つかりません");
                }

                var incidentDto = _mapper.Map<IncidentResponseDto>(incident);
                incidentDto.Status = _statusCalculationService.CalculateIncidentStatus(incident);
                return ApiResponseDto<IncidentResponseDto>.SuccessResponse(incidentDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "インシデントの取得中にエラーが発生しました: {IncidentId}", id);
                return ApiResponseDto<IncidentResponseDto>.ErrorResponse("インシデントの取得に失敗しました");
            }
        }

        /// <summary>
        /// インシデントの作成
        /// </summary>
        public async Task<ApiResponseDto<IncidentResponseDto>> CreateIncidentAsync(CreateIncidentDto createDto, int userId)
        {
            try
            {
                var incident = _mapper.Map<Incident>(createDto);
                // IDは自動生成されるため設定不要
                incident.CreatedBy = userId;
                incident.UpdatedBy = userId;
                incident.CreatedAt = DateTime.UtcNow;
                incident.UpdatedAt = DateTime.UtcNow;

                _context.Incidents.Add(incident);
                await _context.SaveChangesAsync();

                var incidentDto = _mapper.Map<IncidentResponseDto>(incident);
                incidentDto.Status = _statusCalculationService.CalculateIncidentStatus(incident);
                return ApiResponseDto<IncidentResponseDto>.SuccessResponse(incidentDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "インシデントの作成中にエラーが発生しました");
                return ApiResponseDto<IncidentResponseDto>.ErrorResponse("インシデントの作成に失敗しました");
            }
        }

        /// <summary>
        /// インシデントの更新
        /// </summary>
        public async Task<ApiResponseDto<IncidentResponseDto>> UpdateIncidentAsync(int id, UpdateIncidentDto updateDto, int userId)
        {
            try
            {
                var incident = await _context.Incidents
                    .FirstOrDefaultAsync(i => i.Id == id);

                if (incident == null)
                {
                    return ApiResponseDto<IncidentResponseDto>.ErrorResponse("インシデントが見つかりません");
                }

                // 更新可能なプロパティのみ更新
                if (updateDto.CreationDate.HasValue)
                    incident.CreationDate = updateDto.CreationDate.Value;
                if (updateDto.Organization.HasValue)
                    incident.Organization = updateDto.Organization.Value;
                if (!string.IsNullOrEmpty(updateDto.Creator))
                    incident.Creator = updateDto.Creator;
                if (updateDto.OccurrenceDateTime.HasValue)
                    incident.OccurrenceDateTime = updateDto.OccurrenceDateTime.Value;
                if (updateDto.OccurrenceLocation.HasValue)
                    incident.OccurrenceLocation = updateDto.OccurrenceLocation.Value;
                if (updateDto.ShippingWarehouse.HasValue)
                    incident.ShippingWarehouse = updateDto.ShippingWarehouse.Value;
                if (updateDto.ShippingCompany.HasValue)
                    incident.ShippingCompany = updateDto.ShippingCompany.Value;
                if (updateDto.TroubleCategory.HasValue)
                    incident.TroubleCategory = updateDto.TroubleCategory.Value;
                if (updateDto.TroubleDetailCategory.HasValue)
                    incident.TroubleDetailCategory = updateDto.TroubleDetailCategory.Value;
                if (!string.IsNullOrEmpty(updateDto.Details))
                    incident.Details = updateDto.Details;
                if (updateDto.VoucherNumber != null)
                    incident.VoucherNumber = updateDto.VoucherNumber;
                if (updateDto.CustomerCode != null)
                    incident.CustomerCode = updateDto.CustomerCode;
                if (updateDto.ProductCode != null)
                    incident.ProductCode = updateDto.ProductCode;
                if (updateDto.Quantity.HasValue)
                    incident.Quantity = updateDto.Quantity.Value;
                if (updateDto.Unit.HasValue)
                    incident.Unit = updateDto.Unit.Value;

                // 2次情報
                if (updateDto.InputDate.HasValue)
                    incident.InputDate = updateDto.InputDate.Value;
                if (updateDto.ProcessDescription != null)
                    incident.ProcessDescription = updateDto.ProcessDescription;
                if (updateDto.Cause != null)
                    incident.Cause = updateDto.Cause;
                if (updateDto.PhotoDataUri != null)
                    incident.PhotoDataUri = updateDto.PhotoDataUri;

                // 3次情報
                if (updateDto.InputDate3.HasValue)
                    incident.InputDate3 = updateDto.InputDate3.Value;
                if (updateDto.RecurrencePreventionMeasures != null)
                    incident.RecurrencePreventionMeasures = updateDto.RecurrencePreventionMeasures;

                incident.UpdatedBy = userId;
                incident.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                // ステータスキャッシュをクリア
                _statusCalculationService.ClearIncidentStatusCache(id);

                var incidentDto = _mapper.Map<IncidentResponseDto>(incident);
                incidentDto.Status = _statusCalculationService.CalculateIncidentStatus(incident);
                return ApiResponseDto<IncidentResponseDto>.SuccessResponse(incidentDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "インシデントの更新中にエラーが発生しました: {IncidentId}", id);
                return ApiResponseDto<IncidentResponseDto>.ErrorResponse("インシデントの更新に失敗しました");
            }
        }

        /// <summary>
        /// インシデントの削除
        /// </summary>
        public async Task<ApiResponseDto<bool>> DeleteIncidentAsync(int id)
        {
            try
            {
                var incident = await _context.Incidents
                    .FirstOrDefaultAsync(i => i.Id == id);

                if (incident == null)
                {
                    return ApiResponseDto<bool>.ErrorResponse("インシデントが見つかりません");
                }

                _context.Incidents.Remove(incident);
                await _context.SaveChangesAsync();

                return ApiResponseDto<bool>.SuccessResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "インシデントの削除中にエラーが発生しました: {IncidentId}", id);
                return ApiResponseDto<bool>.ErrorResponse("インシデントの削除に失敗しました");
            }
        }


        /// <summary>
        /// インシデントのCSV出力
        /// </summary>
        public async Task<ApiResponseDto<byte[]>> ExportIncidentsToCsvAsync(IncidentSearchDto searchDto)
        {
            try
            {
                var query = _context.Incidents.AsQueryable();

                // 検索条件の適用（GetIncidentsAsyncと同じロジック）
                if (!string.IsNullOrEmpty(searchDto.Search))
                {
                    var searchTerm = searchDto.Search.ToLower();
                    query = query.Where(i =>
                        i.Details.ToLower().Contains(searchTerm) ||
                        i.VoucherNumber != null && i.VoucherNumber.ToLower().Contains(searchTerm) ||
                        i.CustomerCode != null && i.CustomerCode.ToLower().Contains(searchTerm) ||
                        i.ProductCode != null && i.ProductCode.ToLower().Contains(searchTerm));
                }

                if (searchDto.Year.HasValue)
                {
                    query = query.Where(i => i.OccurrenceDateTime.Year == searchDto.Year.Value);
                }

                if (searchDto.Month.HasValue)
                {
                    query = query.Where(i => i.OccurrenceDateTime.Month == searchDto.Month.Value);
                }

                if (searchDto.Warehouse.HasValue)
                {
                    query = query.Where(i => i.ShippingWarehouse == searchDto.Warehouse.Value);
                }


                if (searchDto.TroubleCategory.HasValue)
                {
                    query = query.Where(i => i.TroubleCategory == searchDto.TroubleCategory.Value);
                }

                var incidents = await query
                    .OrderByDescending(i => i.OccurrenceDateTime)
                    .ToListAsync();

                var csvData = GenerateCsvData(incidents);
                return ApiResponseDto<byte[]>.SuccessResponse(csvData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CSV出力中にエラーが発生しました");
                return ApiResponseDto<byte[]>.ErrorResponse("CSV出力に失敗しました");
            }
        }

        /// <summary>
        /// ダッシュボード統計の取得
        /// </summary>
        public async Task<ApiResponseDto<DashboardStatsDto>> GetDashboardStatsAsync()
        {
            try
            {
                var stats = new DashboardStatsDto();

                // 基本統計
                stats.TotalIncidents = await _context.Incidents.CountAsync();

                // 日別発生件数（過去30日）- 発生日時（OccurrenceDateTime）を基準に集計
                var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
                stats.DailyIncidentCounts = await _context.Incidents
                    .Where(i => i.OccurrenceDateTime >= thirtyDaysAgo)
                    .GroupBy(i => i.OccurrenceDateTime.Date)
                    .Select(g => new DailyIncidentCountDto
                    {
                        Date = g.Key,
                        Count = g.Count()
                    })
                    .OrderBy(x => x.Date)
                    .ToListAsync();

                // 出荷元倉庫別件数
                stats.WarehouseIncidentCounts = await _context.Incidents
                    .GroupBy(i => i.ShippingWarehouse)
                    .Select(g => new WarehouseIncidentCountDto
                    {
                        Warehouse = g.Key.ToString(),
                        Count = g.Count()
                    })
                    .OrderByDescending(x => x.Count)
                    .ToListAsync();

                // トラブル区分別件数
                stats.TroubleCategoryCounts = await _context.Incidents
                    .GroupBy(i => i.TroubleCategory)
                    .Select(g => new TroubleCategoryCountDto
                    {
                        Category = g.Key.ToString(),
                        Count = g.Count()
                    })
                    .OrderByDescending(x => x.Count)
                    .ToListAsync();

                // トラブル詳細区分別件数
                stats.TroubleDetailCategoryCounts = await _context.Incidents
                    .GroupBy(i => i.TroubleDetailCategory)
                    .Select(g => new TroubleDetailCategoryCountDto
                    {
                        DetailCategory = g.Key.ToString(),
                        Count = g.Count()
                    })
                    .OrderByDescending(x => x.Count)
                    .ToListAsync();

                // 運送会社別件数
                stats.ShippingCompanyCounts = await _context.Incidents
                    .GroupBy(i => i.ShippingCompany)
                    .Select(g => new ShippingCompanyCountDto
                    {
                        Company = g.Key.ToString(),
                        Count = g.Count()
                    })
                    .OrderByDescending(x => x.Count)
                    .ToListAsync();


                return ApiResponseDto<DashboardStatsDto>.SuccessResponse(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ダッシュボード統計の取得中にエラーが発生しました");
                return ApiResponseDto<DashboardStatsDto>.ErrorResponse("ダッシュボード統計の取得に失敗しました");
            }
        }


        /// <summary>
        /// CSVデータの生成
        /// </summary>
        private byte[] GenerateCsvData(List<Incident> incidents)
        {
            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream, System.Text.Encoding.UTF8);

            // CSVヘッダー
            writer.WriteLine("ID,作成日,所属組織,作成者名,発生日時,発生場所,出荷元倉庫,運送会社名,トラブル区分,トラブル詳細区分,内容詳細,伝票番号,得意先コード,商品コード,数量,単位,2次情報入力日,発生経緯,発生原因,3次情報入力日,再発防止策,作成日時,更新日時");

            // CSVデータ
            foreach (var incident in incidents)
            {
                writer.WriteLine($"{incident.Id},{incident.CreationDate:yyyy-MM-dd},{incident.Organization},{incident.Creator},{incident.OccurrenceDateTime:yyyy-MM-dd HH:mm},{incident.OccurrenceLocation},{incident.ShippingWarehouse},{incident.ShippingCompany},{incident.TroubleCategory},{incident.TroubleDetailCategory},\"{incident.Details}\",{incident.VoucherNumber ?? ""},{incident.CustomerCode ?? ""},{incident.ProductCode ?? ""},{incident.Quantity ?? 0},{incident.Unit?.ToString() ?? ""},{incident.InputDate?.ToString("yyyy-MM-dd") ?? ""},\"{incident.ProcessDescription ?? ""}\",\"{incident.Cause ?? ""}\",{incident.InputDate3?.ToString("yyyy-MM-dd") ?? ""},\"{incident.RecurrencePreventionMeasures ?? ""}\",{incident.CreatedAt:yyyy-MM-dd HH:mm:ss},{incident.UpdatedAt:yyyy-MM-dd HH:mm:ss}");
            }

            writer.Flush();
            return memoryStream.ToArray();
        }
    }
}

