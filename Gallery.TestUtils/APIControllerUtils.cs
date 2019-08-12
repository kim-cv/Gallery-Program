using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Gallery.TestUtils
{
    public class APIControllerUtils
    {
        public static ControllerContext CreateApiControllerContext(string claimName)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, claimName)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };

            return context;
        }
    }
}