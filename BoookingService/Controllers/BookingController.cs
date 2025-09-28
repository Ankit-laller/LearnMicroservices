using BoookingService.Dtos;
using BoookingService.Utils;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BoookingService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        // GET: api/<BookingController>
        [HttpPost]
        public async Task<ActionResult> Get(UserDto request)
        {

           // await RabbitMqHelper.SendMessageAsync(user);


            // Consumer (runs until cancellation)
            //using var cts = new CancellationTokenSource();
            //var consumerTask = RabbitMqHelper.ReceiveMessagesAsync(cts.Token);

            //// Stop after 10 sec
            //await Task.Delay(10000);
            //cts.Cancel();
            //await consumerTask;
            var creditResponse = await RabbitMqHelper
            .SendRequestAsync<CreditRequestDto, CreditResponseDto>(
            requestQueue: "credit-requests",
            request: new CreditRequestDto { UserId = request.UserId, Amount = request.Amount },
            timeout: TimeSpan.FromSeconds(10));

            if (!creditResponse.Success)
                return Ok(creditResponse);

            return Ok(new { Message = "Booking confirmed", creditResponse.AvailibleBalance });

        }

        // GET api/<BookingController>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/<BookingController>
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT api/<BookingController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<BookingController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
