using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace API.Tests;

public class UserApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public UserApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllUsers_ReturnsSuccessAndUsers()
    {
        var response = await _client.GetAsync("/api/users");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var users = await response.Content.ReadFromJsonAsync<List<AppUser>>();
        users.Should().NotBeNull().And.HaveCountGreaterThan(0);
    }

    [Fact]
    public async Task GetUserById_ReturnsCorrectUser()
    {
        var response = await _client.GetAsync("/api/users/1");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var user = await response.Content.ReadFromJsonAsync<AppUser>();
        user.Should().NotBeNull();
        user!.Id.Should().Be(1);
    }

    [Fact]
    public async Task CreateUser_ReturnsCreated()
    {
        var newUser = new AppUser
        {
            Id = 99,
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com"
        };

        var response = await _client.PostAsJsonAsync("/api/users", newUser);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task UpdateUser_ReturnsNoContent()
    {
        var updatedUser = new AppUser
        {
            Id = 2,
            FirstName = "Updated",
            LastName = "User",
            Email = "updated@example.com"
        };

        var response = await _client.PutAsJsonAsync("/api/users/2", updatedUser);
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteUser_ReturnsNoContent()
    {
        var response = await _client.DeleteAsync("/api/users/3");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
