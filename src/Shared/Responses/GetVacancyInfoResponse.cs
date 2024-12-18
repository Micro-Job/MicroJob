using SharedLibrary.Dtos.CategoryDtos;
using SharedLibrary.Enums;

namespace SharedLibrary.Responses
{
    public class GetVacancyInfoResponse
    {
        public string Requirement { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public CategoryDto? Category { get; set; }
        public WorkType? WorkType { get; set; }
        public string? Location { get; set; }
        public int ViewCount { get; set; }
        public FamilySituation Family { get; set; }  //temporary fix
        public Gender Gender { get; set; }           //temporary fix
    }
}