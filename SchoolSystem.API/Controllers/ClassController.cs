using SchoolSystem.Application.Constans;
using SchoolSystem.Application.Models.DTOs;
using SchoolSystem.Application.Models.Request;
using SchoolSystem.Application.Services.IServices;
using System.Threading.Tasks;
using System.Web.Http;

namespace SchoolSystem.API.Controllers
{
    [RoutePrefix(ApiRoutePrefix.ApiRoute + ApiRoutePrefix.Classes)]
    public class ClassController : ApiController
    {
        private readonly IClassService _classService;

        public ClassController(IClassService classService)
        {
            _classService = classService;
        }

        [HttpPost]
        [Route("add")]
        public async Task<IHttpActionResult> Add([FromBody] ClassModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _classService.AddAsync(model);

            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpPost]
        [Route("get-by")]
        public async Task<IHttpActionResult> GetBy([FromBody] ClassModelDto modelDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _classService.GetFirstOrDefaulteAsync(modelDto);

            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpDelete]
        [Route("delete/{id:int}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            if (id < 0)
                return BadRequest("Invalid ID ");

            var result = await _classService.DeleteFirstOrDefaultAsync(id);

            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpPatch]
        [Route("update")]
        public async Task<IHttpActionResult> Update([FromBody] ClassModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (model == null)
                return BadRequest("Request body is required.");

            var result = await _classService.UpdateAsync(model);

            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpPost]
        [Route("all")]
        public async Task<IHttpActionResult> GetAll([FromBody] PageRequest request)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _classService.GetPagedAsync(request);
            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(result);
        }
    }
}