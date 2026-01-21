# Follow-Up Email Plan

## Goal
Provide users with a fast way to generate short, straightforward thank-you follow-up emails after interview or screening timeline events. The output should use timeline notes and any relevant contact emails, then open a new tab with a `mailto:` link containing the subject and body.

## User Experience Summary
- Each interview or screening timeline event includes a "Generate Follow-Up Email" button.
- Clicking the button opens a new browser tab to a `mailto:` URL with a prefilled subject and body.
- The email content is short, direct, and based on event notes plus any stored contact details.
- If multiple emails exist, the primary recruiter or interviewer email is used, with a fallback chooser.

## Email Content Rules
### Tone and Length
- 2 to 4 short sentences.
- Polite, grateful, and direct.
- Avoid buzzwords and avoid filler.

### Subject Line Options
- "Thank you for your time today"
- "Thank you for the interview"
- "Thank you for the screening"

### Body Template (Default)
"Hi {ContactFirstName},

Thank you for taking the time to speak with me about the {RoleTitle} role at {Company}. I appreciated the discussion about {KeyNoteOrTopic}. Please let me know if I can provide anything else.

Best,
{CandidateName}"

### Notes and Tokens
- {KeyNoteOrTopic} comes from the event notes timeline entry.
- If no notes exist, drop the sentence about the topic.
- If the contact name is missing, start with "Hi there,".
- If role or company is missing, remove those phrases rather than leaving blanks.

## Data Inputs
- Timeline event type (interview or screening)
- Timeline event notes (short summary)
- Contact emails from:
  - Event attendees (preferred)
  - Application contacts (fallback)
  - Company or recruiter email fields (last fallback)
- Candidate display name from profile
- Role title and company name

## Mailto Construction
### Required Behavior
- Build a `mailto:` URL with `subject` and `body` query parameters.
- URL-encode all content.
- Use the best available email address; allow user selection when multiple emails are present.
- Open the `mailto:` in a new tab/window to avoid losing app state.

### Example
`mailto:recruiter@company.com?subject=Thank%20you%20for%20the%20interview&body=Hi%20Alex%2C%0A%0AThank%20you%20for%20taking%20the%20time...`

## UI Placement
- Timeline event cards for interview and screening events.
- Button label: "Generate Follow-Up Email".
- Disabled state if no email address can be resolved; show tooltip with guidance.

## Edge Cases
- Multiple emails: open a lightweight chooser modal before launching `mailto:`.
- No notes: use the short template without the topic sentence.
- Very long notes: truncate to the first sentence or 120 characters.
- Missing candidate name: end with "Best," only.

## Manual Verification
- Button appears for interview and screening events only.
- Mailto opens in a new tab with correct subject and body.
- Content adapts when notes, contact name, or role/company are missing.
- Works on desktop and mobile.
- Respects `prefers-reduced-motion` if any button animation is added.

## Acceptance Criteria
- Users can generate a follow-up email from any interview or screening event.
- Mailto link includes subject and body prefilled and correctly encoded.
- Notes are incorporated when available without exceeding the short format.
- Contact email resolution follows the stated priority order.
