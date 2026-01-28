namespace Domain.Enums;

/// <summary>
/// Categories for interview questions to help organize and filter practice material.
/// </summary>
public enum QuestionCategory
{
    /// <summary>
    /// "Tell me about a time..." questions about past experiences and behaviors.
    /// </summary>
    Behavioral = 0,

    /// <summary>
    /// Skills, tools, coding, and domain-specific technical questions.
    /// </summary>
    Technical = 1,

    /// <summary>
    /// "What would you do if..." hypothetical scenario questions.
    /// </summary>
    Situational = 2,

    /// <summary>
    /// Questions about the specific company, their products, or the role.
    /// </summary>
    CompanySpecific = 3,

    /// <summary>
    /// General questions like salary expectations, availability, work preferences.
    /// </summary>
    General = 4,

    /// <summary>
    /// Questions that don't fit other categories.
    /// </summary>
    Other = 5
}
