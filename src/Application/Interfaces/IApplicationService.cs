using Application.DTOs.Applications;

namespace Application.Interfaces;

public interface IApplicationService
{
    Task<CreateApplicationResponse> CreateApplicationAsync(CreateApplicationRequest request);
    Task<ApplicationsListResponse> GetApplicationsAsync(int page, int pageSize);
    Task<ApplicationDto?> GetApplicationAsync(Guid id);
    Task<ApplicationDto?> UpdateApplicationAsync(Guid id, UpdateApplicationRequest request);
    Task<ApplicationDto?> UpdateApplicationStatusAsync(Guid id, UpdateStatusRequest request);
    Task<bool> DeleteApplicationAsync(Guid id);
}
