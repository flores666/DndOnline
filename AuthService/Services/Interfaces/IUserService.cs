using AuthService.DataAccess.Objects;
using AuthService.Models;

namespace AuthService.Services.Interfaces;

public interface IUserService
{
    public Response Register(RegisterModel model);
    public User Get(string name);
    public void Update(User user);
    public bool Delete(int id);
    public Response Authenticate(LoginModel model);
    public Response Logout(string name);
}