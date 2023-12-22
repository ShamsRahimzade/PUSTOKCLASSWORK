﻿using Microsoft.AspNetCore.Identity;

namespace PustokSH.Model
{
    public class AppUser:IdentityUser
    {
        public string Name { get; set; }
        public string SurName { get; set; }
        public List<BasketItem> BasketItems { get; set; }
     
    }
}
