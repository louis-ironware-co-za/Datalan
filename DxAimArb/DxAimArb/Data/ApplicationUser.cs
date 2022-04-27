using Microsoft.AspNetCore.Identity;

namespace DxAimArb.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string? firstName { get; set; }
        public string? lastName { get; set; }
    }

    //add all other onboarding info here

}
