using Application.DTOs.Questions;
using Application.Interfaces;
using Application.Interfaces.Data;
using Domain.Entities;
using Domain.Enums;

namespace Infrastructure.Services;

/// <summary>
/// Service implementation for interview question operations.
/// </summary>
public class InterviewQuestionService : IInterviewQuestionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public InterviewQuestionService(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<InterviewQuestionDto> CreateAsync(CreateInterviewQuestionRequest request)
    {
        var userId = _currentUser.GetUserId()
            ?? throw new UnauthorizedAccessException("User not authenticated");

        // If an application ID is provided, verify it belongs to the user
        if (request.ApplicationId.HasValue)
        {
            var application = await _unitOfWork.Applications.GetByIdAsync(request.ApplicationId.Value);
            if (application == null || application.UserId != userId)
            {
                throw new KeyNotFoundException($"Application {request.ApplicationId} not found");
            }
        }

        var question = new InterviewQuestion
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            QuestionText = request.QuestionText.Trim(),
            AnswerText = request.AnswerText?.Trim(),
            Notes = request.Notes?.Trim(),
            Category = request.Category,
            Tags = request.Tags ?? new List<string>(),
            TimesAsked = 1,
            LastAskedDate = DateTime.UtcNow,
            CreatedDate = DateTime.UtcNow,
            ApplicationId = request.ApplicationId
        };

        await _unitOfWork.InterviewQuestions.AddAsync(question);
        await _unitOfWork.SaveChangesAsync();

        // Reload with application data if linked
        if (request.ApplicationId.HasValue)
        {
            question = await _unitOfWork.InterviewQuestions.GetByIdWithApplicationAsync(question.Id)
                ?? question;
        }

        return MapToDto(question);
    }

    public async Task<InterviewQuestionsListResponse> GetAllAsync(
        QuestionCategory? category = null,
        string? searchTerm = null,
        Guid? applicationId = null,
        int page = 1,
        int pageSize = 20,
        string sortBy = "createdDate",
        bool sortDescending = true)
    {
        var userId = _currentUser.GetUserId()
            ?? throw new UnauthorizedAccessException("User not authenticated");

        var (questions, totalCount) = await _unitOfWork.InterviewQuestions.GetByUserIdAsync(
            userId,
            category,
            searchTerm,
            applicationId,
            page,
            pageSize,
            sortBy,
            sortDescending);

        return new InterviewQuestionsListResponse
        {
            Questions = questions.Select(MapToDto).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<InterviewQuestionDto?> GetByIdAsync(Guid id)
    {
        var userId = _currentUser.GetUserId()
            ?? throw new UnauthorizedAccessException("User not authenticated");

        var question = await _unitOfWork.InterviewQuestions.GetByIdWithApplicationAsync(id);
        if (question == null || question.UserId != userId)
        {
            return null;
        }

        return MapToDto(question);
    }

    public async Task<InterviewQuestionDto?> UpdateAsync(Guid id, UpdateInterviewQuestionRequest request)
    {
        var userId = _currentUser.GetUserId()
            ?? throw new UnauthorizedAccessException("User not authenticated");

        var question = await _unitOfWork.InterviewQuestions.GetByIdAsync(id);
        if (question == null || question.UserId != userId)
        {
            return null;
        }

        // If an application ID is provided, verify it belongs to the user
        if (request.ApplicationId.HasValue)
        {
            var application = await _unitOfWork.Applications.GetByIdAsync(request.ApplicationId.Value);
            if (application == null || application.UserId != userId)
            {
                throw new KeyNotFoundException($"Application {request.ApplicationId} not found");
            }
        }

        question.QuestionText = request.QuestionText.Trim();
        question.AnswerText = request.AnswerText?.Trim();
        question.Notes = request.Notes?.Trim();
        question.Category = request.Category;
        question.Tags = request.Tags ?? new List<string>();
        question.ApplicationId = request.ApplicationId;
        question.UpdatedDate = DateTime.UtcNow;

        _unitOfWork.InterviewQuestions.Update(question);
        await _unitOfWork.SaveChangesAsync();

        // Reload with application data
        question = await _unitOfWork.InterviewQuestions.GetByIdWithApplicationAsync(id) ?? question;

        return MapToDto(question);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var userId = _currentUser.GetUserId()
            ?? throw new UnauthorizedAccessException("User not authenticated");

        var question = await _unitOfWork.InterviewQuestions.GetByIdAsync(id);
        if (question == null || question.UserId != userId)
        {
            return false;
        }

        _unitOfWork.InterviewQuestions.Delete(question);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<InterviewQuestionDto?> IncrementTimesAskedAsync(Guid id)
    {
        var userId = _currentUser.GetUserId()
            ?? throw new UnauthorizedAccessException("User not authenticated");

        var question = await _unitOfWork.InterviewQuestions.GetByIdAsync(id);
        if (question == null || question.UserId != userId)
        {
            return null;
        }

        question.TimesAsked++;
        question.LastAskedDate = DateTime.UtcNow;
        question.UpdatedDate = DateTime.UtcNow;

        _unitOfWork.InterviewQuestions.Update(question);
        await _unitOfWork.SaveChangesAsync();

        // Reload with application data
        question = await _unitOfWork.InterviewQuestions.GetByIdWithApplicationAsync(id) ?? question;

        return MapToDto(question);
    }

    public async Task<IEnumerable<InterviewQuestionDto>> GetFrequentAsync(int limit = 10)
    {
        var userId = _currentUser.GetUserId()
            ?? throw new UnauthorizedAccessException("User not authenticated");

        var questions = await _unitOfWork.InterviewQuestions.GetFrequentQuestionsAsync(userId, limit);
        return questions.Select(MapToDto);
    }

    private static InterviewQuestionDto MapToDto(InterviewQuestion entity)
    {
        return new InterviewQuestionDto
        {
            Id = entity.Id,
            QuestionText = entity.QuestionText,
            AnswerText = entity.AnswerText,
            Notes = entity.Notes,
            Category = entity.Category.ToString(),
            Tags = entity.Tags,
            TimesAsked = entity.TimesAsked,
            LastAskedDate = entity.LastAskedDate,
            CreatedDate = entity.CreatedDate,
            UpdatedDate = entity.UpdatedDate,
            ApplicationId = entity.ApplicationId,
            ApplicationCompany = entity.Application?.CompanyName,
            ApplicationRole = entity.Application?.RoleTitle
        };
    }
}
