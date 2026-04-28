using UserManagementAPI.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Diagnostics;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
        if (contextFeature != null)
        {
            await context.Response.WriteAsJsonAsync(new { Error = "An internal server error occurred." });
        }
    });
});

app.UseHttpsRedirection();


// In-memory data store for boilerplate purposes
var users = new List<User>
{
    new User { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@hypetech.com", Department = "IT", Role = "Developer" },
    new User { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane.smith@hypetech.com", Department = "HR", Role = "Manager" }
};

// GET: /users - Retrieve all users with pagination
app.MapGet("/users", (int page = 1, int pageSize = 10) => 
{
    try 
    {
        var pagedUsers = users.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return Results.Ok(pagedUsers);
    }
    catch (Exception)
    {
        return Results.BadRequest("Invalid pagination parameters.");
    }
})
   .WithName("GetUsers")
   .WithOpenApi();


// GET: /users/{id} - Retrieve a user by ID
app.MapGet("/users/{id}", (int id) =>
{
    var user = users.FirstOrDefault(u => u.Id == id);
    return user is not null ? Results.Ok(user) : Results.NotFound();
})
.WithName("GetUserById")
.WithOpenApi();

// POST: /users - Create a new user
app.MapPost("/users", (User newUser) =>
{
    try
    {
        // Validation
        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(newUser);
        if (!Validator.TryValidateObject(newUser, context, validationResults, true))
        {
            return Results.BadRequest(validationResults.Select(r => r.ErrorMessage));
        }

        // Business Logic: Check for duplicate email
        if (users.Any(u => u.Email.Equals(newUser.Email, StringComparison.OrdinalIgnoreCase)))
        {
            return Results.Conflict("A user with this email already exists.");
        }

        newUser.Id = users.Count > 0 ? users.Max(u => u.Id) + 1 : 1;
        users.Add(newUser);
        return Results.Created($"/users/{newUser.Id}", newUser);
    }
    catch (Exception ex)
    {
        return Results.Problem("An error occurred while creating the user.");
    }
})
.WithName("CreateUser")
.WithOpenApi();


// PUT: /users/{id} - Update an existing user
app.MapPut("/users/{id}", (int id, User updatedUser) =>
{
    try
    {
        var index = users.FindIndex(u => u.Id == id);
        if (index == -1) return Results.NotFound();

        // Validation
        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(updatedUser);
        if (!Validator.TryValidateObject(updatedUser, context, validationResults, true))
        {
            return Results.BadRequest(validationResults.Select(r => r.ErrorMessage));
        }

        // Business Logic: Check for duplicate email (excluding current user)
        if (users.Any(u => u.Email.Equals(updatedUser.Email, StringComparison.OrdinalIgnoreCase) && u.Id != id))
        {
            return Results.Conflict("A user with this email already exists.");
        }

        updatedUser.Id = id;
        users[index] = updatedUser;
        return Results.NoContent();
    }
    catch (Exception)
    {
        return Results.Problem("An error occurred while updating the user.");
    }
})
.WithName("UpdateUser")
.WithOpenApi();


// DELETE: /users/{id} - Delete a user
app.MapDelete("/users/{id}", (int id) =>
{
    var user = users.FirstOrDefault(u => u.Id == id);
    if (user is null) return Results.NotFound();

    users.Remove(user);
    return Results.NoContent();
})
.WithName("DeleteUser")
.WithOpenApi();

app.Run();
