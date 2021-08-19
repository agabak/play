using System;

namespace Play.Inventory.Service.Dtos
{
    public record GrandItemsDto(Guid Userid, Guid CatalogItemId, int Quantity);
    public record InventoryItemDto(Guid CatalogItemId, int Quantity, DateTimeOffset AcquiredDate);
}
