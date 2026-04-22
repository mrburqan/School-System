using AutoMapper;
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
    public class StudentMarkService : IStudentMarkService
    {
        private IGenericRepository<Ismail_StudentMarks> _studentMarkRepository;
        private IGenericRepository<Ismail_Clients> _clientRepository;
        private IMapper _mapper;

        public StudentMarkService(
            IGenericRepository<Ismail_StudentMarks> studentMarkRepository,
            IGenericRepository<Ismail_Clients> clientRepository,
            IMapper mapper)
        {
            _studentMarkRepository = studentMarkRepository;
            _clientRepository = clientRepository;
            _mapper = mapper;
        }

        public async Task<ResponseData<StudentMarkModelDto>> AddAsync(StudentMarkModel model)
        {
            if (model == null)
                return ResponseData<StudentMarkModelDto>.Failure("Mark data is required");

            var studentExist = await _clientRepository.AnyAsync(x => x.UserID == model.StudentID);
            if (!studentExist)
                return ResponseData<StudentMarkModelDto>.Failure("Student not  found ");

            var hasExistingMarks = await _studentMarkRepository.AnyAsync(x => x.StudentID == model.StudentID);
            if (hasExistingMarks)
            {
                return ResponseData<StudentMarkModelDto>.Failure("This student already has a marks record. Use Update instead.");
            }

            var markEntity = _mapper.Map<Ismail_StudentMarks>(model);
            var isSaved = await _studentMarkRepository.AddAsync(markEntity);

            if (!isSaved)
                return ResponseData<StudentMarkModelDto>.Failure();

            var markDto = _mapper.Map<StudentMarkModelDto>(markEntity);
            return ResponseData<StudentMarkModelDto>.Success(markDto, "Data retrive successfully");

        }

        public async Task<ResponseData<bool>> DeleteFirstOrDefaultAsync(int id)
        {
            if (id < 1) return ResponseData<bool>.Failure("Invalid ID ");

            var studentHasMark = await _studentMarkRepository.AnyAsync(x => (x.StudentID == id));
            if (!studentHasMark) return ResponseData<bool>.Failure("No marks found for this student to delete.");

            var isDeleted = await _studentMarkRepository.DeleteFirstOrDefault(x => x.StudentID == id);
            if (!isDeleted)
                return ResponseData<bool>.Failure("Delete operation failed at database level.");

            return ResponseData<bool>.Success(true, "Student marks deleted successfully.");
        }

        public async Task<ResponseData<bool>> DeleteRangeAsync()
        {
            var isDeleted = await _studentMarkRepository.DeleteRange(x => true);
            if (!isDeleted)
                return ResponseData<bool>.Failure("Could not delete marks. Database might be empty or locked.");

            return ResponseData<bool>.Success(true, "All student marks have been cleared successfully.");
        }

        public async Task<ResponseData<StudentMarkModelDto>> GetByAsync(StudentMarkModelDto model)
        {
            var student = await _clientRepository.GetQueryable()
                .Include(x => x.Ismail_Classes).FirstOrDefaultAsync(x =>
            (x.UserID == model.StudentID ||
            x.UserName.Equals(model.UserName, System.StringComparison.CurrentCultureIgnoreCase)) && x.TypeID == 3);

            if (student == null)
                return ResponseData<StudentMarkModelDto>.Failure("Student not exists");

            var mark = await _studentMarkRepository.GetFirstOrDefaultAsync(x => x.StudentID == student.UserID);
            if (mark == null)
                return ResponseData<StudentMarkModelDto>.Failure("No mark yet");




            var resultDto = new StudentMarkModelDto
            {
                StudentID = student.UserID,
                UserName = student.UserName,
                ClassID = student.Ismail_Classes.Select(x => (int?)x.ClassID).ToList(),
                AllClasses = student.Ismail_Classes.Select(x => x.ClassName).ToList(),

                English = mark?.English,
                Math = mark?.Math,
                Physics = mark?.Physics,
                Chemistry = mark?.Chemistry

            };

            int totalMarks = 0;
            int subjectsCount = 0;

            if (resultDto.Physics.HasValue && resultDto.Physics > 0) { totalMarks += resultDto.Physics.Value; subjectsCount++; }
            if (resultDto.Math.HasValue && resultDto.Math > 0) { totalMarks += resultDto.Math.Value; subjectsCount++; }
            if (resultDto.Chemistry.HasValue && resultDto.Chemistry > 0) { totalMarks += resultDto.Chemistry.Value; subjectsCount++; }
            if (resultDto.English.HasValue && resultDto.English > 0) { totalMarks += resultDto.English.Value; subjectsCount++; }

            if (subjectsCount > 0)
            {
                resultDto.Averge = (long)((double)totalMarks / subjectsCount);
            }
            else
            {
                resultDto.Averge = 0;
            }
            return ResponseData<StudentMarkModelDto>.Success(resultDto, "Data retrive successfully .");

        }

        public async Task<ResponseData<StudentMarkModelDto>> UpdateAsync(StudentMarkModel model)
        {
            if (model == null || model.StudentID <= 0)
                return ResponseData<StudentMarkModelDto>.Failure("Invalid data to update");

            var studentMarkExists = await _studentMarkRepository.GetFirstOrDefaultAsync(x => x.StudentID == model.StudentID);
            if (studentMarkExists == null)
                return ResponseData<StudentMarkModelDto>.Failure("No mark found to update for this student");

            _mapper.Map(model, studentMarkExists);

            var isUpdated = await _studentMarkRepository.UpdateAsync(studentMarkExists);
            if (!isUpdated)
                return ResponseData<StudentMarkModelDto>.Failure("Update failed at database level");

            var markDto = _mapper.Map<StudentMarkModelDto>(model);
            return ResponseData<StudentMarkModelDto>.Success(markDto, "Marks updated successfully");
        }

        public async Task<ResponseData<PagedResponse<StudentMarkModelDto>>> GetPagedAsync(PageRequest request)
        {
            Expression<Func<Ismail_StudentMarks, bool>> filter = BuildFilterExpression(request);

            var (data, totalCount) = await _studentMarkRepository.GetPagedAsync(
                request.pageNumber,
                request.pageSize,
                filter,
                request.OrderByColumn,
                request.IsAscending,
                true,
                includes: new Expression<Func<Ismail_StudentMarks, object>>[]
                {
                c=> c.Ismail_Clients
                               }
                );

            var dtos = data.Select(student => new StudentMarkModelDto
            {
                StudentID = student.StudentID,
                UserName = student.Ismail_Clients?.UserName,
                AllClasses = student.Ismail_Clients?.Ismail_Classes?.Select(x => x.ClassName).ToList() ?? new List<string>(),
                English = student.English,
                Math = student.Math,
                Physics = student.Physics,
                Chemistry = student.Chemistry,
            }).ToList();
            var pageResponse = PagedResponse<StudentMarkModelDto>.Create(dtos, totalCount, request.pageNumber, request.pageSize);

            return ResponseData<PagedResponse<StudentMarkModelDto>>.Success(pageResponse, "Data retrive successfully . ");
        }
        private Expression<Func<Ismail_StudentMarks, bool>> BuildFilterExpression(PageRequest request)
        {
            var parameter = Expression.Parameter(typeof(Ismail_StudentMarks), "p");
            Expression baseFilter = Expression.Constant(true);


            var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
            var toStringMethod = typeof(object).GetMethod("ToString");
            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });

            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.ToLower();
                var searchTermConstant = Expression.Constant(searchTerm);


                var studentIdProperty = Expression.Property(parameter, nameof(Ismail_StudentMarks.StudentID));
                var idToStringCall = Expression.Call(studentIdProperty, toStringMethod);
                var idContains = Expression.Call(idToStringCall, containsMethod, searchTermConstant);

                var clientProperty = Expression.Property(parameter, nameof(Ismail_StudentMarks.Ismail_Clients));
                var userNameProperty = Expression.Property(clientProperty, nameof(Ismail_Clients.UserName));
                var userNameToLower = Expression.Call(userNameProperty, toLowerMethod);
                var userNameContains = Expression.Call(userNameToLower, containsMethod, searchTermConstant);
                var searchTremContains = Expression.OrElse(idContains, userNameContains);
                baseFilter = Expression.AndAlso(baseFilter, searchTremContains);
            }
            if (request.SearchTerm != null && request.Filters.Any())
            {
                foreach (var filter in request.Filters)
                {
                    if (filter.Value == null || string.IsNullOrEmpty(filter.Value.ToString()))
                        continue;

                    var propertyInfo = typeof(Ismail_StudentMarks).GetProperty(filter.Key);
                    if (propertyInfo == null) continue;

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
                        var convertValue = Convert.ChangeType(filter.Value, property.Type);
                        var filterValue = Expression.Constant(convertValue, property.Type);

                        filterExpression = Expression.Equal(property, filterValue);
                    }
                    baseFilter = Expression.AndAlso(baseFilter, filterExpression);
                }
            }
            return Expression.Lambda<Func<Ismail_StudentMarks, bool>>(baseFilter, parameter);
        }
    }

}




