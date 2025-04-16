namespace AuthService.Business.Dtos;

public class DataListDto<T> where T : class
{
    public List<T> Datas { get; set; }
    //public int? TotalPage { get; set; }
    public int? TotalCount { get; set; }
}
