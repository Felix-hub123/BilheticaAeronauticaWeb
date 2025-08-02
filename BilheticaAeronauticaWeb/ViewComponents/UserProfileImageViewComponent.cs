using BilheticaAeronauticaWeb.Data.Entities;
using BilheticaAeronauticaWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.ViewComponents
{
    public class UserProfileImageViewComponent : ViewComponent
    {
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
       

        public UserProfileImageViewComponent(
            UserManager<User> userManager,
             IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IViewComponentResult> InvokeAsync(string displayMode = null)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Content(""); 
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Content("");
            }

            var model = new UserProfileImageViewModel
            {
                IsAuthenticated = true,
                DisplayMode = displayMode,
                FullName = user.Nome,  
                ImageFullPath = user.ImageId == Guid.Empty
                ? "/images/users/noimage.png"
                : $"https://bilhetica.blob.core.windows.net/users/{user.ImageId}"
            };

        

            return View(model);
        }
    }
}


