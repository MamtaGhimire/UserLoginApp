using UserLoginApp.Services;
using UserLoginApp.Models;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoDBSettings>(  //binds the mongoDB from json.
    builder.Configuration.GetSection("MongoDB"));

builder.Services.AddSingleton<UserService>();  //this register userservice for dependency injection

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();   // enables swagger for testing

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "User Login App is running!");

app.MapGet("/users", (UserService service) => //get's all users
{
    return service.GetAllUsers();
});

app.MapPost("/users", (User user, UserService service) =>
{
    service.CreateUser(user);
    return Results.Created($"/user/{user.Username}", user);
});

app.MapGet("/users/{id}", (string id, UserService service) =>
{
    var user = service.GetUserById(id);
    return user is not null ? Results.Ok(user) : Results.NotFound();
});

// PUT update user
app.MapPut("/users/{id}", (string id, User updatedUser, UserService service) =>
{
    var existingUser = service.GetUserById(id);
    if (existingUser is null) return Results.NotFound();
    
    service.UpdateUser(id, updatedUser);
    return Results.Ok(updatedUser);
});

// DELETE user
app.MapDelete("/users/{id}", (string id, UserService service) =>
{
    var user = service.GetUserById(id);
    if (user is null) return Results.NotFound();

    service.DeleteUser(id);
    return Results.Ok($"User with ID {id} deleted");
});


app.Run();
