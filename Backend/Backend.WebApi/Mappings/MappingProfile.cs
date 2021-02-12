using AutoMapper;
using Backend.Core.Queries.Common;
using Backend.WebApi.Models;
using System.Linq;

namespace Backend.WebApi.Mappings
{
  public class MappingProfile : Profile
  {
    public MappingProfile()
    {
      CreateMap<LoadParams, PagingData>()
        .ForMember(pd => pd.Count, conf => conf.MapFrom(lp => lp.Rows))
        .ForMember(pd => pd.From, conf => conf.MapFrom(lp => lp.First));      
    }

    /*
     *  data.From = loadParams.First;
      data.Count = loadParams.Rows;
      data.Sort = loadParams.Sort;
      data.Ascending = loadParams.Ascending;*/
  }
}
