﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Play.Catalog.Service
{
    public record 
   ItemDto(Guid Id, string Name, string Description, decimal Price, DateTimeOffset CreateDate);

   public record CreateItemDto([Required]string Name, string Description, decimal Price);

   public record UpdateItemDto([Required]string Name, string Description,decimal Price);
}
