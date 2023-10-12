using Microsoft.EntityFrameworkCore;

namespace ToDoGrpc;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options){}

    public DbSet<ToDoItem> ToDoItems => Set<ToDoItem>();
}