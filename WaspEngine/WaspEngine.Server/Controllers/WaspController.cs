using Application.Abstracts;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace WaspEngine.Server.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class WaspController : ControllerBase
    {
        private readonly IWaspService _waspService;

        public WaspController(IWaspService waspService)
        {
            _waspService = waspService;
        }

        [HttpGet(Name = "GetWasps")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Wasp>>> GetWasps()
        {
            var wasps = await _waspService.GetWaspsAsync();

            return Ok(wasps);
        }

        [HttpGet("{id:long}", Name = "GetWaspById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Wasp>> GetWaspById(long id)
        {
            var wasp = await _waspService.GetWaspAsync(id);

            return wasp is null ? NotFound() : Ok(wasp);
        }

        [HttpPost("{id:long}/zap", Name = "ZapWasp")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Wasp>> ZapWasp(long id)
        {
            var wasp = await _waspService.ZapWaspAsync(id);

            return wasp is null ? NotFound() : Ok(wasp);
        }

        [HttpPost("{id:long}/recover", Name = "RecoverWasp")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Wasp>> RecoverWasp(long id)
        {
            var wasp = await _waspService.RecoverWaspAsync(id);

            return wasp is null ? NotFound() : Ok(wasp);
        }

        [HttpPost("recover", Name = "RecoverKnockedWasps")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> RecoverKnockedWasps()
        {
            var recovered = await _waspService.RecoverKnockedWaspsAsync();

            return Ok(recovered);
        }
    }
}
