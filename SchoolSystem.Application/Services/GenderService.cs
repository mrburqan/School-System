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
    public class GenderService : IGenderService
    {
        private readonly IGenericRepository<Ismail_Gender> _genderRepository;
        private readonly IUtilitiesService _utilities;
        private readonly IMapper _mapper;

        public GenderService(IGenericRepository<Ismail_Gender> genderRepository, IUtilitiesService utilities, IMapper mapper)
        {
            _genderRepository = genderRepository;
            _utilities = utilities;
            _mapper = mapper;
        }

        public async Task<ResponseData<GenderModel>> AddAsync(GenderModel model)
        {
            if (model == null)
                return ResponseData<GenderModel>.Failure("Model is required");
            if (string.IsNullOrWhiteSpace(model.GenderDesc))
                return ResponseData<GenderModel>.Failure("Gender description is required");

            if (!Enum.IsDefined(typeof(UserGender), model.GenderID))
            {
                return ResponseData<GenderModel>.Failure("ID should be in range(1 - 2)");
            }
            var expectedGender = ((UserGender)model.GenderID).ToString().ToLower();
            if (model.GenderDesc.Trim().ToLower() != expectedGender)
            {
                return ResponseData<GenderModel>.Failure($"Gender description must match the ID Type:{expectedGender}");
            }

            var ifExist = await _genderRepository.AnyAsync(g => g.GenderID == model.GenderID);
            if (ifExist)
            {
                return ResponseData<GenderModel>.Failure("Gender ID already exists");
            }
            var genderExist = await _genderRepository.AnyAsync(g => g.GenderDesc.Trim().ToLower() == model.GenderDesc.Trim().ToLower());
            if (genderExist)
            {
                return ResponseData<GenderModel>.Failure("Gender description already exists");
            }
            model.GenderDesc = await _utilities.FirstCharecterToApperCase(model.GenderDesc);

            var newModel = _mapper.Map<Ismail_Gender>(model);

            var result = await _genderRepository.AddAsync(newModel);
            if (!result)
            {
                return ResponseData<GenderModel>.Failure("Failed to add");
            }
            return ResponseData<GenderModel>.Success(model, "Gender added successfully");

        }

        public async Task<ResponseData<GenderModelDto>> GetFirstOrDefaultAsync(GenderModelDto model)
        {
            if (model == null)
                return ResponseData<GenderModelDto>.Failure("Search model is required");

            if (model.GenderID == null && string.IsNullOrEmpty(model.GenderDesc))
            {
                return ResponseData<GenderModelDto>.Failure("Please provide at least one search criteria (ID or Description)");
            }

            if (model.GenderID != null && !Enum.IsDefined(typeof(UserGender), model.GenderID))
            {
                return ResponseData<GenderModelDto>.Failure("ID should be in range(1 - 2)");
            }

            if (!string.IsNullOrWhiteSpace(model.GenderDesc))
            {
                if (!string.Equals(model.GenderDesc, UserGenderDesc.Male, StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(model.GenderDesc, UserGenderDesc.Female, StringComparison.OrdinalIgnoreCase))
                {
                    return ResponseData<GenderModelDto>.Failure("Gender description should be 'Male 'or 'Female'");
                }
            }

            var gender = await _genderRepository.GetFirstOrDefaultAsync(g =>
                (model.GenderID != null && g.GenderID == model.GenderID ||
                (!string.IsNullOrEmpty(model.GenderDesc) && g.GenderDesc.ToLower() == model.GenderDesc.ToLower()))
            );

            if (gender == null)
            {
                return ResponseData<GenderModelDto>.Failure("No gender found ");
            }

            var dto = _mapper.Map<GenderModelDto>(gender);
            return ResponseData<GenderModelDto>.Success(dto, "Gender retrieved successfully");

        }


        public async Task<ResponseData<GenderModelDto>> UpdateAsync(GenderModel model)
        {
            if (!Enum.IsDefined(typeof(UserGender), model.GenderID))
            {
                return ResponseData<GenderModelDto>.Failure("ID should be in range(1 - 2)");
            }

            var existingGender = await _genderRepository.GetFirstOrDefaultAsync(x => x.GenderID == model.GenderID);
            if (existingGender == null)
            {
                return ResponseData<GenderModelDto>.Failure("Gender record not found ");
            }

            var expectedDesc = ((UserGender)model.GenderID).ToString();
            if (!string.Equals(model.GenderDesc, expectedDesc, StringComparison.OrdinalIgnoreCase))
            {
                return ResponseData<GenderModelDto>.Failure($"ID {model.GenderID} must have the description: {expectedDesc}");

            }

            _mapper.Map(model, existingGender);

            var result = await _genderRepository.UpdateAsync(existingGender);

            if (!result)
                return ResponseData<GenderModelDto>.Failure("Update failed at database level");

            var entityDto = _mapper.Map<GenderModelDto>(existingGender);
            return ResponseData<GenderModelDto>.Success(entityDto, "Data retrive successfully ");
        }

        public async Task<ResponseData<PagedResponse<GenderModelDto>>> GetPageAsync(PageRequest request)
        {
            if (request == null)
                return ResponseData<PagedResponse<GenderModelDto>>.Failure("Request data is required");

            Expression<Func<Ismail_Gender, bool>> filter = BuildFilterExpression(request);

            var (data, totalCount) = await _genderRepository.GetPagedAsync(
                           request.pageNumber,
                           request.pageSize,
                           filter,
                           request.OrderByColumn,
                           request.IsAscending,
                           true
                           );
            var items = _mapper.Map<List<GenderModelDto>>(data);
            var pageResponse = PagedResponse<GenderModelDto>.Create(items, totalCount, request.pageNumber, request.pageSize);
            return ResponseData<PagedResponse<GenderModelDto>>.Success(pageResponse);
        }

        private Expression<Func<Ismail_Gender, bool>> BuildFilterExpression(PageRequest request)
        {
            var parameter = Expression.Parameter(typeof(Ismail_Gender), "p");

            Expression baseFilter = Expression.Constant(true);

            var toStringMethod = typeof(object).GetMethod("ToString", Type.EmptyTypes);
            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);


            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                var searchTermConstant = Expression.Constant(request.SearchTerm.ToLower());

                var genderIdProperty = Expression.Property(parameter, nameof(Ismail_Gender.GenderID));
                var genderIdToString = Expression.Call(genderIdProperty, toStringMethod);
                var genderIdContains = Expression.Call(genderIdToString, containsMethod, searchTermConstant);

                var genderDescProperty = Expression.Property(parameter, nameof(Ismail_Gender.GenderDesc));
                var genderDescToLower = Expression.Call(genderDescProperty, toLowerMethod);
                var genderDescContains = Expression.Call(genderDescToLower, containsMethod, searchTermConstant);

                var searchTermConditions = Expression.OrElse(genderIdContains, genderDescContains);

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


            var filter = Expression.Lambda<Func<Ismail_Gender, bool>>(baseFilter, parameter);

            return filter;
        }

    }
}
