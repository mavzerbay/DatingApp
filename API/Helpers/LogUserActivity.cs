using System;
using System.Threading.Tasks;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
namespace API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
           var resultContenxt = await next();

           if(!resultContenxt.HttpContext.User.Identity.IsAuthenticated) return;

           var userId = resultContenxt.HttpContext.User.GetUserId();
           var uow = resultContenxt.HttpContext.RequestServices.GetService<IUnitOfWork>();
           var user = await uow.UserRepository.GetUserByIdAsync(userId);
           user.LastActive = DateTime.UtcNow;
           await uow.Complete();
        }
    }
}