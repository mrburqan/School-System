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
    public class ErrorLogsService : IErrorLogsService
    {
        private readonly IGenericRepository<Ismail_ErrorLogs> _errorRepository;
        private readonly IMapper _mapper;

        public ErrorLogsService(IGenericRepository<Ismail_ErrorLogs> errorRepository, IMapper mapper)
        {
            _errorRepository = errorRepository;
            _mapper = mapper;
        }

        public async Task<ResponseData<bool>> AddErrorLogAsync(ErrorLogsModel errorLogs)
        {
            if (errorLogs == null)
                return ResponseData<bool>.Failure("Logs data is null");


            var entity = _mapper.Map<Ismail_ErrorLogs>(errorLogs);
            entity.CreatedAt = DateTime.UtcNow;
            var result = await _errorRepository.AddAsync(entity);
            if (!result)
                return ResponseData<bool>.Failure("Failed to log error");

            return ResponseData<bool>.Success(result, "Error logged successfully");
        }

        public async Task<ResponseData<bool>> ClearAllLogsAsync()
        {
            var result = await _errorRepository.DeleteRange(x => true);
            if (!result) return ResponseData<bool>.Failure("Failed clear  logs or error is empty");

            return ResponseData<bool>.Success(true, " All logs Cleared successfully");

        }
        public async Task<ResponseData<bool>> ResolveAndDeleteAsync(int errorId)
        {
            if (errorId <= 0)
                return ResponseData<bool>.Failure("Valid error id is required");

            var isExist = await _errorRepository.AnyAsync(x => x.ErrorId == errorId);
            if (!isExist)
                return ResponseData<bool>.Failure("Error log not found");

            var result = await _errorRepository.DeleteFirstOrDefault(x => x.ErrorId == errorId);
            if (!result) return ResponseData<bool>.Failure("Failed error delete error log");

            return ResponseData<bool>.Success(true, "Error resolve and remove successfully");
        }

        public async Task<ResponseData<PagedResponse<ErrorLogsModelDto>>> GetPagedAsync(PageRequest request)
        {
            if (request == null)
                return ResponseData<PagedResponse<ErrorLogsModelDto>>.Failure("Request data is required");

            Expression<Func<Ismail_ErrorLogs, bool>> filter = BuildFilterExpression(request);
            var (data, totalCount) = await _errorRepository.GetPagedAsync(
                request.pageNumber,
                request.pageSize,
                filter,
                request.OrderByColumn,
                request.IsAscending,
                true
                );

            var items = _mapper.Map<List<ErrorLogsModelDto>>(data);
            var pagedResponse = PagedResponse<ErrorLogsModelDto>.Create(items, totalCount, request.pageNumber, request.pageSize);
            return ResponseData<PagedResponse<ErrorLogsModelDto>>.Success(pagedResponse);

        }

        private Expression<Func<Ismail_ErrorLogs, bool>> BuildFilterExpression(PageRequest request)
        {
            var parameter = Expression.Parameter(typeof(Ismail_ErrorLogs), "p");

            Expression baseFilter = Expression.Constant(true);

            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                var errorIdProperty = Expression.Property(parameter, nameof(Ismail_ErrorLogs.ErrorId));
                var errorIdToString = Expression.Call(errorIdProperty, "ToString", Type.EmptyTypes);

                var errorCodeProperty = Expression.Property(parameter, nameof(Ismail_ErrorLogs.ErrorCode));
                var searchTermConstant = Expression.Constant(request.SearchTerm);

                var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });

                var errorIdContains = Expression.Call(errorIdToString, containsMethod, searchTermConstant);
                var errorCodeContains = Expression.Call(errorCodeProperty, containsMethod, searchTermConstant);

                var searchTermConditions = Expression.OrElse(errorIdContains, errorCodeContains);

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
                        var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
                        var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });

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


            var filter = Expression.Lambda<Func<Ismail_ErrorLogs, bool>>(baseFilter, parameter);

            return filter;
        }

    }
}
