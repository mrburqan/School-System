using AutoMapper;
using SchoolSystem.Application.Models;
using SchoolSystem.Application.Models.Request;
using SchoolSystem.Application.Models.Response;
using SchoolSystem.Application.Services.IServices;
using SchoolSystem.Core.Entites;
using SchoolSystem.Infrastructure.Repositories.CustomRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SchoolSystem.Application.Services
{
    public class AssociationService : IAssociationService
    {
        private readonly AssociationRepository _associationRepository;
        private readonly IMapper _mapper;

        public AssociationService(AssociationRepository associationRepository, IMapper mapper)
        {
            _associationRepository = associationRepository;
            _mapper = mapper;
        }

        public async Task<ResponseData<bool>> AddAssociationAsync(AssociationModel association)
        {
            var result = await _associationRepository.AddAssociationAsync(association.ClassId, association.UserId);

            if (!result)
                return ResponseData<bool>.Failure("Failed to add student to class. Maybe they are already enrolled?");
            return ResponseData<bool>.Success(true, "Student added to class successfully.");
        }

        public async Task<ResponseData<bool>> CheckIfEnrolledAsync(int userId, int classId)
        {
            var exists = await _associationRepository.ExistsAsync(userId, classId);
            if (!exists)
                return ResponseData<bool>.Failure("Student is not enrolled in the class.");
            return ResponseData<bool>.Success(true, "Student is enrolled in the class.");
        }

        public async Task<ResponseData<bool>> DeleteAssociationAsync(int userId, int classId)
        {
            var isExistesStudent = await _associationRepository.ExistsAsync(userId, classId);
            if (!isExistesStudent)
            {
                return ResponseData<bool>.Failure("Student is not enrolled in this class.");
            }
            var result = await _associationRepository.DeleteAssociationAsync(userId, classId);
            if (!result)
                return ResponseData<bool>.Failure("Student is not removed from class successfully");

            return ResponseData<bool>.Success(true, "Student removed from class successfully.");


        }

        public async Task<ResponseData<IEnumerable<AssociationModelDto>>> GetAssociationAsync(int? classId, int? userID)
        {

            var clients = await _associationRepository.GetAssociationsAsync(classId, userID);

            var result = clients.SelectMany(client => (client.Ismail_Classes ?? Enumerable.Empty<Ismail_Classes>())
            .Select(schoolSystem => new AssociationModelDto
            {
                UserId = client.UserID,
                UserName = client.UserName,
                ClassId = schoolSystem.ClassID,
                ClassName = schoolSystem.ClassName,
                Name = client.Name,
                TypeDesc = client.Ismail_Types?.TypeDesc,
            })).ToList();

            if (!result.Any())
            {
                return ResponseData<IEnumerable<AssociationModelDto>>.Failure("No associations found.");
            }
            return ResponseData<IEnumerable<AssociationModelDto>>.Success(result, "Associations retrieved successfully.");
        }

        public async Task<ResponseData<AssociationModelDto>> UpdateAssociationAsync(AssociationModel association)
        {
            var result = await _associationRepository.UpdateAssociationAsync(association.UserId, association.ClassId, association.newClassId.Value);
            if (result)
            {
                var dtos = _mapper.Map<AssociationModelDto>(result);
                return ResponseData<AssociationModelDto>.Success(dtos, "Association updated successfully.");
            }
            return ResponseData<AssociationModelDto>.Failure("Failed to update association. Maybe the student is not enrolled in the original class?");
        }

        public async Task<ResponseData<PagedResponse<AssociationModelDto>>> GetPagedAsync(PageRequest pageRequest)
        {
            Expression<Func<Ismail_Clients, bool>> filter = BuildFilterExpression(pageRequest);
            var (data, totalCount) = await _associationRepository.GetPagedAsync(
                pageRequest.pageNumber,
                pageRequest.pageSize,
                filter,
               pageRequest.OrderByColumn,
                pageRequest.IsAscending,
                true,
                includes: new Expression<Func<Ismail_Clients, object>>[]
                {
                    c => c.Ismail_Classes,
                    c => c.Ismail_Types
                }
                );
            var dtos = data.Select(client => new AssociationModelDto
            {
                UserId = client.UserID,
                UserName = client.UserName,
                ClassId = client.Ismail_Classes.FirstOrDefault()?.ClassID ?? 0,
                ClassName = client.Ismail_Classes.FirstOrDefault()?.ClassName,
                Name = client.Name,
                TypeDesc = client.Ismail_Types?.TypeDesc
            }).ToList();
            return ResponseData<PagedResponse<AssociationModelDto>>.Success(
                PagedResponse<AssociationModelDto>.Create(dtos, totalCount, pageRequest.pageNumber, pageRequest.pageSize), "Paged associations retrieved successfully.");
        }

        private Expression<Func<Ismail_Clients, bool>> BuildFilterExpression(PageRequest request)
        {
            var parameter = Expression.Parameter(typeof(Ismail_Clients), "c");
            Expression baseFilter = Expression.Constant(true);


            var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
            var toStringMethod = typeof(object).GetMethod("ToString", Type.EmptyTypes);
            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });


            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                var searchTerm = Expression.Constant(request.SearchTerm.ToLower());


                var userNameProperty = Expression.Property(parameter, nameof(Ismail_Clients.UserName));
                var userNameToLower = Expression.Call(userNameProperty, toLowerMethod);
                var userNameContains = Expression.Call(userNameToLower, containsMethod, searchTerm);

                var userIdProperty = Expression.Property(parameter, nameof(Ismail_Clients.UserID));
                var usreIdToString = Expression.Call(userIdProperty, toStringMethod);
                var userIDContains = Expression.Call(usreIdToString, containsMethod, searchTerm);


                var searchTremContains = Expression.OrElse(userNameContains, userIDContains);
                baseFilter = Expression.AndAlso(baseFilter, searchTremContains);
            }

            if (request.Filters != null && request.Filters.Any())
            {
                foreach (var filter in request.Filters)
                {
                    var property = Expression.Property(parameter, filter.Key);
                    Expression filterExpression;

                    if (property.Type == typeof(string))
                    {
                        var propertyToLower = Expression.Call(property, toLowerMethod);
                        var filterValue = Expression.Constant(filter.Value.ToString().ToLower());

                        filterExpression = Expression.Call(propertyToLower, containsMethod, filterValue);
                    }
                    else
                    {
                        var convertedValue = Convert.ChangeType(filter.Value, property.Type);
                        var filterValue = Expression.Constant(convertedValue, property.Type);

                        filterExpression = Expression.Equal(property, filterValue);
                    }
                    baseFilter = Expression.AndAlso(baseFilter, filterExpression);
                }
            }

            return Expression.Lambda<Func<Ismail_Clients, bool>>(baseFilter, parameter);
        }
    }

}
