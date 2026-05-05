using Emart_DotNet.Models;          // contains Address class (data model)
using Emart_DotNet.Services;        // contains IAddressService (business logic interface)
using Microsoft.AspNetCore.Mvc;     // provides API features like ControllerBase, Ok(), routing
using System.Collections.Generic;   // allows use of List<T>
using System.Threading.Tasks;       // required for async and await

namespace Emart_DotNet.Controllers  // logical grouping of controller classes
{
    [ApiController]
    // tells ASP.NET Core:
    // - this class handles HTTP requests (API)
    // - enables automatic model validation
    // - automatically binds request data (no need for manual parsing)

    [Route("api/address")]
    // base URL for all methods in this controller
    // final URL will be: /api/address/....

    public class AddressController : ControllerBase
    // ControllerBase gives built-in methods like:
    // Ok(), BadRequest(), NotFound(), etc.
    {
        private readonly IAddressService _addressService;
        // interface reference (used to call service layer)
        // readonly = value cannot be changed after assignment

        public AddressController(IAddressService addressService)
        // constructor runs when controller is created
        // ASP.NET automatically injects (gives) the service object
        {
            _addressService = addressService;
            // store service object so we can use it in methods
        }

        // ===================== GET API =====================

        [HttpGet("user/{userId}")]
        // handles GET request
        // {userId} is a route parameter (comes from URL)
        // example: GET /api/address/user/5

        public async Task<ActionResult<List<Address>>> GetUserAddresses(int userId)
        // async = method runs asynchronously (non-blocking)
        //What async/await does:
        //Makes API faster
        //Doesn’t block server while waiting for DB
        // Task<> = async return type
        // ActionResult = HTTP response (status + data)
        // List<Address> = list of address objects
        {
            return Ok(await _addressService.GetAddressesByCustomerAsync(userId));
            // call service method to fetch data
            // await = wait for async result
            // Ok() = return HTTP 200 response with data
        }

        // ===================== POST API =====================

        [HttpPost("add/{userId}")]
        // handles POST request
        // example: POST /api/address/add/5

        public async Task<ActionResult<Address>> AddAddress(
            int userId,                      // comes from URL
            [FromBody] Address address       // comes from request body (JSON)
        )
        {
            return Ok(await _addressService.AddAddressAsync(userId, address));
            // send data to service to save in database
            // return saved address with HTTP 200
        }

        // ===================== DELETE API =====================

        [HttpDelete("{addressId}")]
        // handles DELETE request
        // example: DELETE /api/address/10

        public async Task<ActionResult> DeleteAddress(int addressId)
        // no data returned, only status/message
        {
            await _addressService.DeleteAddressAsync(addressId);
            // call service to delete address from database

            return Ok("Address deleted successfully");
            // return success message with HTTP 200
        }
    }
}