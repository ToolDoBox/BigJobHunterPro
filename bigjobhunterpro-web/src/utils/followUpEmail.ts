export interface FollowUpEmailData {
  recipientEmail: string;
  recipientFirstName: string;
  senderName: string;
  companyName: string;
  roleTitle: string;
  topicDiscussed: string;
}

/**
 * Extracts the first name from a full name string
 */
export function extractFirstName(fullName: string): string {
  const parts = fullName.trim().split(/\s+/);
  return parts[0] || fullName;
}

/**
 * Extracts a topic suggestion from event notes
 * Takes the first sentence or first 100 characters
 */
export function extractTopicFromNotes(notes: string | null): string {
  if (!notes || !notes.trim()) return '';

  const trimmed = notes.trim();

  // Try to find the first sentence (ending with . ! or ?)
  const sentenceMatch = trimmed.match(/^[^.!?]+[.!?]/);
  if (sentenceMatch) {
    return sentenceMatch[0].trim();
  }

  // Otherwise, take the first 100 characters and add ellipsis if truncated
  if (trimmed.length <= 100) {
    return trimmed;
  }

  // Find a good break point (space) before 100 characters
  const breakPoint = trimmed.lastIndexOf(' ', 100);
  if (breakPoint > 50) {
    return trimmed.substring(0, breakPoint) + '...';
  }

  return trimmed.substring(0, 100) + '...';
}

/**
 * Generates a follow-up email from the template
 */
export function generateFollowUpEmail(data: FollowUpEmailData): string {
  const { recipientFirstName, senderName, companyName, roleTitle, topicDiscussed } = data;

  const topicPart = topicDiscussed.trim()
    ? `I appreciated the discussion about ${topicDiscussed}. `
    : 'I appreciated learning more about the role and your team. ';

  return `Hi ${recipientFirstName},

Thank you for taking the time to speak with me about the ${roleTitle} role at ${companyName}. ${topicPart}Please let me know if I can provide anything else.

${senderName}`;
}

/**
 * Builds a mailto URL with the email content
 * Note: Uses encodeURIComponent instead of URLSearchParams because
 * mailto URLs require %20 for spaces (RFC 6068), not + which URLSearchParams uses
 */
export function buildMailtoUrl(
  recipientEmail: string,
  subject: string,
  body: string
): string {
  const encodedSubject = encodeURIComponent(subject);
  const encodedBody = encodeURIComponent(body);

  return `mailto:${encodeURIComponent(recipientEmail)}?subject=${encodedSubject}&body=${encodedBody}`;
}

/**
 * Opens a mailto link in a new tab/window
 */
export function openMailtoLink(url: string): void {
  window.open(url, '_blank', 'noopener,noreferrer');
}
