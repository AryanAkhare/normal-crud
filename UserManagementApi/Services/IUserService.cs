using UserManagementApi.Models;

namespace UserManagementApi.Services;

public interface IUserService
{
    IReadOnlyCollection<User> GetAll();

    User? GetById(Guid id);

    User Create(CreateUserRequest request);

    User? Update(Guid id, UpdateUserRequest request);

    bool Delete(Guid id);
}
