using SharedLibrary.Dtos.CategoryDtos;

namespace SharedLibrary.Responses
{
    public class GetAllCategoriesResponse
    {
        public ICollection<CategoryDto> Categories { get; set; }
        public int TotalCount { get; set; }
    }
}