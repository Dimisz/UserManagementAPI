using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// In-memory user store
var users = new Dictionary<int, AppUser>
{
  [1] = new AppUser { Id = 1, FirstName = "Alice", LastName = "Smith", Email = "alice@example.com" },
  [2] = new AppUser { Id = 2, FirstName = "Bob", LastName = "Johnson", Email = "bob@example.com" },
  [3] = new AppUser { Id = 3, FirstName = "Charlie", LastName = "Brown", Email = "charlie@example.com" }
};

// GET all users
app.MapGet("/api/users", () => Results.Ok(users.Values));

// GET user by ID
app.MapGet("/api/users/{id:int}", (int id) =>
{
  return users.TryGetValue(id, out var user)
      ? Results.Ok(user)
      : Results.NotFound($"User with ID {id} not found.");
});

// POST create user
app.MapPost("/api/users", (AppUser user) =>
{
  if (users.ContainsKey(user.Id))
    return Results.Conflict($"User with ID {user.Id} already exists.");

  users[user.Id] = user;
  return Results.Created($"/api/users/{user.Id}", user);
});

// PUT update user
app.MapPut("/api/users/{id:int}", (int id, AppUser updatedUser) =>
{
  if (!users.ContainsKey(id))
    return Results.NotFound($"User with ID {id} not found.");

  users[id] = updatedUser;
  return Results.NoContent();
});

// DELETE user
app.MapDelete("/api/users/{id:int}", (int id) =>
{
  if (!users.Remove(id))
    return Results.NotFound($"User with ID {id} not found.");

  return Results.NoContent();
});

app.Run();

public class AppUser
{
  public int Id { get; set; }
  public required string FirstName { get; set; }
  public required string LastName { get; set; }
  public required string Email { get; set; }
}





