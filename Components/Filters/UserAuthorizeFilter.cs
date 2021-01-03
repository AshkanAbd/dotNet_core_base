using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using dotNet_base.Components.Extensions;
using dotNet_base.Components.Extensions;
using dotNet_base.Components.Response;
using dotNet_base.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace dotNet_base.Components.Filters
{
    public class UserAuthorizeFilter : ActionFilterAttribute
    {
        private readonly BaseContext _dbContext;

        public UserAuthorizeFilter(BaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var authorizedUser = context.HttpContext.User;
            if (!authorizedUser.Identity.IsAuthenticated) {
                await next();
                return;
            }

            var user = await GetAuthenticatedUser(authorizedUser);
            if (user == null) {
                context.Result = ResponseFormat.NotAuthMsg();
                return;
            }

            switch (CheckUserAccount(authorizedUser, user)) {
                case 0:
                    context.Result = ResponseFormat.PermissionDeniedMsg("حساب کاربری شما غیرفعال شده است.");
                    return;
                case 2:
                    context.Result = ResponseFormat.PermissionDeniedMsg("لطفا ابتدا حساب کاربری را تکمیل کنید.");
                    break;
                case 3:
                    context.Result =
                        ResponseFormat.PermissionDeniedMsg("حساب کاربری شما قبلا تکمیل شده است، لطفا مجددا وارد شوید.");
                    break;
            }

            var routePolicy = GetRoutePolicy(context.ActionDescriptor.EndpointMetadata);

            if (CheckRoutePolicy(authorizedUser, routePolicy)) {
                if (context.Result == null) {
                    ((ControllerExtension) context.Controller).AuthenticatedUser = user;
                    await next();
                }
            }
            else {
                context.Result = null;
                ((ControllerExtension) context.Controller).AuthenticatedUser = user;
                await next();
            }
        }

        private async Task<object> GetAuthenticatedUser(ClaimsPrincipal authorizedUser)
        {
            if (authorizedUser.IsInRole(Policies.Admin)) {
                return await _dbContext.Admins
                    .Include(x => x.Role)
                    .ThenInclude(x => x.RolePermissions)
                    .ThenInclude(x => x.Permission)
                    .FirstOrDefaultAsync(x =>
                        x.Id == long.Parse(authorizedUser.FindFirstValue("id")));
            }

            if (authorizedUser.IsInRole(Policies.Owner)) {
                return await _dbContext.Owners.FirstOrDefaultAsync(x =>
                    x.Id == long.Parse(authorizedUser.FindFirstValue("id")));
            }

            if (authorizedUser.IsInRole(Policies.User)) {
                return await _dbContext.Users.FirstOrDefaultAsync(x =>
                    x.Id == long.Parse(authorizedUser.FindFirstValue("id")));
            }

            if (authorizedUser.IsInRole(Policies.IncompleteUser)) {
                return await _dbContext.Users.FirstOrDefaultAsync(x =>
                    x.Id == long.Parse(authorizedUser.FindFirstValue("id")));
            }

            return null;
        }

        private int CheckUserAccount(ClaimsPrincipal authorizedUser, object user)
        {
            if (authorizedUser.IsInRole(Policies.Admin)) {
                return ((Admin) user).Active ? 1 : 0;
            }

            if (authorizedUser.IsInRole(Policies.Owner)) {
                return 1;
            }

            if (authorizedUser.IsInRole(Policies.User)) {
                return !((User) user).Active ? 0 : ((User) user).IsCompleted ? 1 : 2;
            }

            if (authorizedUser.IsInRole(Policies.IncompleteUser)) {
                return !((User) user).Active ? 0 : ((User) user).IsCompleted ? 3 : 1;
            }

            return 0;
        }

        private bool CheckRoutePolicy(ClaimsPrincipal authorizedUser, string routePolicy)
        {
            return routePolicy != null && authorizedUser.IsInRole(routePolicy);
        }

        private string GetRoutePolicy(IEnumerable<object> endPointMetadata)
        {
            var authorizeAttributes = endPointMetadata.OfType<AuthorizeAttribute>().ToList();
            if (authorizeAttributes.Count == 0) return null;
            var authorizeAttribute = authorizeAttributes[0];
            return authorizeAttribute.Policy;
        }
    }
}