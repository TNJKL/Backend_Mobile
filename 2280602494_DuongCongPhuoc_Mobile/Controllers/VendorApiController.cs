using Microsoft.AspNetCore.Mvc;
using _2280602494_DuongCongPhuoc_Mobile.Models;
using _2280602494_DuongCongPhuoc_Mobile.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace _2280602494_DuongCongPhuoc_Mobile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VendorApiController : ControllerBase
    {
        private readonly IVendorRepository _vendorRepository;

        public VendorApiController(IVendorRepository vendorRepository)
        {
            _vendorRepository = vendorRepository;
        }

        // GET: api/VendorApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Vendor>>> GetVendors()
        {
            return Ok(await _vendorRepository.GetAllVendorsAsync());
        }

        // GET: api/VendorApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Vendor>> GetVendor(int id)
        {
            var vendor = await _vendorRepository.GetVendorByIdAsync(id);
            if (vendor == null) return NotFound();
            return Ok(vendor);
        }

        // POST: api/VendorApi
        [HttpPost]
        public async Task<ActionResult<Vendor>> CreateVendor(Vendor vendor)
        {
            // Simple validation
            if (string.IsNullOrEmpty(vendor.Name) || string.IsNullOrEmpty(vendor.VendorType))
            {
                return BadRequest("Name and VendorType are required");
            }
            
            await _vendorRepository.AddVendorAsync(vendor);
            return CreatedAtAction(nameof(GetVendor), new { id = vendor.Id }, vendor);
        }

        // PUT: api/VendorApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVendor(int id, Vendor vendor)
        {
            if (id != vendor.Id) return BadRequest();
            await _vendorRepository.UpdateVendorAsync(vendor);
            return NoContent();
        }

        // DELETE: api/VendorApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVendor(int id)
        {
            await _vendorRepository.DeleteVendorAsync(id);
            return NoContent();
        }
    }
}
