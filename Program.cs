using Microsoft.EntityFrameworkCore;
using todo_api.Data;
using todo_api.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddDbContext<TodoDb>(options => options.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "TodoAPI";
    config.Title = "TodoAPI v1";
    config.Version = "v1";
});

// TODO: Использовать API MapGroup и TypedResults

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(config =>
    {
        config.DocumentTitle = "TodoAPI";
        config.Path = "/swagger";
        config.DocumentPath = "/swagger/{documentName}/swagger.json";
        config.DocExpansion = "list";
    });
}

app.MapGet("/todo-items", async (TodoDb db) =>
{
    await db.Todos.ToListAsync();
});

app.MapGet("/todo-items/complete", async (TodoDb db) =>
{
    await db.Todos.Where(todo => todo.IsComplete).ToListAsync();
});

app.MapGet("/todo-items/{id}", async (int id, TodoDb db) =>
{
    var todo = await db.Todos.FindAsync(id);
    return todo is Todo t ? Results.Ok(t) : Results.NotFound();
});

app.MapPost("todo-items", async (Todo todo, TodoDb db) =>
{
    db.Todos.Add(todo);
    await db.SaveChangesAsync();

    return Results.Created($"/todo-items/{todo.Id}", todo);
});

app.MapPut("/todo-items/{id}", async (int id, Todo inputTodo, TodoDb db) =>
{
    var todo = await db.Todos.FindAsync(id);
    
    if (todo is null) return Results.NotFound();
    
    todo.Name = inputTodo.Name;
    todo.IsComplete = inputTodo.IsComplete;
    
    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/todo-items/{id}", async (int id, TodoDb db) =>
{
    if (await db.Todos.FindAsync(id) is Todo todo)
    {
        db.Todos.Remove(todo);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    return Results.NotFound();
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();