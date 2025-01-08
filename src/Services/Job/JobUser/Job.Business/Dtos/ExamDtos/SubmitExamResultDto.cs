namespace Job.Business.Dtos.ExamDtos;

public class SubmitExamResultDto
{
    public byte TrueAnswerCount { get; set; } // Number of correct answers submitted by the user
    public byte FalseAnswerCount { get; set; } // Number of incorrect answers submitted by the user
    public bool IsPassed { get; set; } // Indicates whether the user passed the exam
}