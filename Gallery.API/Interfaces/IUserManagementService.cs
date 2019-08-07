namespace Gallery.API.Interfaces
{
    public interface IUserManagementService
    {
        bool IsValidUser(string username, string password);
    }
}
