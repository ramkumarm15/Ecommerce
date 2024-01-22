using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShopping.Models;
using OnlineShopping.Models.DTO;

namespace OnlineShopping.Repository
{
    public interface IBillingAddressRepository
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<BillingAddress>> GetBillingAddresses(int userId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="addressId"></param>
        /// <returns></returns>
        Task<BillingAddress> GetBillingAddress(int userId, int addressId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="billingAddress"></param>
        /// <returns></returns>
        Task<bool> CreateBillingAddress(User user, BillingAddressDto billingAddress);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="addressId"></param>
        /// <param name="billingAddress"></param>
        /// <returns></returns>
        Task<bool?> UpdateBillingAddress(int userId, int addressId, BillingAddressDto billingAddress);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="addressId"></param>
        /// <returns></returns>
        Task<bool?> RemoveDefaultAddress(int userId, int addressId);
    }
    public class BillingAddressRepository : IBillingAddressRepository
    {
        private readonly ApplicationDbContext _context;

        public BillingAddressRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all address of user as list
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<BillingAddress>> GetBillingAddresses(int userId)
        {
            if (await _context.BillingAddresses.AnyAsync(b => b.UserId == userId))
            {
                List<BillingAddress> billingAddress = await _context.BillingAddresses
                    .Where(x => x.User.Id == userId)
                    .ToListAsync();

                return billingAddress;
            }

            return new List<BillingAddress>();
        }

        /// <summary>
        /// Get individual address of user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="addressId"></param>
        /// <returns></returns>
        public async Task<BillingAddress> GetBillingAddress(int userId, int addressId)
        {
            if (await _context.BillingAddresses.AnyAsync(b => b.UserId == userId && b.Id == addressId))
            {
                BillingAddress billingAddress = await _context.BillingAddresses
                    .Where(x => x.Id == addressId && x.User.Id == userId)
                    .FirstOrDefaultAsync();

                return billingAddress;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="billingAddress"></param>
        /// <returns></returns>
        public async Task<bool> CreateBillingAddress(User user, BillingAddressDto billingAddress)
        {
            if (billingAddress.defaultAddress)
            {
                var addressToRemoveDefault = await GetDefaultAddress(user.Id);

                if (addressToRemoveDefault != null)
                {
                    addressToRemoveDefault.Default = false;

                    _context.BillingAddresses.Update(addressToRemoveDefault);
                }
            };

            BillingAddress billingAddressToBeAdded = new BillingAddress
            {
                BillingName = billingAddress.BillingName,
                Address1 = billingAddress.Address1,
                Address2 = billingAddress.Address2,
                City = billingAddress.City,
                State = billingAddress.State,
                PostalCode = billingAddress.PostalCode,
                MobileNumber = billingAddress.MobileNumber,
                Default = billingAddress.defaultAddress,
                User = user
            };

            await _context.BillingAddresses.AddAsync(billingAddressToBeAdded);
            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// update the individual address of user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="addressId"></param>
        /// <param name="billingAddress"></param>
        /// <returns></returns>
        public async Task<bool?> UpdateBillingAddress(int userId, int addressId, BillingAddressDto billingAddress)
        {
            if (await _context.BillingAddresses.AnyAsync(b => b.UserId == userId && b.Id == addressId))
            {
                BillingAddress? billingAddressToBeUpdated = await GetBillingAddress(userId, addressId);

                if (billingAddress.defaultAddress)
                {
                    var addressToRemoveDefault = await GetDefaultAddress(userId);

                    if (addressToRemoveDefault != null)
                    {
                        addressToRemoveDefault.Default = false;

                        _context.BillingAddresses.Update(addressToRemoveDefault);
                    }
                }

                billingAddressToBeUpdated.BillingName = billingAddress.BillingName;
                billingAddressToBeUpdated.Address1 = billingAddress.Address1;
                billingAddressToBeUpdated.Address2 = billingAddress.Address2;
                billingAddressToBeUpdated.MobileNumber = billingAddress.MobileNumber;
                billingAddressToBeUpdated.City = billingAddress.City;
                billingAddressToBeUpdated.State = billingAddress.State;
                billingAddressToBeUpdated.PostalCode = billingAddress.PostalCode;
                billingAddressToBeUpdated.Default = billingAddress.defaultAddress;

                _context.BillingAddresses.Update(billingAddressToBeUpdated);
                return await _context.SaveChangesAsync() > 0;
            }

            return null;
        }

        /// <summary>
        /// Remove default address
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="addressId"></param>
        /// <returns></returns>
        public async Task<bool?> RemoveDefaultAddress(int userId, int addressId)
        {
            if (await _context.BillingAddresses.AnyAsync(b => b.UserId == userId && b.Id == addressId))
            {
                BillingAddress billingAddress = await GetBillingAddress(userId, addressId);

                billingAddress.Default = false;

                _context.BillingAddresses.Update(billingAddress);
                var result = await _context.SaveChangesAsync();

                return result > 0;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<BillingAddress> GetDefaultAddress(int userId)
        {
            if (await _context.BillingAddresses.AnyAsync(b => b.UserId == userId))
            {
                BillingAddress billingAddress = await _context.BillingAddresses
                    .Where(x => x.User.Id == userId && x.Default == true)
                    .FirstOrDefaultAsync();

                return billingAddress;
            }

            return null;
        }
    }
}
