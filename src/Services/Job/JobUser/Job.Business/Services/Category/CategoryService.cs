using MassTransit;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace Job.Business.Services.Category
{
    public class CategoryService(IRequestClient<GetAllCategoriesRequest> requestClient) : ICategoryService
    {
        private readonly IRequestClient<GetAllCategoriesRequest> _requestClient = requestClient;
        public async Task<GetAllCategoriesResponse> GetAllCategroies(int skip = 1, int take = 6)
        {
            var response = await _requestClient.GetResponse<GetAllCategoriesResponse>(new GetAllCompaniesRequest
            {
                Skip = skip,
                Take = take,
            });

            return new GetAllCategoriesResponse
            {
                Categories = response.Message.Categories,
                TotalCount = response.Message.TotalCount,
            };
        }
    }
}