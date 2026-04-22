using AutoMapper;
using SchoolSystem.Application.Models.DTOs;
using SchoolSystem.Application.Models.Request;
using SchoolSystem.Application.Models.Response;
using SchoolSystem.Application.Services.IServices;
using SchoolSystem.Core.Entites;
using SchoolSystem.Core.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SchoolSystem.Application.Services
{
    public class ClassService : IClassService
    {
        private readonly IGenericRepository<Ismail_Classes> _classRepository;
        private readonly IMapper _mapper;
        private readonly IUtilitiesService _utilities;

        public ClassService(IGenericRepository<Ismail_Classes> classRepository, IMapper mapper, IUtilitiesService utilities)
        {
            this._classRepository = classRepository;
            _mapper = mapper;
            _utilities = utilities;
        }

        public async Task<ResponseData<ClassModelDto>> AddAsync(ClassModel model)
        {
            if (model == null)
                return ResponseData<ClassModelDto>.Failure("Model cannot be  null");

            var existed = await _classRepository.AnyAsync(x => x.ClassName.ToLower().Equals(model.ClassName.ToLower()));
            if (existed)
            {
                return ResponseData<ClassModelDto>.Failure("This class already exists");
            }
            model.ClassName = await _utilities.FirstCharecterToApperCase(model.ClassName);


            var newModel = _mapper.Map<Ismail_Classes>(model);
            var result = await _classRepository.AddAsync(newModel);
            if (!result)
            {
                return ResponseData<ClassModelDto>.Failure("Add operation is failed");
            }
            var resultToDto = _mapper.Map<ClassModelDto>(newModel);
            return ResponseData<ClassModelDto>.Success(resultToDto, "Added successfully");
        }

        public async Task<ResponseData<bool>> DeleteFirstOrDefaultAsync(int classId)
        {
            var existsCalss = await _classRepository.GetFirstOrDefaultAsync(x => x.ClassID == classId);
            if (existsCalss == null)
                return ResponseData<bool>.Failure("Class Not found");

            var result = await _classRepository.DeleteFirstOrDefault(x => x.ClassID == classId);
            if (!result)
            {
                return ResponseData<bool>.Failure("Failed to delete class");
            }
            return ResponseData<bool>.Success(true, "Class Deleted successfully");
        }

        public async Task<ResponseData<ClassModelDto>> UpdateAsync(ClassModel model)
        {
            if (model == null)
                return ResponseData<ClassModelDto>.Failure("Model cannot be null");
            var entityToUpdate = await _classRepository.GetFirstOrDefaultAsync(x => x.ClassID == model.ClassID);
            if (entityToUpdate == null)
            {
                return ResponseData<ClassModelDto>.Failure("Class not found");
            }
            entityToUpdate.ClassName = await _utilities.FirstCharecterToApperCase(model.ClassName);
            var result = await _classRepository.UpdateAsync(entityToUpdate);

            if (result)
            {
                var item = _mapper.Map<ClassModelDto>(entityToUpdate);
                return ResponseData<ClassModelDto>.Success(item, "Updated successfully");
            }
            return ResponseData<ClassModelDto>.Failure("Update failed");
        }

        public Task<ResponseData<ClassModelDto>> GetFirstOrDefaulteAsync(ClassModelDto data)
        {
            return GetFirstOrDefaultAsync(data);
        }

        public async Task<ResponseData<ClassModelDto>> GetFirstOrDefaultAsync(ClassModelDto data)
        {
            if (data == null)
                return ResponseData<ClassModelDto>.Failure("Request data cannot be null");
            if (string.IsNullOrWhiteSpace(data.ClassName) && data.ClassID <= 0)
            {
                return ResponseData<ClassModelDto>.Failure("At least one parameter (Class ID or Name) must be provided");
            }

            var existClass = await _classRepository
                .GetFirstOrDefaultAsync(c => (data.ClassID != null && c.ClassID == data.ClassID) ||
                                             (!string.IsNullOrEmpty(data.ClassName) && c.ClassName.ToLower() == data.ClassName.ToLower())
                                        );
            if (existClass == null)
                return ResponseData<ClassModelDto>.Failure("Class not found");

            var result = _mapper.Map<ClassModelDto>(existClass);
            return ResponseData<ClassModelDto>.Success(result, "Operation succeeded");
        }

        public async Task<ResponseData<PagedResponse<ClassModelDto>>> GetPagedAsync(PageRequest pageRequest)
        {
            Expression<Func<Ismail_Classes, bool>> filter = BuildFilterExpression(pageRequest);
            var (data, totalCount) = await _classRepository.GetPagedAsync(
                pageRequest.pageNumber,
                pageRequest.pageSize,
                filter,
                pageRequest.OrderByColumn,
                pageRequest.IsAscending,
                true);
            var item = _mapper.Map<List<ClassModelDto>>(data);
            var pageResponse = PagedResponse<ClassModelDto>.Create(item, totalCount, pageRequest.pageNumber, pageRequest.pageSize);
            return ResponseData<PagedResponse<ClassModelDto>>.Success(pageResponse, "Operation succeeded");

        }
        private Expression<Func<Ismail_Classes, bool>> BuildFilterExpression(PageRequest request)
        {
            var parameter = Expression.Parameter(typeof(Ismail_Classes), "c");
            Expression baseFilter = Expression.Constant(true);


            var toStringMethod = typeof(object).GetMethod("ToString", Type.EmptyTypes);
            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);


            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                var searchTerm = Expression.Constant(request.SearchTerm.ToLower());


                var classIdProperty = Expression.Property(parameter, nameof(Ismail_Classes.ClassID));
                var classIdToString = Expression.Call(classIdProperty, toStringMethod);
                var classIdContains = Expression.Call(classIdToString, containsMethod, searchTerm);

                var classNameProperty = Expression.Property(parameter, nameof(Ismail_Classes.ClassName));
                var classNameToLower = Expression.Call(classNameProperty, toLowerMethod);
                var classNameContains = Expression.Call(classNameToLower, containsMethod, searchTerm);


                var searchTremContains = Expression.OrElse(classNameContains, classIdContains);
                baseFilter = Expression.AndAlso(baseFilter, searchTremContains);
            }
            if (request.Filters != null && request.Filters.Any())
            {
                foreach (var filterItem in request.Filters)
                {
                    var property = Expression.Property(parameter, filterItem.Key);
                    Expression filterExpression;

                    if (property.Type == typeof(string))
                    {

                        var propertyToLower = Expression.Call(property, toLowerMethod);
                        var filterValue = Expression.Constant(filterItem.Value.ToString().ToLower());

                        filterExpression = Expression.Call(propertyToLower, containsMethod, filterValue);
                    }
                    else
                    {
                        var convertValue = Convert.ChangeType(filterItem.Value, property.Type);
                        var filterValue = Expression.Constant(convertValue, property.Type);

                        filterExpression = Expression.Equal(property, filterValue);
                    }
                    baseFilter = Expression.AndAlso(baseFilter, filterExpression);
                }
            }
            return Expression.Lambda<Func<Ismail_Classes, bool>>(baseFilter, parameter);
        }

    }
}

