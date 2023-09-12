using Microsoft.EntityFrameworkCore;
using System;

namespace ToDoApi
{
    public class TodoDb : DbContext
    {
        public TodoDb(DbContextOptions<TodoDb> options)
            : base(options) { }



        public DbSet<Todo> Todos => Set<Todo>();

       
    }

    public class DataSeeder
    {
        private readonly TodoDb toDoDbContext;

        public DataSeeder(TodoDb toDoDbContext)
        {
            this.toDoDbContext  = toDoDbContext;
        }

        public void Seed()
        {
            if (!toDoDbContext.Todos.Any())
            {
                var todos = new List<Todo>()
                {
                    new Todo() { Id=Guid.NewGuid(), IsComplete= false, Name = "Car Maintenance Service" },
                    new Todo() { Id = Guid.NewGuid(), IsComplete = false, Name = "Schedule dentist appointment for kids" },
                    new Todo() { Id = Guid.NewGuid(), IsComplete = false, Name = "Prepare for Open AI presentation & demo" },
                    new Todo() { Id = Guid.NewGuid(), IsComplete = false, Name = "Get Home and Auto insurance renewal quote" }
                };
            

            toDoDbContext.Todos.AddRange(todos);
             toDoDbContext.SaveChanges();
            }
        }
    }


}
