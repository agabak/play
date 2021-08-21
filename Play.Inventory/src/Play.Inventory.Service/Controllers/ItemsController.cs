using Microsoft.AspNetCore.Mvc;
using Play.Common;
using Play.Inventory.Service.Clients;
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
        private readonly CatalogClient _catalogClient;

        public ItemsController(
             IRepository<InventoryItem> repository,
             CatalogClient catalogClient)
        {
            _repository = repository;
            _catalogClient = catalogClient;
        }

        [HttpGet("{userid}")]
        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userid)
        {
            if (userid == Guid.Empty) return BadRequest();

            var catalogItems = await _catalogClient.GetCatalogItemsAsync();
            var inventoryItemEntites = (await _repository.GetAllAsync(item => item.UserId == userid)).ToList();

            var inventoryItemDtos =  inventoryItemEntites.Select(inventoryItem =>
                {
                   var catalogItem = 
                   catalogItems.Single(catalogItem => catalogItem.Id == inventoryItem.CatalogItemId);
                   return inventoryItem.AsDto(catalogItem.Name, catalogItem.Description);
                 });
            return Ok(inventoryItemDtos);
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
