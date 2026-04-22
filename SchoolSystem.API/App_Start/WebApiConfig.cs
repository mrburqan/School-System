using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using SchoolSystem.API.Filters;
using SchoolSystem.Application.Mapper;
using SchoolSystem.Application.Services;
using SchoolSystem.Application.Services.IServices;
using SchoolSystem.Core.IRepositories;
using SchoolSystem.Core.IRepositories.ICustomRepository;
using SchoolSystem.Infrastructure.Repositories;
using SchoolSystem.Infrastructure.Repositories.CustomRepository;
using System;
using System.Web.Http;
using Unity;
using Unity.WebApi;

namespace SchoolSystem.API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            config.Filters.Add(new GlobalExceptionFilter());
            // Enable attribute routing
            config.MapHttpAttributeRoutes();

            // Default API route
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            var container = new UnityContainer();


            var configMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            }, NullLoggerFactory.Instance);

            IMapper mapper = configMapper.CreateMapper();
            container.RegisterInstance<IMapper>(mapper);

            container
                .RegisterType(typeof(IGenericRepository<>), typeof(GenericRepository<>))
                .RegisterType(typeof(IAssociationRepository), typeof(AssociationRepository))
                .RegisterType<IStudentMarkService, StudentMarkService>()
                .RegisterType<ITypeService, TypeService>()
                .RegisterType<IGenderService, GenderService>()
                .RegisterType<IClassService, ClassService>()
                .RegisterType<IClientService, ClientService>()
                .RegisterType<IAssociationService, AssociationService>()
                .RegisterType<IErrorLogsService, ErrorLogsService>()
                .RegisterType<IUtilitiesService, UtilitiesService>();

            config.DependencyResolver = new UnityDependencyResolver(container);


        }
    }
}
