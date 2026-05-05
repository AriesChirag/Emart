using Emart_DotNet.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Emart_DotNet.Controllers
{
    [ApiController]
    [Route("api/epoints")]
    public class EPointsController : ControllerBase
    {
        private readonly IEPointsService _epointsService;

        public EPointsController(IEPointsService epointsService)
        {
            _epointsService = epointsService;
        }

        [HttpPost("credit/{userId}/{epoints}")]
        public async Task<ActionResult<int>> Credit(int userId, int epoints)
        {
            return Ok(await _epointsService.CreditPointsAsync(userId, epoints));
        }

        [HttpPost("redeem/{userId}/{epoints}")]
        public async Task<ActionResult<int>> Redeem(int userId, int epoints)
        {
            try
            {
                return Ok(await _epointsService.RedeemPointsAsync(userId, epoints));
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("balance/{userId}")]
        public async Task<ActionResult<int>> Balance(int userId)
        {
            return Ok(await _epointsService.GetBalanceAsync(userId));
        }
    }
}
