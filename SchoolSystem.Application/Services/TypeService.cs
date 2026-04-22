using AutoMapper;
using SchoolSystem.Application.Constans;
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
    public class TypeService : ITypeService
    {
        private readonly IGenericRepository<Ismail_Types> _typeRepository;
        private readonly IMapper _mapper;
        private readonly IUtilitiesService _utilities;

        public TypeService(IGenericRepository<Ismail_Types> typeRepository, IMapper mapper, IUtilitiesService utilitiesService)
        {
            _typeRepository = typeRepository;
            _mapper = mapper;
            _utilities = utilitiesService;
        }

        public async Task<ResponseData<TypeModel>> AddAsync(TypeModel model)
        {
            if (model == null)
                return ResponseData<TypeModel>.Failure("Model is required");
            if (string.IsNullOrWhiteSpace(model.TypeDesc))
                return ResponseData<TypeModel>.Failure("Type description is required");

            if (!Enum.IsDefined(typeof(UserType), model.TypeID))
            {
                return ResponseData<TypeModel>.Failure("ID Should be in range (1 - 3)");
            }

            var expectedType = ((UserType)model.TypeID).ToString().ToLower();
            if (model.TypeDesc.ToLower().Trim() != expectedType)
            {
                return ResponseData<TypeModel>.Failure($"TypeDesc must match the ID Type: {expectedType}");
            }

            var idExist = await _typeRepository.AnyAsync(x => x.TypeID == model.TypeID);
            if (idExist)
            {
                return ResponseData<TypeModel>.Failure("ID already exists");
            }

            var typeExist = await _typeRepository.AnyAsync(x => x.TypeDesc.Trim().ToLower() == model.TypeDesc.Trim().ToLower());
            if (typeExist)
            {
                return ResponseData<TypeModel>.Failure("Type already exists");
            }
            model.TypeDesc = await _utilities.FirstCharecterToApperCase(model.TypeDesc);

            var newModel = _mapper.Map<Ismail_Types>(model);
            var result = await _typeRepository.AddAsync(newModel);
            if (!result)
            {
                return ResponseData<TypeModel>.Failure("Failed to add type");
            }
            return ResponseData<TypeModel>.Success(model, "Type added successfully");
        }

        public async Task<ResponseData<TypeModelDto>> GetFirstOrDefaultAsync(TypeModelDto model)
        {
            if (model == null)
                return ResponseData<TypeModelDto>.Failure("Search model is required");

            if (model.TypeID == null && string.IsNullOrWhiteSpace(model.TypeDesc))
            {
                return ResponseData<TypeModelDto>.Failure("At least one of the parameters (TypeID or TypeDesc) must be provided");
            }

            if (model.TypeID != null && !Enum.IsDefined(typeof(UserType), model.TypeID))
            {
                return ResponseData<TypeModelDto>.Failure("ID Should be in range (1 - 3)");
            }

            if (!string.IsNullOrWhiteSpace(model.TypeDesc))
            {
                var expectedType = model.TypeDesc.ToLower();
                if (expectedType != UserDescType.Admin && expectedType != UserDescType.Teacher && expectedType != UserDescType.Student)
                {
                    return ResponseData<TypeModelDto>.Failure("TypeDesc should be 'Admin', 'Teacher', or 'Student'");
                }
            }

            var type = await _typeRepository.GetFirstOrDefaultAsync(x =>
                (model.TypeID != null && x.TypeID == model.TypeID) ||
                (!string.IsNullOrEmpty(model.TypeDesc) && x.TypeDesc.ToLower() == model.TypeDesc.ToLower())
            );

            if (type == null)
            {
                return ResponseData<TypeModelDto>.Failure("Type not found");
            }

            var dto = _mapper.Map<TypeModelDto>(type);
            return ResponseData<TypeModelDto>.Success(dto, "Type retrieved successfully");

        }

        public async Task<ResponseData<TypeModelDto>> UpdateAsync(TypeModel model)
        {
            if (!Enum.IsDefined(typeof(UserType), model.TypeID))
            {
                return ResponseData<TypeModelDto>.Failure("ID should be in range(1 - 3)");
            }

            var expectedDesc = ((UserType)model.TypeID).ToString();
            if (!string.Equals(model.TypeDesc, expectedDesc, StringComparison.OrdinalIgnoreCase))
            {
                return ResponseData<TypeModelDto>.Failure($"ID {model.TypeID} must have the description: {expectedDesc}");
            }

            var existingType = await _typeRepository.GetFirstOrDefaultAsync(x => x.TypeID == model.TypeID);
            if (existingType == null)
            {
                return ResponseData<TypeModelDto>.Failure("Type not exists");
            }
            _mapper.Map(model, existingType);
            var result = await _typeRepository.UpdateAsync(existingType);
            if (!result)
            {
                return ResponseData<TypeModelDto>.Failure("Update failed at database level");
            }
            var entitiyDto = _mapper.Map<TypeModelDto>(existingType);
            return ResponseData<TypeModelDto>.Success(entitiyDto, "Updated successfully");
        }


        public async Task<ResponseData<PagedResponse<TypeModelDto>>> GetPageAsync(PageRequest request)
        {
            if (request == null)
                return ResponseData<PagedResponse<TypeModelDto>>.Failure("Request data is required");

            Expression<Func<Ismail_Types, bool>> filter = BuildFilterExpression(request);

            var (data, totalCount) = await _typeRepository.GetPagedAsync(
                request.pageNumber,
                request.pageSize,
                filter,
                request.OrderByColumn,
                request.IsAscending,
                true
            );

            var items = _mapper.Map<List<TypeModelDto>>(data);


            var pagedResponse = PagedResponse<TypeModelDto>.Create(items, totalCount, request.pageNumber, request.pageSize);

            return ResponseData<PagedResponse<TypeModelDto>>.Success(pagedResponse);
        }

        private Expression<Func<Ismail_Types, bool>> BuildFilterExpression(PageRequest request)
        {
            var parameter = Expression.Parameter(typeof(Ismail_Types), "p");

            Expression baseFilter = Expression.Constant(true);

            var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
            var toStringMethod = typeof(object).GetMethod("ToString");
            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });

            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                var searchTermConstant = Expression.Constant(request.SearchTerm);

                var typeIdProperty = Expression.Property(parameter, nameof(Ismail_Types.TypeID));
                var typeIdToString = Expression.Call(typeIdProperty, toStringMethod);
                var typeIdContains = Expression.Call(typeIdToString, containsMethod, searchTermConstant);


                var typeDescProperty = Expression.Property(parameter, nameof(Ismail_Types.TypeDesc));
                var typeDescToLower = Expression.Call(typeDescProperty, toLowerMethod);
                var typeDescContains = Expression.Call(typeDescToLower, containsMethod, searchTermConstant);


                var searchTermConditions = Expression.OrElse(typeIdContains, typeDescContains);

                baseFilter = Expression.AndAlso(baseFilter, searchTermConditions);
            }

            if (request.Filters != null && request.Filters.Any())
            {
                foreach (var filterItem in request.Filters)
                {
                    if (filterItem.Value == null || string.IsNullOrWhiteSpace(filterItem.Value.ToString()))
                        continue;

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
                        var targetType = Nullable.GetUnderlyingType(property.Type) ?? property.Type;
                        var convertedValue = Convert.ChangeType(filterItem.Value, targetType);
                        var filterValue = Expression.Constant(convertedValue, property.Type);

                        filterExpression = Expression.Equal(property, filterValue);
                    }

                    baseFilter = Expression.AndAlso(baseFilter, filterExpression);
                }
            }


            var filter = Expression.Lambda<Func<Ismail_Types, bool>>(baseFilter, parameter);

            return filter;
        }


    }
}
