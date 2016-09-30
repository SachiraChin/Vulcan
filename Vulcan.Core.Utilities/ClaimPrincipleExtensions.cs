namespace Vulcan.Core.Utilities
{
    public static class ClaimPrincipleExtensions
    {

        //public static ClaimUserInfo ExtractClaimPrinciple(this IPrincipal user)
        //{
        //    var claimInfo = new ClaimUserInfo();
        //    if (user == null) return claimInfo;

        //    var claimsIdentity = user.Identity as ClaimsIdentity;
        //    if (claimsIdentity == null) return claimInfo;

        //    var grant = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "type");
        //    if (grant == null) return claimInfo;

        //    var grantVal = grant.Value;
        //    switch (grantVal)
        //    {
        //        case "User":
        //            claimInfo.Username = claimsIdentity.Name;
        //            var userSystemId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "UserSystemId");
        //            if (userSystemId != null)
        //                claimInfo.UserSystemId = long.Parse(userSystemId.Value);
        //            return claimInfo;
        //        case "Application":
        //            claimInfo.ClientId = claimsIdentity.Name;
        //            var clientSystemId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "ClientSystemId");
        //            if (clientSystemId != null)
        //                claimInfo.UserSystemId = long.Parse(clientSystemId.Value);
        //            return claimInfo;
        //    }

        //    return claimInfo;
        //}
    }
}
