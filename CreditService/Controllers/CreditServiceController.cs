using CreditService.Dtos;
using CreditService.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CreditService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreditServiceController : ControllerBase
    {
        private readonly ICreditAppService _creditAppService;

        public CreditServiceController(ICreditAppService creditAppService)
        {
            _creditAppService = creditAppService;
        }
        [HttpGet]
        // GET: api/<CreditServiceController>
        [HttpGet("GetCreditLimit{id}")]
        public async Task<IActionResult> Get(int id)
        {
            return await _creditAppService.GetCreditLimitAsync(id) != null ? 
                Ok(await _creditAppService.GetCreditLimitAsync(id)) : NotFound();
        }


        // POST api/<CreditServiceController>
        [HttpPost("AddCredits")]
        public async Task<IActionResult> Post([FromBody] AddOrUpdateCreditLimitDto value)
        {
            var response = await _creditAppService.AddCreditLimitAsync(value);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        // PUT api/<CreditServiceController>/5
        [HttpPut("UpdateCreditLimit")]
        public async Task<IActionResult> Put(AddOrUpdateCreditLimitDto value)
        {
            var response = await _creditAppService.UpdateCreditLimitAsync(value);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

    }
}
