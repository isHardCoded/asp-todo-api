using Microsoft.EntityFrameworkCore;
using todo_api.Models;

namespace todo_api.Data;

public class TodoDb : DbContext
{
    public TodoDb(DbContextOptions<TodoDb> options) : base(options) {}
    public DbSet<Todo> Todos => Set<Todo>();
}