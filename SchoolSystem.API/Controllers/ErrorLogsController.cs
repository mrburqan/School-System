using SchoolSystem.Application.Constans;
using SchoolSystem.Application.Models.Request;
using SchoolSystem.Application.Services.IServices;
using System.Threading.Tasks;
using System.Web.Http;

namespace SchoolSystem.API.Controllers
{
    [RoutePrefix(ApiRoutePrefix.ApiRoute + ApiRoutePrefix.ErrorLogs)]
    public class ErrorLogsController : ApiController
    {
        private readonly IErrorLogsService _errorLogsService;

        public ErrorLogsController(IErrorLogsService errorLogsService)
        {
            _errorLogsService = errorLogsService;
        }

        [HttpPost]
        [Route("GetPaged")]
        public async Task<IHttpActionResult> GetPaged([FromBody] PageRequest request)
        {
            if (request == null)
                return BadRequest("Request body is required.");

            var result = await _errorLogsService.GetPagedAsync(request);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpDelete]
        [Route("Resolve/{id:int}")]
        public async Task<IHttpActionResult> Resolve(int id)
        {
            if (id <= 0)
                return BadRequest("Valid error log id is required.");

            var result = await _errorLogsService.ResolveAndDeleteAsync(id);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpDelete]
        [Route("ClearAll")]
        public async Task<IHttpActionResult> ClearAll()
        {
            var result = await _errorLogsService.ClearAllLogsAsync();
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

    }
}