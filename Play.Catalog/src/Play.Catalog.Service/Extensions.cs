using Play.Catalog.Service.Entities;

namespace Play.Catalog.Service
{
    public static class Extensions
    {
        public static ItemDto AsDtos(this Item item)
        {
            return new
            ItemDto(item.Id, item.Name, item.Description, item.Price, item.CreatedDate);
        }
    }
}
