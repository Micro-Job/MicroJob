namespace SharedLibrary.Responses;

public class GetExamQuestionMappingResponse
{
    public Dictionary<Guid, Guid> ExamQuestionMapping { get; set; }
}
