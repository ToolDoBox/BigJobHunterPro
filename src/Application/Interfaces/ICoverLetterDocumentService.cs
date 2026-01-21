using Application.DTOs.CoverLetter;

namespace Application.Interfaces;

public interface ICoverLetterDocumentService
{
    Task<CombinedCoverLetterPdfResult> GetCombinedPdfAsync(Guid applicationId);
}
