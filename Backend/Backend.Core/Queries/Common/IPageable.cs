namespace Backend.Core.Queries.Common
{
  public interface IPageable
  {
    PagingData Paging { get; set; }   
  }
}
