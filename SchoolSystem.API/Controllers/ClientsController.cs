using SchoolSystem.Application.Constans;
using SchoolSystem.Application.Models.DTOs;
using SchoolSystem.Application.Models.Request;
using SchoolSystem.Application.Services.IServices;
using System.Threading.Tasks;
using System.Web.Http;

namespace SchoolSystem.API.Controllers
{
    [RoutePrefix(ApiRoutePrefix.ApiRoute + ApiRoutePrefix.Clients)]
    public class ClientsController : ApiController
    {
        private readonly IClientService _clientService;

        public ClientsController(IClientService clientService)
        {
            this._clientService = clientService;
        }

        [HttpPost]
        [Route("add")]
        public async Task<IHttpActionResult> AddAsync([FromBody] ClientModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (model == null)
                return BadRequest("Request body is required.");

            var result = await _clientService.AddAsync(model);

            if (!result.IsSuccess)
                return Content(System.Net.HttpStatusCode.BadRequest, result);

            return Ok(result);
        }

        [HttpPatch]
        [Route("update")]
        public async Task<IHttpActionResult> UpdateAsync([FromBody] ClientModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (model == null)
                return BadRequest("Request body is required.");

            var result = await _clientService.UpdateAsync(model);

            if (!result.IsSuccess) return Content(System.Net.HttpStatusCode.BadRequest, result);

            return Ok(result);
        }

        [HttpDelete]
        [Route("delete/{id:int}")]
        public async Task<IHttpActionResult> DeleteAsync(int id)
        {
            if (id <= 0)
                return BadRequest("Valid client id is required.");

            var result = await _clientService.DeleteFirstOrDefaultAsync(id);

            if (!result.IsSuccess) return Content(System.Net.HttpStatusCode.BadRequest, result);

            return Ok(result);
        }

        [HttpPost]
        [Route("get-by")]
        public async Task<IHttpActionResult> GetByAsync([FromBody] ClientModelDto model)
        {
            if (model == null)
                return BadRequest("Request body is required.");

            var result = await _clientService.GetByFirstOrDefult(model);

            if (!result.IsSuccess) return Content(System.Net.HttpStatusCode.NotFound, result);

            return Ok(result);
        }

        [HttpPost]
        [Route("all")]
        public async Task<IHttpActionResult> GetAll([FromBody] PageRequest request)
        {
            if (request == null)
                return BadRequest("Request body is required.");

            var result = await _clientService.GetPagedAsync(request);

            return Ok(result);
        }
    }
}