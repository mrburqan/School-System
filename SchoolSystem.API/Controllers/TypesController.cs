using SchoolSystem.Application.Constans;
using SchoolSystem.Application.Models.DTOs;
using SchoolSystem.Application.Models.Request;
using SchoolSystem.Application.Services.IServices;
using System.Threading.Tasks;
using System.Web.Http;

namespace SchoolSystem.API.Controllers
{
    [RoutePrefix(ApiRoutePrefix.ApiRoute + ApiRoutePrefix.Types)]
    public class TypesController : ApiController
    {
        private readonly ITypeService _typeService;

        public TypesController(ITypeService typeService)
        {
            _typeService = typeService;
        }

        [HttpPost]
        [Route("add")]
        public async Task<IHttpActionResult> Add([FromBody] TypeModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (model == null)
                return BadRequest("Request body is required.");

            var result = await _typeService.AddAsync(model);

            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpPost]
        [Route("get-by")]
        public async Task<IHttpActionResult> GetBy([FromBody] TypeModelDto model)
        {
            if (model == null)
                return BadRequest("Request body is required.");

            var result = await _typeService.GetFirstOrDefaultAsync(model);

            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpPost]
        [Route("all")]
        public async Task<IHttpActionResult> All([FromBody] PageRequest request)
        {
            if (request == null)
                return BadRequest("Request body is required.");

            var result = await _typeService.GetPageAsync(request);

            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(result);
        }


    }
}