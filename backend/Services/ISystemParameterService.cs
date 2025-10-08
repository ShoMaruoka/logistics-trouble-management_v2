using LogisticsTroubleManagement.Models;

namespace LogisticsTroubleManagement.Services
{
    public interface ISystemParameterService
    {
        Task<T?> GetParameterValueAsync<T>(string parameterKey, T? defaultValue = default);
        Task<string> GetParameterValueAsync(string parameterKey, string defaultValue = "");
        Task<int> GetIntParameterValueAsync(string parameterKey, int defaultValue = 0);
        Task<bool> GetBoolParameterValueAsync(string parameterKey, bool defaultValue = false);
        Task<decimal> GetDecimalParameterValueAsync(string parameterKey, decimal defaultValue = 0);
        Task UpdateParameterValueAsync(string parameterKey, string value, int userId);
        Task<List<SystemParameter>> GetAllParametersAsync();
        Task<SystemParameter?> GetParameterAsync(string parameterKey);
    }
}
