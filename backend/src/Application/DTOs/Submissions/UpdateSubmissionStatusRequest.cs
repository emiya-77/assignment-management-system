using Domain.Enums;

namespace Application.DTOs.Submissions;

public class UpdateSubmissionStatusRequest
{
    public SubmissionStatus Status { get; set; }
}