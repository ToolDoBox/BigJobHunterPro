# Repository Guidelines

## Project Structure and Module Organization
- `index.html`, `styles.css`, and `script.js` implement the landing page.
- `imgs/` contains screenshots and brand assets referenced by the HTML.
- `Meta/Docs/` holds strategy, scope, and architecture planning notes; `Meta/Ref/` stores reference artifacts; `Meta/Sprints/` contains sprint planning, backlog, and templates. Keep these updated when product decisions change.
- If you need a new Markdown file that does not fit an existing category, place it in `Meta/` (misc bucket, but do not create junk files).

## Build, Test, and Development Commands
- Local preview: open `index.html` directly, or run `python -m http.server 8000` and visit `http://localhost:8000`.
- Build and test: no build pipeline or automated test runner exists in this repo yet. If you add tooling, document the exact commands here.

## Coding Style and Naming Conventions
- Indentation is 4 spaces in HTML, CSS, and JS files.
- CSS uses `:root` variables and kebab-case class names (example: `.feature-card`); keep new styles grouped by section with banner comments.
- JavaScript uses camelCase for functions and variables (example: `showComingSoon`); prefer small helpers and event listeners over inline scripts.
- Keep assets in `imgs/` and update alt text in `index.html` when you add or replace images.

## Testing Guidelines
- No automated tests yet. Manually verify: modal open and close, counter animation on scroll, and key interactions after changes.
- Check responsive behavior at <=768px and honor `prefers-reduced-motion` for animation-heavy edits.

## Commit and Pull Request Guidelines
- Git history uses short Title Case summaries without prefixes (example: "Sprint planning"); keep commits concise and focused.
- PRs should include a brief description, screenshots for UI changes, and manual test notes. Link any relevant updates in `Meta/Docs/` or `Meta/Sprints/`.

## Architecture Notes
- Planned full-stack architecture and future commands are described in `Meta/Docs/Project-Structure.md` and `Meta/Docs/deprecated_GETTING_STARTED.md`. Use them for direction, but do not add build instructions until the code exists.
