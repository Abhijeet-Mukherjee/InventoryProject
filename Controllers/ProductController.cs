﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspnetIdentityRoleBasedTutorial.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
      
        public IActionResult GetAllProduct()
        {
            return View();
        }
    }
}