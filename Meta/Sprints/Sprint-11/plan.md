# Cover Letter + Resume Combined PDF Plan

## Idea Review

### Strengths
- Keeps the cover letter as page 1 while bundling the resume for one-click submissions
- Separates AI inputs from the final resume layout (resume HTML never goes to the AI API)
- Improves consistency for "quick apply" flows that only allow one upload

### Risks and Tradeoffs
- Storing HTML introduces XSS risk if it is rendered without sandboxing or sanitization
- HTML-to-PDF conversion adds runtime dependencies and can be brittle across environments
- CSS conflicts between cover letter HTML and resume HTML can break layout
- Large HTML payloads can bloat storage and slow PDF generation

### Recommendation
- Keep two distinct profile fields: resumeText (AI input) and resumeHtml (PDF output)
- Sanitize resumeHtml with a strict allowlist (no scripts, no external JS, limited attributes)
- Render cover letter and resume to PDF separately, then merge into one PDF to avoid CSS collisions

## Proposed User Flow
1. User saves resumeText (plain text) for AI cover letter generation.
2. User saves resumeHtml (exported HTML from their resume editor).
3. User generates a cover letter as today.
4. User clicks "Download Combined PDF" to receive a single file: cover letter first, resume appended.

## Implementation Plan

### Data Model and API
- Add new fields to `ApplicationUser`:
  - ResumeHtml (string?, with size limit)
  - ResumeHtmlUpdatedAt (DateTime?)
- Add migration to persist new columns.
- Update profile DTOs to include resumeHtml, resumeHtmlUpdatedAt, and htmlCharacterCount.
- Add endpoints:
  - `PUT /api/profile/resume-html` to save sanitized HTML
  - `DELETE /api/profile/resume-html` to clear it
- Add server-side validation for max length and basic HTML preflight checks.

### HTML Sanitization
- Add an HTML sanitizer service to strip scripts, event handlers, and disallowed tags.
- Allow style tags and inline styles needed for print layout.
- Block remote resources or require data URIs for images.

### PDF Generation and Merge
- Add a document rendering service (ex: Playwright or PuppeteerSharp) for HTML-to-PDF.
- Render cover letter HTML and resume HTML separately with the same page size (US Letter).
- Merge PDFs using a .NET PDF library (ex: PdfSharpCore) into a single file.
- Create API endpoint:
  - `GET /api/applications/{id}/cover-letter/combined-pdf`
  - Returns `application/pdf` with filename `{Company}-{Role}-cover-letter+resume.pdf`

### Frontend Updates
- Profile page:
  - Add a "Resume HTML" panel with paste area and optional "Upload HTML file" button.
  - Provide a sandboxed preview iframe (no script execution).
  - Show size counter and last updated timestamp.
- Cover Letter section:
  - Add "Download Combined PDF" button.
  - Handle missing resumeHtml with a clear error message and link to profile.

### Manual Verification
- Save resumeHtml, preview it, and ensure scripts are not executed.
- Generate cover letter, download combined PDF, confirm ordering and formatting.
- Verify errors for missing resumeHtml or missing cover letter.
- Check mobile layout and respects existing UI patterns.

## Acceptance Criteria
- Users can store resumeHtml separately from resumeText.
- Combined PDF downloads with cover letter as first page and resume appended.
- HTML is sanitized and previewed safely.
- Clear error messaging for missing inputs.

## Open Questions
- Preferred HTML-to-PDF engine (Playwright vs. PuppeteerSharp vs. serverless approach)?
- Maximum allowed size for resumeHtml?
- Should we allow base64 images, or require text-only resumes?
