using Application.DTOs.Applications;

namespace Application.Interfaces;

public interface IApplicationService
{
    Task<CreateApplicationResponse> CreateApplicationAsync(CreateApplicationRequest request);
}
