using SharedLibrary.Responses;

namespace Job.Business.Services.Category
{
    public interface ICategoryService
    {
        Task<GetAllCategoriesResponse> GetAllCategroies(int skip = 1, int take = 6);
    }
}