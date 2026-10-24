using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using UserManagementApi.Models;
using Xunit;

namespace UserManagementApi.Tests;

public sealed class UsersApiTests
{
    [Fact]
    public async Task GetUsers_ReturnsEmptyCollectionInitially()
    {
        using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        var users = await client.GetFromJsonAsync<User[]>("/api/users");

        Assert.NotNull(users);
        Assert.Empty(users);
    }

    [Fact]
    public async Task CreateUser_ReturnsCreatedUserThatCanBeRetrieved()
    {
        using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/users",
            new CreateUserRequest { Name = "Ada Lovelace", Email = "ada@example.com" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var createdUser = await response.Content.ReadFromJsonAsync<User>();
        Assert.NotNull(createdUser);

        var retrievedUser = await client.GetFromJsonAsync<User>($"/api/users/{createdUser.Id}");
        Assert.Equal(createdUser, retrievedUser);

        var users = await client.GetFromJsonAsync<User[]>("/api/users");
        Assert.NotNull(users);
        Assert.Contains(createdUser, users);
    }

    [Fact]
    public async Task CreateUser_WithInvalidInput_ReturnsBadRequest()
    {
        using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/users",
            new CreateUserRequest { Name = "", Email = "not-an-email" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateAndDeleteUser_ReturnExpectedResponses()
    {
        using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();
        var createResponse = await client.PostAsJsonAsync(
            "/api/users",
            new CreateUserRequest { Name = "Ada Lovelace", Email = "ada@example.com" });
        var createdUser = await createResponse.Content.ReadFromJsonAsync<User>();
        Assert.NotNull(createdUser);

        var updateResponse = await client.PutAsJsonAsync(
            $"/api/users/{createdUser.Id}",
            new UpdateUserRequest { Name = "Ada Byron", Email = "ada.byron@example.com" });

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        var updatedUser = await updateResponse.Content.ReadFromJsonAsync<User>();
        Assert.NotNull(updatedUser);
        Assert.Equal("Ada Byron", updatedUser.Name);
        Assert.Equal("ada.byron@example.com", updatedUser.Email);

        var deleteResponse = await client.DeleteAsync($"/api/users/{createdUser.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getAfterDelete = await client.GetAsync($"/api/users/{createdUser.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getAfterDelete.StatusCode);
    }

    [Fact]
    public async Task OperationsForMissingUser_ReturnNotFound()
    {
        using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();
        var missingId = Guid.NewGuid();

        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"/api/users/{missingId}")).StatusCode);
        Assert.Equal(
            HttpStatusCode.NotFound,
            (await client.PutAsJsonAsync(
                $"/api/users/{missingId}",
                new UpdateUserRequest { Name = "Ada", Email = "ada@example.com" })).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.DeleteAsync($"/api/users/{missingId}")).StatusCode);
    }
}
