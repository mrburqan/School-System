using SchoolSystem.Application.Constans;
using SchoolSystem.Application.Models.Request;
using SchoolSystem.Application.Services.IServices;
using System.Threading.Tasks;
using System.Web.Http;

namespace SchoolSystem.API.Controllers
{
    [RoutePrefix(ApiRoutePrefix.ApiRoute + ApiRoutePrefix.Associations)]
    public class AssociationController : ApiController
    {
        private readonly IAssociationService _associationService;

        public AssociationController(IAssociationService associationService)
        {
            _associationService = associationService;
        }
        [HttpGet]
        [Route("all")]
        public async Task<IHttpActionResult> GetAll([FromBody] PageRequest pageRequest)
        {
            if (pageRequest == null) pageRequest = new PageRequest();

            var response = await _associationService.GetPagedAsync(pageRequest);

            if (response.IsSuccess)
            {
                return Ok(response);
            }

            return BadRequest(response.Message);
        }

        [HttpPut]
        [Route("update")]
        public async Task<IHttpActionResult> UpdateAssociation([FromBody] AssociationModel model)
        {
            var result = await _associationService.UpdateAssociationAsync(model);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result.Message);
        }
        [HttpPost]
        [Route("add")]
        public async Task<IHttpActionResult> AddAssociations([FromBody] AssociationModel model)
        {
            var result = await _associationService.AddAssociationAsync(model);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result.Message);
        }
        [HttpDelete]
        [Route("delete/{clientId:int}/{classId:int}")]
        public async Task<IHttpActionResult> DeleteAssociations(int clientId, int classId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _associationService.DeleteAssociationAsync(clientId, classId);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result.Message);
        }


    }
}