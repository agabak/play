using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Play.Catalog.Service.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private static readonly List<ItemDto> items = new()
        {
            new ItemDto(Guid.NewGuid(),
                "Portion", "Restores a small amount of HP", 5, DateTimeOffset.UtcNow),
            new ItemDto(Guid.NewGuid(),
                "Antidote", "Cures poison", 7, DateTimeOffset.UtcNow),
            new ItemDto(Guid.NewGuid(),
                "Bronze  sword", "Deal as small amount of damage", 25, DateTimeOffset.UtcNow)
        };

        [HttpGet]
        public IEnumerable<ItemDto> Get()
        {
            return items;
        }

        [HttpGet("{id}")]
        public ActionResult<ItemDto> GetById(Guid id)
        {
            var item = items.SingleOrDefault(x => x.Id == id);
            return item is not null ? item : NotFound();
        }

        [HttpPost]
        public ActionResult<ItemDto> Post(CreateItemDto createItem)
        {
            // create a item in the constructor , benefit of using record inserd of class
            ItemDto item =
            new(Guid.NewGuid(), createItem.Name, createItem.Description,
            createItem.Price, DateTimeOffset.UtcNow);
            items.Add(item);

            // get this from ActionResult
            return CreatedAtAction(nameof(GetById), new { Id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public ActionResult Put(Guid id, UpdateItemDto updateItem)
        {
            var item = items.SingleOrDefault(x => x.Id == id);
            if (item == null) return BadRequest();
            var itemToUpdate = item with
            {
                Name = updateItem.Name,
                Description = updateItem.Description,
                Price = updateItem.Price
            };

            var index = items.FindIndex(it => item.Id == id);
            items[index] = itemToUpdate;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var item = items.SingleOrDefault(x => x.Id == id);
            if (item == null) return BadRequest();
            items.Remove(item);
            return NoContent();
        }
    }
}
