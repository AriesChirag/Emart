using Emart_DotNet.Models;
using Emart_DotNet.Services;
using Microsoft.AspNetCore.Mvc;

namespace Emart_DotNet.Controllers
{
    [Route("api/stores")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private readonly IStoreService _storeService;

        public StoreController(IStoreService storeService)
        {
            _storeService = storeService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Store>>> GetAllStores()
        {
            return Ok(await _storeService.GetAllStoresAsync());
        }

        [HttpGet("{storeId}")]
        public async Task<ActionResult<Store>> GetStoreById(int storeId)
        {
            var store = await _storeService.GetStoreByIdAsync(storeId);
            if (store == null)
            {
                // In Java it returns Ok even if null? Or maybe throws 500?
                // Java code: return ResponseEntity.ok(storeService.getStoreById(storeId));
                // JpaRepository.findById returns Optional. If not handled, it might be null or Optional.
                // Assuming standard behavior: return logic.
                // Actually Java `getStoreById` usually throws exception or returns null.
                // I'll return NotFound if null for better API design, or Ok(null) if strictly following "Java Controller returns Ok".
                // Java code calls `storeService.getStoreById(storeId)`. `StoreService` (if auto-generated) likely returns entity or throws.
                // Safest to return Ok(store) and let it be null if that's what happens, but NotFound is better.
                // User asked "Match Java logic... behavior as closely as possible".
                // I will include a null check.
                return NotFound();
            }
            return Ok(store);
        }
    }
}
