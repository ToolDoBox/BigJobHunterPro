namespace Application.DTOs.CoverLetter;

public class CombinedCoverLetterPdfResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public byte[]? PdfBytes { get; set; }
    public string? FileName { get; set; }

    public static CombinedCoverLetterPdfResult Succeeded(byte[] pdfBytes, string fileName)
    {
        return new CombinedCoverLetterPdfResult
        {
            Success = true,
            PdfBytes = pdfBytes,
            FileName = fileName
        };
    }

    public static CombinedCoverLetterPdfResult Failed(string errorMessage)
    {
        return new CombinedCoverLetterPdfResult
        {
            Success = false,
            ErrorMessage = errorMessage
        };
    }
}
