using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using ModilistPortal.API.Configuration;
using ModilistPortal.Business.Utils.AddressDomain;
using ModilistPortal.Business.Utils.AddressDomain.Models;

namespace ModilistPortal.API.Area.API.Controllers
{
    public class AddressController : APIBaseController
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        [Authorize(nameof(AuthorizationPermissions.Addresses))]
        [HttpGet("GetCities")]
        [ProducesResponseType(typeof(IEnumerable<City>), 200)]
        public IActionResult GetCities()
        {
            return Ok(_addressService.GetCities());
        }

        [Authorize(nameof(AuthorizationPermissions.Addresses))]
        [HttpGet("GetDistricts/{cityCode}")]
        [ProducesResponseType(typeof(IEnumerable<District>), 200)]
        public IActionResult GetDistricts(string cityCode)
        {
            return Ok(_addressService.GetDistricts(cityCode));
        }
    }
}
