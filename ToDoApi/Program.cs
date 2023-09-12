using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using ToDoApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddTransient<DataSeeder>();
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => { c.EnableAnnotations(); });


void SeedData(IHost app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

    using (var scope = scopedFactory.CreateScope())
    {
        var service = scope.ServiceProvider.GetService<DataSeeder>();
        service.Seed();
    }
}

var app = builder.Build();
SeedData(app);
// Configure the HTTP request pipeline.

app.MapSwagger();
    app.UseSwaggerUI();


app.UseHttpsRedirection();


app.MapGet("/todoitems", [SwaggerOperation(
        Summary = "returns list of todoitems",
        Description = "Gets the list of ToDo items")] async (TodoDb db) =>
    await db.Todos.ToListAsync())
  ;



app.MapGet("/todoitems/{id}", [SwaggerOperation(
        Summary = "Return the ToDo item based on id..",
        Description = "Fetch the ToDo item from the list based on id if exist.")] async (string id, TodoDb db) =>
    await db.Todos.FindAsync(Guid.Parse(id))
        is Todo todo
            ? Results.Ok(todo)
            : Results.NotFound());


app.MapPost("/todoitems", [SwaggerOperation(
        Summary = "Create new ToDo item.",
        Description = "Add new ToDo item to the list.")]
async (Todo todo, TodoDb db) =>
{
    db.Todos.Add(todo);
    await db.SaveChangesAsync();

    return Results.Created($"/todoitems/{todo.Id}", todo);
});

app.MapPut("/todoitems/{id}", [SwaggerOperation(
        Summary = "Update ToDo item.",
        Description = "Update the exiting ToDo item in the list.")] async (string id, Todo inputTodo, TodoDb db) =>
{
    var todo = await db.Todos.FindAsync(Guid.Parse(id));

    if (todo is null) return Results.NotFound();

    todo.Name = inputTodo.Name;
    todo.IsComplete = inputTodo.IsComplete;

    await db.SaveChangesAsync();

    return Results.NoContent();
})
    .WithDescription("Update particular todo item.");


app.MapPut("/todoitems/complete/{id}", [SwaggerOperation(
        Summary = "Complete ToDo item.",
        Description = "Complete the exiting ToDo item in the list.")] async (string id, TodoDb db) =>
{
    var todo = await db.Todos.FindAsync(Guid.Parse(id));

    if (todo is null) return Results.NotFound();

    
    todo.IsComplete = true;

    await db.SaveChangesAsync();

    return Results.NoContent();
})
    .WithDescription("Update particular todo item.");

app.MapDelete("/todoitems/{id}", [SwaggerOperation(
        Summary = "Delete the ToDo item from the list.",
        Description = "Remove the Todo item from the list.")] async (string id, TodoDb db) =>
{
    if (await db.Todos.FindAsync(Guid.Parse(id)) is Todo todo)
    {
        db.Todos.Remove(todo);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    return Results.NotFound();
}); 


app.Run();


