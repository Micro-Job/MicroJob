namespace JobCompany.Business.Dtos.CategoryDtos
{
    public record CategoryListDto
    {
        public Guid Id { get; set; }
        public string CategoryName { get; set; }
    }
}