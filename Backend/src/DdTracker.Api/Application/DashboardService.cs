using DdTracker.Api.Data;
using DdTracker.Api.Dtos;

namespace DdTracker.Api.Application;

public class DashboardService
{
    private readonly ContainerService _containerService;

    public DashboardService(ContainerService containerService)
        => _containerService = containerService;

    public async Task<DashboardSummaryDto> GetSummaryAsync()
    {
        var containers = await _containerService.GetAllAsync();

        var atRisk   = containers.Count(c => c.Risk == "AtRisk");
        var accruing = containers.Count(c => c.Risk == "Accruing");
        var safe     = containers.Count(c => c.Risk == "Safe");
        var closed   = containers.Count(c => c.Risk == "Closed");

        var totalAccrued = containers
            .Where(c => c.Risk == "Accruing")
            .Sum(c => c.CurrentCharge);

        return new DashboardSummaryDto(
            TotalContainers: containers.Count,
            Safe: safe,
            AtRisk: atRisk,
            Accruing: accruing,
            Closed: closed,
            TotalAccrued: totalAccrued,
            Currency: "USD");
    }
}