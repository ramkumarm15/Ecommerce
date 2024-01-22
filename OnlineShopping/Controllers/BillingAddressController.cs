using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShopping.Models;
using OnlineShopping.Models.DTO;
using OnlineShopping.Repository;

namespace OnlineShopping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "User_Admin")]
    public class BillingAddressController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IBillingAddressRepository _repository;
        private BillingAddressResponse response;

        public BillingAddressController(ApplicationDbContext context, IBillingAddressRepository repository)
        {
            _context = context;
            _repository = repository;
            response = new BillingAddressResponse();
        }

        /// <summary>
        /// Get all Billing Address of user
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetBillingAddresses()
        {
            int userId = Convert.ToInt32(User.FindFirstValue("id"));

            var result = await _repository.GetBillingAddresses(userId);

            if (result.Count != 0)
            {
                return Ok(result);
            }
            response.Message = "No address found";
            return BadRequest(response);
        }

        /// <summary>
        /// Get Specific Billing Address of user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetBillingAddress([FromRoute] int id)
        {
            int userId = Convert.ToInt32(User.FindFirstValue("id"));

            var result = await _repository.GetBillingAddress(userId, id);
            if (result != null)
            {
                return Ok(result);
            }
            response.Message = "No address found";
            return BadRequest(response);
        }


        /// <summary>
        /// Update Specific Billing address of user
        /// </summary>
        /// <param name="addressId"></param>
        /// <param name="billingAddress"></param>
        /// <returns></returns>
        [HttpPut("{id:int}")]
        public async Task<ActionResult> PutBillingAddress([FromRoute] int addressId,
            [FromBody] BillingAddressDto billingAddress)
        {
            int userId = Convert.ToInt32(User.FindFirstValue("id"));

            var result = await _repository.UpdateBillingAddress(userId, addressId, billingAddress);

            if ((bool)result)
            {
                response.Message = "Address updated";
                return Ok(response);
            }

            response.Message = "No address found";
            return BadRequest(response);
        }

        /// <summary>
        /// Create a new Billing Address for user
        /// </summary>
        /// <param name="billingAddress"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> PostBillingAddress([FromBody] BillingAddressDto billingAddress)
        {
            if (billingAddress == null)
            {
                response.Message = "Data empty";
                return BadRequest(response);
            }
            int userId = Convert.ToInt32(User.FindFirstValue("id"));

            User? userToAddBillingAddress = await _context.Users.FindAsync(userId);

            var result = await _repository.CreateBillingAddress(userToAddBillingAddress, billingAddress);

            response.Message = "Address added";
            return Ok(response);
        }


        /// <summary>
        /// Delete Billing address of user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteBillingAddress([FromRoute] int id)
        {
            int userId = Convert.ToInt32(User.FindFirstValue("id"));

            if (BillingAddressExists(id, userId))
            {
                BillingAddress? billingAddressToBeDeleted = await _context.BillingAddresses
                    .Where(w => w.Id == id && w.User.Id == userId).FirstOrDefaultAsync();


                _context.BillingAddresses.Remove(billingAddressToBeDeleted);
                await _context.SaveChangesAsync();

                response.Message = "Address deleted";
                return Ok(response);
            }

            response.Message = "Address not found";
            return BadRequest(response);
        }

        [HttpPost]
        [Route("address/set-default")]
        public async Task<ActionResult> SetDefaultAddress([FromBody] DefaultAddressDto addressDto)
        {
            int userId = Convert.ToInt32(User.FindFirstValue("id"));

            if (BillingAddressExists(addressDto.AddressId, userId))
            {
                BillingAddress? AddressToSetDefault = await _context.BillingAddresses
                    .Where(w => w.Id == addressDto.AddressId && w.User.Id == userId).FirstOrDefaultAsync();

                BillingAddress? AddressToRemoveDefault = await _context.BillingAddresses
                    .Where(w => w.Default && w.User.Id == userId).FirstOrDefaultAsync();

                AddressToRemoveDefault.Default = false;
                AddressToSetDefault.Default = true;

                _context.BillingAddresses.Update(AddressToSetDefault);
                _context.BillingAddresses.Update(AddressToRemoveDefault);
                await _context.SaveChangesAsync();

                response.Message = "Default address changed";
                return Ok(response);
            }

            response.Message = "Address not found";
            return BadRequest(response);
        }

        private bool BillingAddressExists(int id, int userId)
        {
            return _context.BillingAddresses.Any(e => e.Id == id && e.User.Id == userId);
        }
    }
}
