using UserManagementApi.Models;

namespace UserManagementApi.Services;

public sealed class InMemoryUserService : IUserService
{
    private readonly Dictionary<Guid, User> users = [];
    private readonly object syncRoot = new();

    public IReadOnlyCollection<User> GetAll()
    {
        lock (syncRoot)
        {
            return users.Values.OrderBy(user => user.Name).ToArray();
        }
    }

    public User? GetById(Guid id)
    {
        lock (syncRoot)
        {
            return users.GetValueOrDefault(id);
        }
    }

    public User Create(CreateUserRequest request)
    {
        var user = new User(Guid.NewGuid(), request.Name, request.Email);

        lock (syncRoot)
        {
            users.Add(user.Id, user);
        }

        return user;
    }

    public User? Update(Guid id, UpdateUserRequest request)
    {
        lock (syncRoot)
        {
            if (!users.ContainsKey(id))
            {
                return null;
            }

            var updatedUser = new User(id, request.Name, request.Email);
            users[id] = updatedUser;
            return updatedUser;
        }
    }

    public bool Delete(Guid id)
    {
        lock (syncRoot)
        {
            return users.Remove(id);
        }
    }
}
