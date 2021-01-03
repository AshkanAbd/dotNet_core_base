using Microsoft.AspNetCore.Authorization;

namespace dotNet_base.Models
{
    public class Policies
    {
        public const string Admin = "Admin";
        public const string User = "User";
        public const string IncompleteUser = "IncompleteUser";
        public const string Owner = "Owner";

        public static AuthorizationPolicy AdminPolicy()
        {
            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireRole(Admin).Build();
        }

        public static AuthorizationPolicy UserPolicy()
        {
            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireRole(User).Build();
        }

        public static AuthorizationPolicy IncompleteUserPolicy()
        {
            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireRole(IncompleteUser).Build();
        }

        public static AuthorizationPolicy OwnerPolicy()
        {
            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireRole(Owner).Build();
        }
    }
}