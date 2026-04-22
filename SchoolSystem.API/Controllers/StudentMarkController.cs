using SchoolSystem.Application.Constans;
using SchoolSystem.Application.Models.DTOs;
using SchoolSystem.Application.Models.Request;
using SchoolSystem.Application.Services.IServices;
using System.Threading.Tasks;
using System.Web.Http;

namespace SchoolSystem.API.Controllers
{
    [RoutePrefix(ApiRoutePrefix.ApiRoute + ApiRoutePrefix.StudentMark)]
    public class StudentMarkController : ApiController
    {
        private readonly IStudentMarkService _studentMarkService;

        public StudentMarkController(IStudentMarkService studentMarkService)
        {
            _studentMarkService = studentMarkService;
        }

        [HttpPost]
        [Route("get-by")]
        public async Task<IHttpActionResult> Get([FromBody] StudentMarkModelDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _studentMarkService.GetByAsync(dto);

            if (!result.IsSuccess)
                return BadRequest(result.Message);
            return Ok(result);
        }
        [HttpPost, Route("add")]

        public async Task<IHttpActionResult> Add([FromBody] StudentMarkModel dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _studentMarkService.AddAsync(dto);

            if (!result.IsSuccess)
                return BadRequest(result.Message);
            return Ok(result);
        }
        [HttpPost, Route("delete/{id:int}")]

        public async Task<IHttpActionResult> Delete(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _studentMarkService.DeleteFirstOrDefaultAsync(id);

            if (!result.IsSuccess)
                return BadRequest(result.Message);
            return Ok(result);
        }

        [HttpPost, Route("delete-all")]
        public async Task<IHttpActionResult> DeleteRangeAsync()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _studentMarkService.DeleteRangeAsync();

            if (!result.IsSuccess)
                return BadRequest(result.Message);
            return Ok(result);
        }

        [HttpPut, Route("update")]
        public async Task<IHttpActionResult> UpdateAsync([FromBody] StudentMarkModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _studentMarkService.UpdateAsync(model);

            if (!result.IsSuccess)
                return BadRequest(result.Message);
            return Ok(result);
        }
        [HttpPut, Route("all")]
        public async Task<IHttpActionResult> GetAll([FromBody] PageRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _studentMarkService.GetPagedAsync(request);

            if (!result.IsSuccess)
                return BadRequest(result.Message);
            return Ok(result);
        }
    }
}