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
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SchoolSystem.Application.Services
{
    public class ClientService : IClientService
    {
        private readonly IGenericRepository<Ismail_Clients> _clientRepository;
        private readonly IMapper _mapper;
        private readonly IUtilitiesService _utilites;

        public ClientService(IGenericRepository<Ismail_Clients> clientRepository, IMapper mapper, IUtilitiesService utilites)
        {
            _clientRepository = clientRepository;
            _mapper = mapper;
            _utilites = utilites;
        }

        public async Task<ResponseData<ClientModelDto>> AddAsync(ClientModel model)
        {
            if (model == null) return ResponseData<ClientModelDto>.Failure("Data is required");
            if (string.IsNullOrWhiteSpace(model.UserName))
                return ResponseData<ClientModelDto>.Failure("UserName is required");
            bool isTryingToBeAdmin = model.UserName.Equals("Admin", StringComparison.OrdinalIgnoreCase) || model.TypeID == 1;
            if (isTryingToBeAdmin)
            {
                var adminExist = await _clientRepository.AnyAsync(x => x.TypeID == 1 || x.UserName.ToLower() == "admin");
                if (adminExist) return ResponseData<ClientModelDto>.Failure("An Admin already exists.");
            }


            var UserNameTaken = await _clientRepository.AnyAsync(x => (x.UserName.ToLower() == model.UserName.ToLower()) && (x.UserID != model.UserID));
            if (UserNameTaken)
                return ResponseData<ClientModelDto>.Failure("User name already taken ");


            if (!Enum.IsDefined(typeof(UserGender), model.GenderID))
                return ResponseData<ClientModelDto>.Failure("Gender ID must be 1 (Male) or 2 (Female).");

            if (!Enum.IsDefined(typeof(UserType), model.TypeID))
                return ResponseData<ClientModelDto>.Failure("Type ID must be between 1 and 3.");

            var dbContext = _clientRepository.GetContext();

            try
            {
                using (var transaction = dbContext.Database.BeginTransaction())
                {
                    var clientEntity = _mapper.Map<Ismail_Clients>(model);

                    var isSaved = await _clientRepository.AddAsync(clientEntity);
                    if (!isSaved)
                    {
                        transaction.Rollback();
                        return ResponseData<ClientModelDto>.Failure("Failed to save Client Entity.");
                    }
                    if (!string.IsNullOrEmpty(model.NameAR))
                    {
                        var translation = new Ismail_TranslationCientLanguage
                        {
                            UserID = clientEntity.UserID,
                            DisplayName = model.NameAR,
                        };
                        dbContext.Ismail_TranslationCientLanguage.Add(translation);
                        await dbContext.SaveChangesAsync();
                    }
                    transaction.Commit();

                    var resultDto = _mapper.Map<ClientModelDto>(clientEntity);
                    return ResponseData<ClientModelDto>.Success(resultDto, "Added successfully.");
                }
            }
            catch (Exception)
            {
                return ResponseData<ClientModelDto>.Failure("An unexpected error occurred during add.");
            }
        }

        public async Task<ResponseData<bool>> DeleteFirstOrDefaultAsync(int id)
        {
            if (id <= 0)
            {
                return ResponseData<bool>.Failure("Please provide either  User ID ");
            }
            var dbContext = _clientRepository.GetContext();
            using (var transaction = dbContext.Database.BeginTransaction())
            {


                var entityToDelete = await dbContext.Ismail_Clients.FindAsync(id);

                if (entityToDelete == null)
                {
                    return ResponseData<bool>.Failure($"No user found with the provided details.");
                }
                bool isAdmin = entityToDelete.TypeID == 1 ||
                       entityToDelete.UserName.Equals("Admin", StringComparison.OrdinalIgnoreCase);

                if (isAdmin)
                {
                    transaction.Rollback();
                    return ResponseData<bool>.Failure("Security Restriction: Admin accounts cannot be deleted ");
                }
                var relatedTranslations = dbContext.Ismail_TranslationCientLanguage.Where(x => x.UserID == id);
                if (relatedTranslations.Any())
                {
                    dbContext.Ismail_TranslationCientLanguage.RemoveRange(relatedTranslations);
                }

                dbContext.Ismail_Clients.Remove(entityToDelete);


                var affectedRows = await dbContext.SaveChangesAsync();
                transaction.Commit();


                if (affectedRows <= 0)
                {
                    transaction.Rollback();
                    return ResponseData<bool>.Failure("The delete operation failed in the database.");
                }

                return ResponseData<bool>.Success(true, $"User '{entityToDelete.UserName}' has been deleted successfully.");

            }
        }

        public async Task<ResponseData<ClientModelDto>> UpdateAsync(ClientModel model)
        {
            if (model == null || model.UserID <= 0)
                return ResponseData<ClientModelDto>.Failure("Valid data and User ID are required.");
            if (string.IsNullOrWhiteSpace(model.UserName))
                return ResponseData<ClientModelDto>.Failure("UserName is required.");

            if (!Enum.IsDefined(typeof(UserGender), model.GenderID))
                return ResponseData<ClientModelDto>.Failure("Gender ID must be 1 (Male) or 2 (Female).");
            if (!Enum.IsDefined(typeof(UserType), model.TypeID))
                return ResponseData<ClientModelDto>.Failure("Type ID must be between 1 and 3.");

            if (model.TypeID == 1 || model.UserName.ToLower().Equals("admin"))
            {
                var isAdminExist = await _clientRepository.GetFirstOrDefaultAsync(x => x.TypeID == 1);

                if (isAdminExist != null && isAdminExist.UserID != model.UserID)
                {
                    return ResponseData<ClientModelDto>.Failure("An Admin already exists. You cannot assign another Admin.");
                }
            }


            var isUserNameTaken = await _clientRepository.AnyAsync(x =>
                x.UserName.ToLower() == model.UserName.ToLower() && x.UserID != model.UserID);

            if (isUserNameTaken)
            {
                return ResponseData<ClientModelDto>.Failure("This UserName is already taken by another user.");
            }


            var dbContext = _clientRepository.GetContext();

            try
            {
                using (var transaction = dbContext.Database.BeginTransaction())
                {
                    var existingClient = await dbContext.Ismail_Clients.FindAsync(model.UserID);
                    if (existingClient == null)
                        return ResponseData<ClientModelDto>.Failure("User not found.");


                    _mapper.Map(model, existingClient);


                    existingClient.UserName = await _utilites.FirstCharecterToApperCase(model.UserName);
                    existingClient.Name = await _utilites.FirstCharecterToApperCase(model.Name);

                    var isUpdated = await _clientRepository.UpdateAsync(existingClient);
                    if (!isUpdated)
                    {
                        transaction.Rollback();
                        return ResponseData<ClientModelDto>.Failure("Update operation failed in database.");
                    }
                    transaction.Commit();
                    var updatedItem = await _clientRepository.GetFirstOrDefaultAsync(x => x.UserID == model.UserID);
                    var result = _mapper.Map<ClientModelDto>(updatedItem);
                    result.GenderDesc = updatedItem?.Ismail_Gender?.GenderDesc;
                    result.TypeDesc = updatedItem?.Ismail_Types?.TypeDesc;
                    result.NameAR = updatedItem?.Ismail_TranslationCientLanguage?.FirstOrDefault()?.DisplayName;
                    result.LanguageID = updatedItem?.Ismail_TranslationCientLanguage?.FirstOrDefault()?.LanguageID;
                    var classItem = updatedItem?.Ismail_Classes?.FirstOrDefault();
                    result.ClassID = classItem?.ClassID;
                    result.ClassName = classItem?.ClassName;

                    return ResponseData<ClientModelDto>.Success(result, "Updated successfully");
                }
            }
            catch (Exception)
            {
                return ResponseData<ClientModelDto>.Failure("An unexpected error occurred during update.");
            }
        }

        public async Task<ResponseData<ClientModelDto>> GetByFirstOrDefult(ClientModelDto model)
        {
            if (model == null) return ResponseData<ClientModelDto>.Failure("Criteria is required");

            var item = await _clientRepository.GetQueryable()
                .Include(x => x.Ismail_Gender)
                .Include(x => x.Ismail_Types)
                .Include(x => x.Ismail_TranslationCientLanguage)
                .Include(x => x.Ismail_Classes)
                .FirstOrDefaultAsync(x =>
                    (model.UserID > 0 ? x.UserID == model.UserID : true) &&
                    (!string.IsNullOrEmpty(model.UserName) ? x.UserName.ToLower() == model.UserName.ToLower() : true)
                );

            if (item == null)
                return ResponseData<ClientModelDto>.Failure("No client found.");

            var result = _mapper.Map<ClientModelDto>(item);

            result.GenderDesc = item.Ismail_Gender?.GenderDesc;
            result.TypeDesc = item.Ismail_Types?.TypeDesc;
            result.NameAR = item.Ismail_TranslationCientLanguage?.FirstOrDefault()?.DisplayName;
            result.LanguageID = item.Ismail_TranslationCientLanguage?.FirstOrDefault()?.LanguageID;
            var classItem = item.Ismail_Classes?.FirstOrDefault();
            result.ClassID = classItem?.ClassID;
            result.ClassName = classItem?.ClassName;

            return ResponseData<ClientModelDto>.Success(result, "Success");
        }
        public async Task<ResponseData<PagedResponse<ClientModelDto>>> GetPagedAsync(PageRequest request)
        {
            if (request == null)
                return ResponseData<PagedResponse<ClientModelDto>>.Failure("Request data is required");

            Expression<Func<Ismail_Clients, bool>> filter = BuildFilterExpression(request);
            var (data, totalCount) = await _clientRepository.GetPagedAsync(
                request.pageNumber,
                request.pageSize,
                filter,
                request.OrderByColumn,
                request.IsAscending,
                true,
                includes: new Expression<Func<Ismail_Clients, object>>[]
                    {
                        x => x.Ismail_Gender,
                        x => x.Ismail_Types,
                        x => x.Ismail_TranslationCientLanguage,
                        x => x.Ismail_Classes
                    });



            var items = _mapper.Map<List<ClientModelDto>>(data);

            foreach (var item in items)
            {
                var fullClient = data.FirstOrDefault(x => x.UserID == item.UserID);
                if (fullClient == null)
                    continue;

                item.GenderDesc = fullClient.Ismail_Gender?.GenderDesc;
                item.TypeDesc = fullClient.Ismail_Types?.TypeDesc;
                item.NameAR = fullClient.Ismail_TranslationCientLanguage?.FirstOrDefault()?.DisplayName;
                item.LanguageID = fullClient.Ismail_TranslationCientLanguage?.FirstOrDefault()?.LanguageID;
                var classItem = fullClient.Ismail_Classes?.FirstOrDefault();
                item.ClassID = classItem?.ClassID;
                item.ClassName = classItem?.ClassName;
            }

            var pagedResponse = PagedResponse<ClientModelDto>.Create(items, totalCount, request.pageNumber, request.pageSize);
            return ResponseData<PagedResponse<ClientModelDto>>.Success(pagedResponse, "Data retrieved successfully");
        }

        private Expression<Func<Ismail_Clients, bool>> BuildFilterExpression(PageRequest request)
        {
            var parameter = Expression.Parameter(typeof(Ismail_Clients), "c");
            Expression baseFilter = Expression.Constant(true);

            var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            var toStringMethod = typeof(object).GetMethod("ToString", Type.EmptyTypes);


            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                var searchTerm = Expression.Constant(request.SearchTerm.ToLower());

                var userIdProperty = Expression.Property(parameter, nameof(Ismail_Clients.UserID));
                var userIdToString = Expression.Call(userIdProperty, toStringMethod);
                var userIdContains = Expression.Call(userIdToString, containsMethod, searchTerm);


                var userNameProperty = Expression.Property(parameter, nameof(Ismail_Clients.UserName));
                var userNameToLower = Expression.Call(userNameProperty, toLowerMethod);
                var userNameContains = Expression.Call(userNameToLower, containsMethod, searchTerm);


                var searchTremContains = Expression.OrElse(userNameContains, userIdContains);
                baseFilter = Expression.AndAlso(baseFilter, searchTremContains);
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
                        var convertValue = Convert.ChangeType(filterItem.Value, property.Type);
                        var filterValue = Expression.Constant(convertValue, property.Type);

                        filterExpression = Expression.Equal(property, filterValue);
                    }
                    baseFilter = Expression.AndAlso(baseFilter, filterExpression);
                }
            }
            return Expression.Lambda<Func<Ismail_Clients, bool>>(baseFilter, parameter);
        }

    }
}

