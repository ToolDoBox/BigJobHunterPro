using Application.DTOs.CoverLetter;
using Application.Interfaces;
using Application.Interfaces.Data;
using Microsoft.Playwright;
using Microsoft.Extensions.Logging;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;

namespace Infrastructure.Services;

public class CoverLetterDocumentService : ICoverLetterDocumentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<CoverLetterDocumentService> _logger;

    public CoverLetterDocumentService(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        ILogger<CoverLetterDocumentService> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<CombinedCoverLetterPdfResult> GetCombinedPdfAsync(Guid applicationId)
    {
        var userId = _currentUser.GetUserId()
            ?? throw new UnauthorizedAccessException("User not authenticated");

        var application = await _unitOfWork.Applications.GetByIdAsync(applicationId);
        if (application == null || application.UserId != userId)
        {
            return CombinedCoverLetterPdfResult.Failed("Application not found");
        }

        if (string.IsNullOrWhiteSpace(application.CoverLetterHtml))
        {
            return CombinedCoverLetterPdfResult.Failed("Cover letter not found. Generate or save one first.");
        }

        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null || string.IsNullOrWhiteSpace(user.ResumeHtml))
        {
            return CombinedCoverLetterPdfResult.Failed("No resume HTML saved. Add resume HTML in your profile.");
        }

        try
        {
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true
            });

            var coverLetterPdf = await RenderPdfAsync(browser, application.CoverLetterHtml);
            var resumePdf = await RenderPdfAsync(browser, user.ResumeHtml);
            var mergedPdf = MergePdfs(coverLetterPdf, resumePdf);
            var fileName = BuildFileName(application.CompanyName, application.RoleTitle);

            return CombinedCoverLetterPdfResult.Succeeded(mergedPdf, fileName);
        }
        catch (PlaywrightException ex) when (IsMissingBrowserException(ex))
        {
            _logger.LogError(ex, "Playwright browsers not installed for combined cover letter PDF {ApplicationId}", applicationId);
            return CombinedCoverLetterPdfResult.Failed(
                "Playwright browsers are not installed. Run the Playwright install script and try again.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating combined cover letter PDF for application {ApplicationId}", applicationId);
            return CombinedCoverLetterPdfResult.Failed("Failed to generate combined PDF");
        }
    }

    private static async Task<byte[]> RenderPdfAsync(IBrowser browser, string html)
    {
        var normalizedHtml = EnsureHtmlDocument(html);

        var page = await browser.NewPageAsync();
        await page.SetContentAsync(normalizedHtml, new PageSetContentOptions
        {
            WaitUntil = WaitUntilState.Load
        });

        var pdfBytes = await page.PdfAsync(new PagePdfOptions
        {
            Format = "Letter",
            PrintBackground = true,
            PreferCSSPageSize = true
        });

        await page.CloseAsync();
        return pdfBytes;
    }

    private static byte[] MergePdfs(byte[] coverLetterPdf, byte[] resumePdf)
    {
        using var output = new PdfDocument();
        using var coverLetterDoc = PdfReader.Open(new MemoryStream(coverLetterPdf), PdfDocumentOpenMode.Import);
        using var resumeDoc = PdfReader.Open(new MemoryStream(resumePdf), PdfDocumentOpenMode.Import);

        CopyPages(coverLetterDoc, output);
        CopyPages(resumeDoc, output);

        using var outputStream = new MemoryStream();
        output.Save(outputStream, false);
        return outputStream.ToArray();
    }

    private static void CopyPages(PdfDocument source, PdfDocument destination)
    {
        for (var i = 0; i < source.PageCount; i++)
        {
            destination.AddPage(source.Pages[i]);
        }
    }

    private static string EnsureHtmlDocument(string html)
    {
        if (html.Contains("<html", StringComparison.OrdinalIgnoreCase))
        {
            return html;
        }

        return $"""
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset="utf-8" />
            </head>
            <body>
            {html}
            </body>
            </html>
            """;
    }

    private static bool IsMissingBrowserException(PlaywrightException ex)
    {
        var message = ex.Message ?? string.Empty;
        return message.Contains("playwright install", StringComparison.OrdinalIgnoreCase)
            || message.Contains("executable doesn't exist", StringComparison.OrdinalIgnoreCase);
    }

    private static string BuildFileName(string? companyName, string? roleTitle)
    {
        var companySegment = SanitizeFileSegment(companyName, "company");
        var roleSegment = SanitizeFileSegment(roleTitle, "role");
        return $"{companySegment}-{roleSegment}-cover-letter-resume.pdf";
    }

    private static string SanitizeFileSegment(string? value, string fallback)
    {
        var cleaned = string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();

        foreach (var invalidChar in Path.GetInvalidFileNameChars())
        {
            cleaned = cleaned.Replace(invalidChar, '-');
        }

        cleaned = cleaned.Replace(' ', '-');
        return string.IsNullOrWhiteSpace(cleaned) ? fallback : cleaned;
    }
}
