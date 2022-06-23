using DxAimArb.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DxAimArb.Services
{
    public class ApplicationUserClaimsPrincipalFactory :
                        UserClaimsPrincipalFactory<ApplicationUser>
    {
        public ApplicationUserClaimsPrincipalFactory(UserManager<ApplicationUser> userManager,
                                  IOptions<IdentityOptions> optionsAccessor) : base(userManager, optionsAccessor)
        {
        }

        protected override async Task<ClaimsIdentity>
                 GenerateClaimsAsync(ApplicationUser user)
        {
            ClaimsIdentity claims = await base.GenerateClaimsAsync(user);
            //LJ - Copy Role Permissions to Claims here
            #region User permissions
            bool bSuperUser = await base.UserManager.IsInRoleAsync(user, "SuperUser");
            bool bAllowedOrderBooks = await base.UserManager.IsInRoleAsync(user, "AllowedOrderBooks");
            bool bAllowedMarkets = await base.UserManager.IsInRoleAsync(user, "AllowedMarkets");
            bool bDevelopers = await base.UserManager.IsInRoleAsync(user, "Developers");
            bool bAdmin = await base.UserManager.IsInRoleAsync(user, "Admin");
            bool bAllowedExchanges = await base.UserManager.IsInRoleAsync(user, "AllowedExchanges");
            bool bAllowedTrade = await base.UserManager.IsInRoleAsync(user, "AllowedTrade");
            bool bAllowedWallets = await base.UserManager.IsInRoleAsync(user, "AllowedWallets");
            bool bAllowedProfile = await base.UserManager.IsInRoleAsync(user, "AllowedProfile");

            claims.AddClaim(new Claim("ProfileID", user.Id));
            claims.AddClaim(new Claim("bSuperUser", bSuperUser.ToString()));
            claims.AddClaim(new Claim("bAllowedOrderBooks", bAllowedOrderBooks.ToString()));
            claims.AddClaim(new Claim("bAllowedMarkets", bAllowedMarkets.ToString()));
            claims.AddClaim(new Claim("bDevelopers", bDevelopers.ToString()));
            claims.AddClaim(new Claim("bAdmin", bAdmin.ToString()));
            claims.AddClaim(new Claim("bAllowedExchanges", bAllowedExchanges.ToString()));
            claims.AddClaim(new Claim("bAllowedTrade", bAllowedTrade.ToString()));
            claims.AddClaim(new Claim("bAllowedWallets", bAllowedWallets.ToString()));
            claims.AddClaim(new Claim("bAllowedProfile", bAllowedProfile.ToString()));
            #endregion

            //We also need the user id to filter the tables by this user
            claims.AddClaim(new Claim("UserID", user.Id.ToString()));
            if (user.firstName != null)
                claims.AddClaim(new Claim("FirstName", user.firstName));
            if (user.lastName != null)
                claims.AddClaim(new Claim("LastName", user.lastName));
            claims.AddClaim(new Claim("Email", user.Email));

            return claims;
        }

    }
}
