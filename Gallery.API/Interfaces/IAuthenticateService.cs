using System;

namespace Gallery.API.Interfaces
{
    public interface IAuthenticateService
    {
        string GenerateTokenForUser(Guid userUid);
    }
}
