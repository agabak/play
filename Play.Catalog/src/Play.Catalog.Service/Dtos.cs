using System;
using System.ComponentModel.DataAnnotations;

namespace Play.Catalog.Service
{
    public record 
   ItemDto(Guid Id, string Name, string Description, decimal Price, DateTimeOffset CreateDate);

   public record CreateItemDto(
        [Required][StringLength(100, ErrorMessage = "{0} should be greater than {1}")] string Name, 
       string Description,
       [Range(typeof(decimal),"0","1000")] decimal Price);

   public record UpdateItemDto(
       [Required][StringLength(100, ErrorMessage = "{0} should be greater than {1}")]string Name, 
       string Description, 
       [Range(typeof(decimal), "0", "1000")]decimal Price);
}
