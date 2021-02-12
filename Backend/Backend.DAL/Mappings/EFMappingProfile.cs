using AutoMapper;
using Backend.DAL.Security;
using System.Collections.Generic;
using DTOs = Backend.Contract.DTOs;
using EF = Backend.DAL.Models;

namespace Backend.DAL.Mappings
{
  public class EFMappingProfile : Profile
  {
    public EFMappingProfile()
    {
      CreateMap<DTOs.Town, EF.Town>();

      CreateMap<DTOs.School, EF.School>()
        .ForMember(s => s.Town, opt => opt.Ignore()); //as they both have Town, but with different meaning (town name vs entity)

      CreateMap<DTOs.Student, EF.Student>()
       .ForMember(s => s.School, opt => opt.Ignore());  //as they both have School, but with different meaning

      CreateMap<DTOs.Workshop, EF.Workshop>()
      .ForMember(s => s.School, opt => opt.Ignore());

      CreateMap<DTOs.WorkshopParticipant, EF.WorkshopParticipant>()
      .ForMember(s => s.Workshop, opt => opt.Ignore())
      .ForMember(s => s.ParticipantId, conf => conf.MapFrom(dto => dto.StudentId));

      CreateMap<DTOs.User, EF.User>()
        .ForMember(e => e.UserId, conf => conf.MapFrom(dto => dto.Id))        
        .ForMember(e => e.Name, conf => conf.MapFrom(dto => dto.UserName))
        .ForMember(e => e.Password, conf =>
        {
          conf.PreCondition(dto => dto.ChangePassword);
          conf.ConvertUsing<PasswordConverter,  KeyValuePair<string, string>> (dto => KeyValuePair.Create(dto.UserName, dto.Password));
        });
    }
  }
}
