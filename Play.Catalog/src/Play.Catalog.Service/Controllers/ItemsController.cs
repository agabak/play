using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Entities;
using Play.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Play.Catalog.Contracts;

namespace Play.Catalog.Service.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<Item> _itemRepo;
        private readonly IPublishEndpoint _publishEndpoint;

        public ItemsController(
            IRepository<Item> itemsRepository,
            IPublishEndpoint publishEndpoint)
        {
            _itemRepo = itemsRepository;
            _publishEndpoint = publishEndpoint;
        }

        [HttpGet]
        public async Task<IEnumerable<ItemDto>> Get()
        {
            return (await _itemRepo.GetAllAsync())
                    .Select(item => item.AsDtos());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetById(Guid id)
        {
            var item = (await _itemRepo.GetAsync(id)).AsDtos();
            return item is not null ? item : NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<ItemDto>> Post(CreateItemDto createItem)
        {
            Item item = new()
            {
                Name = createItem.Name,
                Description = createItem.Description,
                Price = createItem.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await _itemRepo.CreateAsync(item);
            await _publishEndpoint.Publish(new CatalogItemCreate(item.Id,item.Name,item.Description));

            // get this from ActionResult
            return CreatedAtAction(nameof(GetById), new { Id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(Guid id, UpdateItemDto updateItem)
        {
            var item = await _itemRepo.GetAsync(id);
            if (item == null) return BadRequest();

            item.Name = updateItem.Name ?? item.Name;
            item.Description = updateItem.Description ?? item.Description;
            item.Price = updateItem.Price > 0 ? updateItem.Price : item.Price;
            await _itemRepo.UpdateAsync(item);
            await _publishEndpoint.Publish(new CatalogItemUpdate(item.Id, item.Name, item.Description));
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var item = await _itemRepo.GetAsync(id);
            if (item is null) return BadRequest();
            await _itemRepo.RemoveAsync(item.Id);
            await _publishEndpoint.Publish(new CatalogItemDeleted(id));
            return NoContent();
        }
    }
}