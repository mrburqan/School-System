using SchoolSystem.Application.Constans;
using SchoolSystem.Application.Models.DTOs;
using SchoolSystem.Application.Models.Request;
using SchoolSystem.Application.Services.IServices;
using System.Threading.Tasks;
using System.Web.Http;

namespace SchoolSystem.API.Controllers
{
    [RoutePrefix(ApiRoutePrefix.ApiRoute + ApiRoutePrefix.Genders)]
    public class GenderController : ApiController
    {
        private readonly IGenderService _genderService;

        public GenderController(IGenderService genderService)
        {
            _genderService = genderService;
        }

        [HttpPost]
        [Route("add")]
        public async Task<IHttpActionResult> Add([FromBody] GenderModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (model == null)
                return BadRequest("Request body is required.");

            var result = await _genderService.AddAsync(model);

            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpPost]
        [Route("get-by")]
        public async Task<IHttpActionResult> GetBy([FromBody] GenderModelDto model)
        {
            if (model == null)
                return BadRequest("Request body is required.");

            var result = await _genderService.GetFirstOrDefaultAsync(model);

            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(result);
        }

    }
}