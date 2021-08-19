using Microsoft.AspNetCore.Mvc;
using Play.Common;
using Play.Inventory.Service.Dtos;
using Play.Inventory.Service.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Play.Inventory.Service.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<InventoryItem> _repository;

        public ItemsController(
             IRepository<InventoryItem> repository)
        {
            _repository = repository;
        }

        [HttpGet("{userid}")]
        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userid)
        {
            if (userid == Guid.Empty) return BadRequest();
            var itemDto = (await _repository.GetAllAsync(item => item.UserId == userid))
                         .Select(item => item.AsDto());
            return Ok(itemDto);
        }

        [HttpPost]
        public async Task<ActionResult> PostAsync(GrandItemsDto grandItemsDto)
        {
            var inventoryItem = await _repository
            .GetAsync(item => item.UserId == grandItemsDto.Userid && item.CatalogItemId == grandItemsDto.CatalogItemId);

            if(inventoryItem is null)
            {
                inventoryItem = new InventoryItem
                {
                    CatalogItemId = grandItemsDto.CatalogItemId,
                    UserId = grandItemsDto.Userid,
                    Quantity = grandItemsDto.Quantity,
                    AcquiredDate = DateTimeOffset.UtcNow
                };

                await _repository.CreateAsync(inventoryItem);    
            }
            else
            {
                inventoryItem.Quantity += grandItemsDto.Quantity;
                await _repository.UpdateAsync(inventoryItem);
            }

            return Ok(inventoryItem);
        }
    }
}
