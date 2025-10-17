using LogisticsTroubleManagement.Data;
using LogisticsTroubleManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace LogisticsTroubleManagement.Services
{
    public class IncidentStatusCalculationService : IIncidentStatusCalculationService
    {
        private readonly ISystemParameterService _parameterService;
        private readonly ILogger<IncidentStatusCalculationService> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;

        public IncidentStatusCalculationService(
            ISystemParameterService parameterService,
            ILogger<IncidentStatusCalculationService> logger,
            ApplicationDbContext context,
            IMemoryCache cache)
        {
            _parameterService = parameterService;
            _logger = logger;
            _context = context;
            _cache = cache;
        }

        public async Task<string> CalculateIncidentStatusAsync(Incident incident)
        {
            var now = DateTime.UtcNow;
            
            // 3次情報が完了している場合は、期限に関係なく「完了」ステータス
            if (IsThirdInfoCompleted(incident))
            {
                return IncidentStatus.Completed;
            }

            // 2次情報が完了している場合
            if (IsSecondInfoCompleted(incident))
            {
                // 3次情報の期限チェック
                if (await IsThirdInfoExpired(incident, now))
                {
                    return IncidentStatus.ThirdInfoDelayed;
                }
                return IncidentStatus.ThirdInfoInvestigation;
            }

            // 2次情報の期限チェック
            if (await IsSecondInfoExpired(incident, now))
            {
                return IncidentStatus.SecondInfoDelayed;
            }

            // デフォルトは2次情報調査中
            return IncidentStatus.SecondInfoInvestigation;
        }

        public string CalculateIncidentStatus(Incident incident)
        {
            var now = DateTime.UtcNow;
            
            // 3次情報が完了している場合は、期限に関係なく「完了」ステータス
            if (IsThirdInfoCompleted(incident))
            {
                return IncidentStatus.Completed;
            }

            // 2次情報が完了している場合
            if (IsSecondInfoCompleted(incident))
            {
                // 3次情報の期限チェック（同期版）
                if (IsThirdInfoExpiredSync(incident, now))
                {
                    return IncidentStatus.ThirdInfoDelayed;
                }
                return IncidentStatus.ThirdInfoInvestigation;
            }

            // 2次情報の期限チェック（同期版）
            if (IsSecondInfoExpiredSync(incident, now))
            {
                return IncidentStatus.SecondInfoDelayed;
            }

            // デフォルトは2次情報調査中
            return IncidentStatus.SecondInfoInvestigation;
        }

        public async Task<string> CalculateIncidentStatusAsync(int incidentId)
        {
            var incident = await _context.Incidents
                .FirstOrDefaultAsync(i => i.Id == incidentId);

            if (incident == null)
            {
                throw new ArgumentException($"インシデントが見つかりません: {incidentId}");
            }

            return CalculateIncidentStatus(incident);
        }

        public Task<Dictionary<int, string>> CalculateIncidentStatusesAsync(List<Incident> incidents)
        {
            var statuses = new Dictionary<int, string>();
            var uncachedIncidents = new List<Incident>();
            var now = DateTime.UtcNow;

            // キャッシュから取得（期限チェックが必要な場合はキャッシュを無視）
            foreach (var incident in incidents)
            {
                var cacheKey = $"incident_status_{incident.Id}";
                var shouldUseCache = true;
                
                // 2次情報が未完了で、期限が過ぎている可能性がある場合はキャッシュを無視
                if (!IsSecondInfoCompleted(incident))
                {
                    var deadline = incident.CreationDate.AddDays(7);
                    if (now > deadline)
                    {
                        shouldUseCache = false;
                    }
                }
                // 2次情報完了済みで3次情報が未完了の場合も期限チェック
                else if (!IsThirdInfoCompleted(incident) && incident.InputDate.HasValue)
                {
                    var deadline = incident.InputDate.Value.AddDays(7);
                    if (now > deadline)
                    {
                        shouldUseCache = false;
                    }
                }

                if (shouldUseCache && _cache.TryGetValue(cacheKey, out string? cachedStatus) && cachedStatus != null)
                {
                    statuses[incident.Id] = cachedStatus;
                }
                else
                {
                    uncachedIncidents.Add(incident);
                }
            }

            // キャッシュされていないインシデントのステータスを計算
            foreach (var incident in uncachedIncidents)
            {
                var status = CalculateIncidentStatus(incident);
                statuses[incident.Id] = status;
                
                // キャッシュに保存（期限が近い場合は短い有効期限を設定）
                var cacheKey = $"incident_status_{incident.Id}";
                var cacheExpiry = TimeSpan.FromMinutes(5);
                
                // 期限が近い場合は短いキャッシュ時間を設定
                if (!IsSecondInfoCompleted(incident))
                {
                    var deadline = incident.CreationDate.AddDays(7);
                    var timeToDeadline = deadline - now;
                    if (timeToDeadline.TotalMinutes < 60) // 1時間以内の場合は1分キャッシュ
                    {
                        cacheExpiry = TimeSpan.FromMinutes(1);
                    }
                }
                
                _cache.Set(cacheKey, status, cacheExpiry);
            }

            return Task.FromResult(statuses);
        }

        /// <summary>
        /// インシデントのステータスキャッシュをクリア
        /// </summary>
        public void ClearIncidentStatusCache(int incidentId)
        {
            var cacheKey = $"incident_status_{incidentId}";
            _cache.Remove(cacheKey);
        }

        /// <summary>
        /// 期限切れの可能性があるインシデントのキャッシュをクリア
        /// </summary>
        public void ClearExpiredIncidentStatusCache(List<Incident> incidents)
        {
            var now = DateTime.UtcNow;
            foreach (var incident in incidents)
            {
                var shouldClearCache = false;
                
                // 2次情報が未完了で期限が過ぎている場合
                if (!IsSecondInfoCompleted(incident))
                {
                    var deadline = incident.CreationDate.AddDays(7);
                    if (now > deadline)
                    {
                        shouldClearCache = true;
                    }
                }
                // 2次情報完了済みで3次情報が未完了で期限が過ぎている場合
                else if (!IsThirdInfoCompleted(incident) && incident.InputDate.HasValue)
                {
                    var deadline = incident.InputDate.Value.AddDays(7);
                    if (now > deadline)
                    {
                        shouldClearCache = true;
                    }
                }

                if (shouldClearCache)
                {
                    ClearIncidentStatusCache(incident.Id);
                }
            }
        }

        private bool IsSecondInfoCompleted(Incident incident)
        {
            return incident.InputDate.HasValue &&
                   !string.IsNullOrEmpty(incident.ProcessDescription) &&
                   !string.IsNullOrEmpty(incident.Cause);
        }

        private bool IsThirdInfoCompleted(Incident incident)
        {
            return IsSecondInfoCompleted(incident) &&
                   incident.InputDate3.HasValue &&
                   !string.IsNullOrEmpty(incident.RecurrencePreventionMeasures);
        }

        private async Task<bool> IsSecondInfoExpired(Incident incident, DateTime currentTime)
        {
            var deadlineDays = await _parameterService.GetIntParameterValueAsync(
                SystemParameterKeys.SecondInfoDeadlineDays, 7);
            
            var deadline = incident.CreationDate.AddDays(deadlineDays);
            return currentTime > deadline;
        }

        private async Task<bool> IsThirdInfoExpired(Incident incident, DateTime currentTime)
        {
            if (!incident.InputDate.HasValue) return false;

            var deadlineDays = await _parameterService.GetIntParameterValueAsync(
                SystemParameterKeys.ThirdInfoDeadlineDays, 7);
            
            var deadline = incident.InputDate.Value.AddDays(deadlineDays);
            return currentTime > deadline;
        }

        private bool IsSecondInfoExpiredSync(Incident incident, DateTime currentTime)
        {
            // デフォルト値7日を使用（同期版）
            var deadlineDays = 7;
            var deadline = incident.CreationDate.AddDays(deadlineDays);
            return currentTime > deadline;
        }

        private bool IsThirdInfoExpiredSync(Incident incident, DateTime currentTime)
        {
            if (!incident.InputDate.HasValue) return false;

            // デフォルト値7日を使用（同期版）
            var deadlineDays = 7;
            var deadline = incident.InputDate.Value.AddDays(deadlineDays);
            return currentTime > deadline;
        }
    }
}
