using AutoMapper;
using SchoolSystem.Application.Models.DTOs;
using SchoolSystem.Application.Models.Request;
using SchoolSystem.Core.Entites;

namespace SchoolSystem.Application.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Ismail_Gender, GenderModelDto>().ReverseMap();
            CreateMap<GenderModel, Ismail_Gender>()
                .ForMember(dest => dest.GenderID, opt => opt.Ignore());

            CreateMap<Ismail_Types, TypeModelDto>().ReverseMap();
            CreateMap<TypeModel, Ismail_Types>()
                .ForMember(dest => dest.TypeID, opt => opt.Ignore());

            CreateMap<Ismail_Clients, ClientModelDto>().ReverseMap();
            CreateMap<ClientModelDto, Ismail_Clients>()
                .ForMember(dest => dest.UserID, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Ismail_Classes, ClassModelDto>().ReverseMap();
            CreateMap<ClassModel, Ismail_Classes>()
                .ForMember(dest => dest.ClassID, opt => opt.Ignore());

            CreateMap<Ismail_ErrorLogs, ErrorLogsModelDto>().ReverseMap();
            CreateMap<ErrorLogsModel, Ismail_ErrorLogs>();

            CreateMap<Ismail_StudentMarks, StudentMarkModelDto>().ReverseMap();
            CreateMap<StudentMarkModel, Ismail_StudentMarks>();
        }

    }
}