using System;

namespace Play.Catalog.Contracts
{
   public record CatalogItemCreate(Guid itemId, string Name, string description);
   public record CatalogItemUpdate(Guid itemId, string Name, string description);
   public record CatalogItemDeleted(Guid itemId); 
}
