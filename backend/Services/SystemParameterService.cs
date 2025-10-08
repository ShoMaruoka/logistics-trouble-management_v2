using LogisticsTroubleManagement.Data;
using LogisticsTroubleManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace LogisticsTroubleManagement.Services
{
    public class SystemParameterService : ISystemParameterService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SystemParameterService> _logger;
        private readonly IMemoryCache _cache;

        public SystemParameterService(
            ApplicationDbContext context, 
            ILogger<SystemParameterService> logger,
            IMemoryCache cache)
        {
            _context = context;
            _logger = logger;
            _cache = cache;
        }

        public async Task<T?> GetParameterValueAsync<T>(string parameterKey, T? defaultValue = default)
        {
            var cacheKey = $"SystemParameter_{parameterKey}";
            
            if (_cache.TryGetValue(cacheKey, out T? cachedValue))
            {
                return cachedValue;
            }

            var parameter = await _context.SystemParameters
                .FirstOrDefaultAsync(p => p.ParameterKey == parameterKey && p.IsActive);

            if (parameter == null)
            {
                _logger.LogWarning("パラメータが見つかりません: {ParameterKey}", parameterKey);
                return defaultValue;
            }

            try
            {
                var value = Convert.ChangeType(parameter.ParameterValue, typeof(T));
                _cache.Set(cacheKey, value, TimeSpan.FromMinutes(30)); // 30分キャッシュ
                return (T)value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "パラメータの型変換に失敗しました: {ParameterKey}, Value: {Value}", 
                    parameterKey, parameter.ParameterValue);
                return defaultValue;
            }
        }

        public async Task<string> GetParameterValueAsync(string parameterKey, string defaultValue = "")
        {
            return await GetParameterValueAsync<string>(parameterKey, defaultValue) ?? defaultValue;
        }

        public async Task<int> GetIntParameterValueAsync(string parameterKey, int defaultValue = 0)
        {
            return await GetParameterValueAsync<int>(parameterKey, defaultValue);
        }

        public async Task<bool> GetBoolParameterValueAsync(string parameterKey, bool defaultValue = false)
        {
            return await GetParameterValueAsync<bool>(parameterKey, defaultValue);
        }

        public async Task<decimal> GetDecimalParameterValueAsync(string parameterKey, decimal defaultValue = 0)
        {
            return await GetParameterValueAsync<decimal>(parameterKey, defaultValue);
        }

        public async Task UpdateParameterValueAsync(string parameterKey, string value, int userId)
        {
            var parameter = await _context.SystemParameters
                .FirstOrDefaultAsync(p => p.ParameterKey == parameterKey);

            if (parameter == null)
            {
                throw new ArgumentException($"パラメータが見つかりません: {parameterKey}");
            }

            parameter.ParameterValue = value;
            parameter.UpdatedAt = DateTime.UtcNow;
            parameter.UpdatedBy = userId;

            await _context.SaveChangesAsync();

            // キャッシュをクリア
            _cache.Remove($"SystemParameter_{parameterKey}");
            
        }

        public async Task<List<SystemParameter>> GetAllParametersAsync()
        {
            return await _context.SystemParameters
                .Where(p => p.IsActive)
                .OrderBy(p => p.ParameterKey)
                .ToListAsync();
        }

        public async Task<SystemParameter?> GetParameterAsync(string parameterKey)
        {
            return await _context.SystemParameters
                .FirstOrDefaultAsync(p => p.ParameterKey == parameterKey && p.IsActive);
        }
    }
}
