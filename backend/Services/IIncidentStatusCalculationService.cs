using LogisticsTroubleManagement.Models;

namespace LogisticsTroubleManagement.Services
{
    public interface IIncidentStatusCalculationService
    {
        string CalculateIncidentStatus(Incident incident);
        Task<string> CalculateIncidentStatusAsync(int incidentId);
        Task<Dictionary<int, string>> CalculateIncidentStatusesAsync(List<Incident> incidents);
        void ClearIncidentStatusCache(int incidentId);
        void ClearExpiredIncidentStatusCache(List<Incident> incidents);
    }
}
